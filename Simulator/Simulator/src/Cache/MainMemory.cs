using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo.Arquitetura
{
    class MainMemory : MemoryUnit
    {
        public MainMemory(int accessTime) : base(accessTime, null) { }

        public override long getTag(long address) { return address; }

        public override CacheBlock hitTest(long address)
        {
            long tag = getTag(address);
            return new CacheBlock(tag);
        }

        public override void erase(long address) {

        }
        public override int put(CacheBlock block)
        {
            return accessTime;
        }
    }
}
