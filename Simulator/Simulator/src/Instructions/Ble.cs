using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Ble : IInstruction
    {
        private const int Clocks = 3;

        /* Fields */

        private int r;
        private int rs;
        private int rt;
        private int imm;
        private bool result;
        private int passedClocks;
        private Simulator simulator;
        private ReserveStation[] RS;
        private RegisterStat[] RegisterStat;
        private int[] Regs;

        /* Constructor */

        public Ble(int rs, int rt, int imm, Simulator simulator)
        {
            this.rs = rs;
            this.rt = rt;
            this.imm = imm;
            this.simulator = simulator;
            this.passedClocks = 0;

            RS = simulator.RS;
            RegisterStat = simulator.RegisterStat;
            Regs = simulator.Regs;
        }

        /* Public Methods */

        public bool TryEmit()
        {
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
            if (simulator.IsHardwareFree[(int)StationType.Branch] && RS[r].Qj == 0 && RS[r].Qk == 0)
            {
                simulator.IsHardwareFree[(int)StationType.Branch] = false;
                result = RS[r].Vj <= RS[r].Vk;
                return true;
            }

            return false;
        }

        public bool TryWrite()
        {
            simulator.IsHardwareFree[(int)StationType.Branch] = true;
            simulator.CompletedInstructions++;

            if (result)
            {
                simulator.PC = imm;
            }

            RS[r].Busy = false;

            simulator.IsBranching = false;
            return true;
        }
    }
}
