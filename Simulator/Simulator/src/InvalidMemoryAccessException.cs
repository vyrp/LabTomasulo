using System;

namespace LabTomasulo
{
    class InvalidMemoryAccessException : Exception
    {
        public InvalidMemoryAccessException(string msg) : base(msg) { }
    }
}
