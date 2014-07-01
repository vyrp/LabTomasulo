
namespace LabTomasulo
{
    class NullStation : ReserveStation
    {
        public NullStation() : base(StationType.None, 0)
        {
            ID = "0";
        }

        public override bool Busy
        {
            get { return false; }
            set { }
        }

        public override void Execute() { }

        public override void Write() { }
    }
}
