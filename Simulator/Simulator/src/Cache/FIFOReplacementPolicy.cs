using System.Collections.Generic;

namespace LabTomasulo.Arquitetura
{
    class FIFOReplacementPolicy : IReplacementPolicy
    {
        int numberOfSets;
        int associativity;

        List<int>[] queues;

        public FIFOReplacementPolicy(int sets, int blocks)
        {
            numberOfSets = sets;
            associativity = blocks;

            queues = new List<int>[sets];

            for (int i = 0; i < sets; i++)
            {
                queues[i] = new List<int>();
                for (int j = 0; j < blocks; j++) queues[i].Add(j);
            }
        }

        public void access(int set, int block)
        {
        }

        public void erase(int set, int block)
        {
            //queues[set].Remove(block);
            //queues[set].Add(block);
        }

        public void write(int set, int block)
        {
            queues[set].Remove(block);
            queues[set].Insert(0, block);
        }

        public int getReplacementBlock(int set)
        {
            return queues[set][associativity - 1];
        }
    }
}
