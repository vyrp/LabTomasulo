using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    // Provisiório
    interface IInstruction
    {
        void Emit();
        void Execute();
        void Write();
    }
}
