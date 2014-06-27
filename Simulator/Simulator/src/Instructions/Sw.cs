﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Sw : IInstruction
    {
        private const int Clocks = 4;
        private const int HW = (int)StationType.LoadStore;

        /* Fields */

        private int r;
        private int rs;
        private int rt;
        private int imm;
        private int result;
        private int passedClocks;
        private Simulator simulator;
        private ReserveStation[] RS;
        private RegisterStat[] RegisterStat;
        private int[] Regs;

        /* Constructor */

        public Sw(int rt, int imm, int rs, Simulator simulator)
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
                if (RS[i].Type == StationType.LoadStore && !RS[i].Busy)
                {
                    r = i;
                    RS[r].Instruction = this;
                    RS[r].Phase = Phase.Emitted;
                    simulator.EnqueueInBuffer(r);

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

                    RS[r].A = imm;
                    RS[r].Busy = true;

                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (passedClocks == 0 && simulator.IsHardwareFree[HW] && RS[r].Qj == 0 && simulator.IsTopOfBuffer(r))
            {
                simulator.IsHardwareFree[HW] = false;
                RS[r].A = RS[r].Vj + RS[r].A;
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
            if (RS[r].Qk != 0)
            {
                return false;
            }

            simulator.IsHardwareFree[HW] = true;
            simulator.CompletedInstructions++;
            simulator.SetMemoryAt(RS[r].A, RS[r].Vk);
            RS[r].Busy = false;

            return true;
        }

        public override string ToString()
        {
            return string.Format("SW R{0}, {1}(R{2})", rt, imm, rs);
        }
    }
}