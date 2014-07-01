using System;

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

        public IInstruction Clone()
        {
            return new Nop();
        }

        public override string ToString()
        {
            return "NOP";
        }
    }
}
