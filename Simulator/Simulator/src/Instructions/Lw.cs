﻿
namespace LabTomasulo
{
    class Lw : IInstruction
    {
        private const int Clocks = 4;
        private const int HW = (int)StationType.LoadStore;

        /* Fields */

        private int r;
        private int result;
        private int passedClocks = 0;
        private int phase;
        private int clocks = -1;
        private readonly int rs;
        private readonly int rt;
        private readonly int imm;
        private readonly Simulator simulator;
        private readonly ReserveStation[] RS;
        private readonly RegisterStat[] RegisterStat;
        private readonly int[] Regs;

        /* Constructor */

        public Lw(int rs, int rt, int imm, Simulator simulator)
        {
            this.rs = rs;
            this.rt = rt;
            this.imm = imm;
            this.simulator = simulator;
            this.phase = 1;

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

                    RS[r].A = imm;
                    RS[r].Busy = true;

                    RegisterStat[rt].Qi = r;

                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (phase == 1 && simulator.IsHardwareFree[HW] && RS[r].Qj == 0 && simulator.IsTopOfBuffer(r))
            {
                simulator.IsHardwareFree[HW] = false;
                RS[r].A = RS[r].Vj + RS[r].A;
                clocks = (MainWindow.UseCache ? simulator.L1.fetch(RS[r].A) : Clocks);
                phase++;
            }
            else if(phase == 2)
            {
                if (passedClocks == 0)
                {
                    result = simulator.GetMemoryAt(RS[r].A);
                }
                passedClocks++;
            }

            return passedClocks == clocks;
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

        public IInstruction Clone()
        {
            return new Lw(rs, rt, imm, simulator);
        }

        public override string ToString()
        {
            return string.Format("LW R{0}, {1}(R{2})", rt, imm, rs);
        }
    }
}
