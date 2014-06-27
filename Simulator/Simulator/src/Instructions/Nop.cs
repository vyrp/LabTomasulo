using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Nop : IInstruction
    {
        /* Public Methods */

        public bool TryEmit()
        {
            return true;
        }

        public bool TryExecute()
        {
            throw new InvalidOperationException("Should not be called");
        }

        public bool TryWrite()
        {
            throw new InvalidOperationException("Should not be called");
        }

        public override string ToString()
        {
            return "NOP";
        }
    }
}
