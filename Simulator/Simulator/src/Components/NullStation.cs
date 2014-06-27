using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class NullStation : ReserveStation
    {
        public NullStation() : base(StationType.None, 0) { }

        public override bool Busy
        {
            get { return false; }
            set { }
        }

        public override void Execute() { }

        public override void Write() { }
    }
}
