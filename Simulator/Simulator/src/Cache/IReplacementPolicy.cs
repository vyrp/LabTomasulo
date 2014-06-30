using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquitetura
{
    interface IReplacementPolicy
    {
        void access(int set, int block);
        void erase(int set, int block);
        void write(int set, int block);
        int getReplacementBlock(int set);
    }
}
