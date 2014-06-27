using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Jmp : IInstruction
    {
        private const int HW = (int)StationType.Branch;

        /* Fields */

        private int r;
        private int imm;
        private int pc;
        private Simulator simulator;
        private ReserveStation[] RS;
        private RegisterStat[] RegisterStat;
        private int[] Regs;

        /* Constructor */

        public Jmp(int imm, Simulator simulator)
        {
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
                    RS[r].Busy = true;

                    simulator.IsBranching = true;
                    return true;
                }
            }

            return false;
        }

        public bool TryExecute()
        {
            if (simulator.IsHardwareFree[HW])
            {
                simulator.IsHardwareFree[HW] = false;
                return true;
            }

            return false;
        }

        public bool TryWrite()
        {
            simulator.IsHardwareFree[HW] = true;
            simulator.CompletedInstructions++;
            simulator.IsBranching = false;
            simulator.PC = imm;

            RS[r].Busy = false;
            
            return true;
        }

        public override string ToString()
        {
            return string.Format("JMP {0}", imm);
        }
    }
}
