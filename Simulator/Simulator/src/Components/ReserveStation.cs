using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class ReserveStation
    {
        /* Fields */

        public readonly string ID;
        public readonly StationType Type;
        public IInstruction Instruction = null;
        public Phase Phase = Phase.None;
        public bool busy = false;
        public int Vj = 0;
        public int Vk = 0;
        public int Qj = 0;
        public int Qk = 0;
        public int A = 0;

        /* Property */

        public virtual bool Busy
        {
            get { return busy; }
            set { busy = value; }
        }

        /* Constructor */

        public ReserveStation(StationType type, int idx)
        {
            ID = "ER" + idx;
            Type = type;
        }

        /* Public Methds */

        public virtual void Execute()
        {
            if (Busy && Phase == Phase.Emitted && Instruction.TryExecute())
            {
                Phase = Phase.Executed;
            }
        }

        public virtual void Write()
        {
            if (Busy && Phase == Phase.Executed && Instruction.TryWrite())
            {
                Phase = Phase.Written;
            }
        }
    }

    enum Phase { None, Emitted, Executed, Written }

    enum StationType { LoadStore, Add, Mult, Branch }
}
