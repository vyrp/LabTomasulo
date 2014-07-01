using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Bne : IInstruction
    {
        private const int HW = (int)StationType.Branch;

        /* Fields */

        private int r;
        private bool result;
        private int pc;
        private readonly int rs;
        private readonly int rt;
        private readonly int imm;
        private readonly Simulator simulator;
        private readonly ReserveStation[] RS;
        private readonly RegisterStat[] RegisterStat;
        private readonly int[] Regs;

        /* Constructor */

        public Bne(int rs, int rt, int imm, Simulator simulator)
        {
            this.rs = rs;
            this.rt = rt;
            this.imm = imm;
            this.simulator = simulator;

            RS = simulator.RS;
            RegisterStat = simulator.RegisterStat;
            Regs = simulator.Regs;
        }

        /* Public Methods */

        public bool TryEmit()
        {
            pc = simulator.PC;
            for (int i = 1; i < RS.Length; i++)
            {
                if (RS[i].Type == StationType.Branch && !RS[i].Busy)
                {
                    r = i;
                    RS[r].Instruction = this;
                    RS[r].Phase = Phase.Emitted;

                    if (RegisterStat[rs].Qi != 0)
                    {
                        RS[r].Qj = RegisterStat[rs].Qi;
                    }
                    else
                    {
                        RS[r].Vj = Regs[rs];
                        RS[r].Qj = 0;
                    }

                    if (RegisterStat[rt].Qi != 0)
                    {
                        RS[r].Qk = RegisterStat[rt].Qi;
                    }
                    else
                    {
                        RS[r].Vk = Regs[rt];
                        RS[r].Qk = 0;
                    }

                    RS[r].Busy = true;

                    simulator.IsBranching = true;
                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (simulator.IsHardwareFree[HW] && RS[r].Qj == 0 && RS[r].Qk == 0)
            {
                simulator.IsHardwareFree[HW] = false;
                result = (RS[r].Vj != RS[r].Vk);
                return true;
            }

            return false;
        }

        public bool TryWrite()
        {
            simulator.IsHardwareFree[HW] = true;
            simulator.CompletedInstructions++;

            if (result)
            {
                simulator.PC = imm;
                simulator.UpdateCurrentInstruction(imm);
            }

            RS[r].Busy = false;

            simulator.IsBranching = false;
            return true;
        }

        public IInstruction Clone()
        {
            return new Bne(rs, rt, imm, simulator);
        }

        public override string ToString()
        {
            return string.Format("BNE R{0}, R{1}, {2}", rs, rt, imm);
        }
    }
}
