using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo.Arquitetura
{
    class CacheBlock
    {
        public long tag;
        public bool dirty;

        public CacheBlock() {
            tag = -1;
        }
        public CacheBlock(long tag, bool dirty = false)
        {
            this.tag = tag;
            this.dirty = dirty;
        }
    }

    abstract class MemoryUnit
    {
        public int accessTime;
        public MemoryUnit nextLevel;
        public int hits;
        public int misses;

        public MemoryUnit(int accessTime, MemoryUnit nextLevel )
        {
            this.accessTime = accessTime;
            this.nextLevel = nextLevel;
        }

        abstract public CacheBlock hitTest(long address);
        abstract public int put(CacheBlock block);
        abstract public void erase(long address);
        abstract public long getTag(long address);

        public int fetch(long address)
        {
            if (hitTest(address) != null)
            {
                hits++;
                return accessTime; 
            }
            else
            {
                misses++;
            }

            Tuple<int, CacheBlock> ret = nextLevel.fetchAfterMiss(address);

            int time = ret.Item1;
            CacheBlock block = ret.Item2;
            block.tag = getTag(address);
 
            time += put(block);
            return time;
        }

        public Tuple<int, CacheBlock> fetchAfterMiss(long address) {
            CacheBlock here = hitTest(address);
            if (here != null)
            {
                hits++;
                erase(address);
                return new Tuple<int,CacheBlock>(accessTime, here);
            }
            else
            {
                misses++;
            }

            return nextLevel.fetchAfterMiss(address);
        }

        public int write(long address)
        {
            CacheBlock here = hitTest(address);
            if (here != null)
            {
                here.dirty = true;
                hits++;
                return 2*accessTime;
            }
            else
            {
                misses++;
            }

            Tuple<int, CacheBlock> ret = nextLevel.fetchAfterMiss(address);

            int time = ret.Item1;
            CacheBlock block = ret.Item2;
            block.tag = getTag(address);
            block.dirty = true;

            time += put(block);
            return time;
        }
    }
}
