﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    interface IInstruction
    {
        bool TryEmit();
        bool TryExecute();
        bool TryWrite();
    }
}
