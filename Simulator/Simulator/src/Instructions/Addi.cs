using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Addi : IInstruction
    {
        private const int HW = (int)StationType.Add;

        /* Fields */

        private int r;
        private int result; 
        private readonly int rt;
        private readonly int rs;
        private readonly int imm;
        private readonly Simulator simulator;
        private readonly ReserveStation[] RS;
        private readonly RegisterStat[] RegisterStat;
        private readonly int[] Regs;

        /* Constructor */

        public Addi(int rt, int rs, int imm, Simulator simulator)
        {
            this.rt = rt;
            this.rs = rs;
            this.imm = imm;
            this.simulator = simulator;

            RS = simulator.RS;
            RegisterStat = simulator.RegisterStat;
            Regs = simulator.Regs;
        }

        /* Public Methods */

        public bool TryEmit()
        {
            for (int i = 1; i < RS.Length; i++)
            {
                if (RS[i].Type == StationType.Add && !RS[i].Busy)
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

                    RS[r].Busy = true;
                    RegisterStat[rt].Qi = r;

                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (simulator.IsHardwareFree[HW] && RS[r].Qj == 0)
            {
                simulator.IsHardwareFree[HW] = false;
                result = RS[r].Vj + imm;
                return true;
            }

            return false;
        }

        public bool TryWrite()
        {
            if (!simulator.IsCDBFree)
            {
                return false;
            }

            simulator.IsCDBFree = false;
            simulator.IsHardwareFree[HW] = true;
            simulator.CompletedInstructions++;

            for (int x = 0; x < RegisterStat.Length; x++)
            {
                if (RegisterStat[x].Qi == r)
                {
                    Regs[x] = result;
                    RegisterStat[x].Qi = 0;
                }
            }

            for (int x = 0; x < RS.Length; x++)
            {
                if (RS[x].Qj == r)
                {
                    RS[x].Vj = result;
                    RS[x].Qj = 0;
                }
            }

            for (int x = 0; x < RS.Length; x++)
            {
                if (RS[x].Qk == r)
                {
                    RS[x].Vk = result;
                    RS[x].Qk = 0;
                }
            }

            RS[r].Busy = false;

            return true;
        }

        public override string ToString()
        {
            return string.Format("ADDi R{0}, R{1}, {2}", rt, rs, imm);
        }
    }
}
