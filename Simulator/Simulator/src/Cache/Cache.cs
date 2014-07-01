
namespace LabTomasulo.Arquitetura
{
    class Cache : MemoryUnit
    {
        int numberOfSets;
        int blockSize;
        int associativity;

        public bool writeOnReplace;
        public IReplacementPolicy replacementPolicy;

        CacheBlock[,] blocks;

        public Cache(int size, int blockSize, int accessTime, int associativity, MemoryUnit nextLevel) : base(accessTime, nextLevel)
        {
            this.numberOfSets = size / (blockSize * associativity);
            this.blockSize = blockSize;
            this.associativity = associativity;

            this.blocks = new CacheBlock[numberOfSets, associativity];
            for (int i = 0; i < numberOfSets; i++)
            {
                for (int j = 0; j < associativity; j++) blocks[i, j] = new CacheBlock();
            }
            this.replacementPolicy = new LRUReplacementPolicy(numberOfSets, associativity);
        }

        public override CacheBlock hitTest (long address) {
            long tag = getTag(address);
            int set = (int) (tag % numberOfSets);

            for (int i = 0; i < associativity; i++)
            {
                if (blocks[set, i].tag == tag)
                {
                    replacementPolicy.access(set, i);
                    return blocks[set, i];
                }
            }

            return null;
        }

        public override void erase(long address)
        {
            long tag = getTag(address);
            int set = (int) (tag % numberOfSets);

            for (int i = 0; i < associativity; i++)
            {
                if (blocks[set, i].tag == tag)
                {
                    blocks[set, i] = new CacheBlock();
                    replacementPolicy.erase(set, i);
                    return;
                }
            }
        }

        public override int put(CacheBlock block)
        {
            int time = accessTime;

            long tag = block.tag;
            int set = (int) (tag % numberOfSets);

            int setIndex = replacementPolicy.getReplacementBlock(set);
            replacementPolicy.write(set, setIndex);

            if ( (writeOnReplace || blocks[set, setIndex].dirty) && blocks[set, setIndex].tag >= 0) {
                time += nextLevel.put(blocks[set, setIndex]);
            }

            blocks[set, setIndex] = block;

            return time;
        }

        public override long getTag(long address)
        {
            return (long) (address / blockSize);
        }
    }
}
