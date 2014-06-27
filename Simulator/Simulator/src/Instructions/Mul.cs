using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Mul : IInstruction
    {
        private const int Clocks = 3;
        private const int HW = (int)StationType.Mult;

        /* Fields */

        private int r;
        private int rs;
        private int rt;
        private int rd;
        private int result;
        private int passedClocks;
        private Simulator simulator;
        private ReserveStation[] RS;
        private RegisterStat[] RegisterStat;
        private int[] Regs;

        /* Constructor */

        public Mul(int rs, int rt, int rd, Simulator simulator)
        {
            this.rs = rs;
            this.rt = rt;
            this.rd = rd;
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
                if (RS[i].Type == StationType.Mult && !RS[i].Busy)
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
                    RegisterStat[rd].Qi = r;

                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (passedClocks == 0 && simulator.IsHardwareFree[HW] && RS[r].Qj == 0 && RS[r].Qk == 0)
            {
                simulator.IsHardwareFree[HW] = false;
                result = RS[r].Vj * RS[r].Vk;
                passedClocks++;
            }
            else if (passedClocks > 0)
            {
                passedClocks++;
            }

            return passedClocks == Clocks;
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
            return string.Format("MUL R{0}, R{1}, R{2}", rd, rs, rt);
        }
    }
}
