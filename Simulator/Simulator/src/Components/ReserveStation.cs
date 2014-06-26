using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class ReserveStation
    {
        private static int counter = 0;

        public readonly string ID;
        public readonly string Type;
        public IInstruction Instruction = null;
        public Phase Phase = Phase.None;
        public bool Busy = false;
        public int Vj = 0;
        public int Vk = 0;
        public int Qj = 0;
        public int Qk = 0;
        public int A = 0;

        public ReserveStation(string Type)
        {
            this.ID = "ER" + (++counter);
            this.Type = Type;
        }
    }

    enum Phase { None, Emission, Execution, Writing }
}
