
namespace LabTomasulo
{
    class ReserveStation
    {
        /* Fields */
        
        public IInstruction instruction = null;

        /* Properties */

        public string ID { get; protected set; }
        public StationType Type { get; private set; }
        public virtual bool Busy { get; set; }
        public IInstruction Instruction
        {
            get { return instruction; }
            set
            {
                instruction = value;
                Vj = Vk = Qj = Qk = A = 0;
            }
        }
        public Phase Phase { get; set; }
        public int Vj { get; set; }
        public int Vk { get; set; }
        public int Qj { get; set; }
        public int Qk { get; set; }
        public int A { get; set; }

        public string InstructionValue
        {
            get { return Instruction.ToString(); }
        }


        /* Constructor */

        public ReserveStation(StationType type, int idx)
        {
            ID = "ER" + idx;
            Type = type;
            Phase = Phase.None;
            Busy = false;
            Instruction = null;
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

        public override string ToString()
        {
            return ID + " (" + Type + ")";
        }
    }

    enum Phase { None, Emitted, Executed, Written }

    enum StationType { None, LoadStore, Add, Mult, Branch }
}
