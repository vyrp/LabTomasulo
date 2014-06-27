using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Simulator
    {
        /* Constants */

        private const int ReserveStationsAmount = 12;
        private const int RegistersAmount = 32;

        /* Fields */

        private int[] memory;
        private List<IInstruction> instructions;
        private bool fileLoaded;
        private bool running;
        private bool reachedEnd;
        private Queue<int> bufferQueue;

        /* Properties */

        public bool[] IsHardwareFree { get; private set; }
        public bool IsCDBFree { get; set; }
        public bool IsBranching { get; set; }

        public ReserveStation[] RS { get; private set; }
        public int[] Regs { get; private set; }
        public RegisterStat[] RegisterStat { get; private set; }
        public int Clock { get; private set; }
        public int PC { get; set; }
        public int CompletedInstructions { get; set; }
        
        public float CPI
        {
            get
            {
                return CompletedInstructions > 0 ? ((float)Clock) / CompletedInstructions : float.NaN;
            }
        }

        public bool Completed
        {
            get
            {
                return reachedEnd && RS.All(rs => !rs.Busy);
            }
        }

        /* Constructor */

        public Simulator()
        {
            Initialize();
        }

        /* Public Methods */

        public void LoadFile(string filename)
        {
            Initialize();
            fileLoaded = true;

            // ...

            Regs[2] = 2;
            Regs[3] = 3;
            Regs[5] = 5;
            Regs[6] = 6;
            Regs[8] = 8;
            Regs[9] = 9;
            instructions.Add(new Mul(3, 2, 1, this));
            instructions.Add(new Ble(9, 8, 0, this));
            instructions.Add(new Mul(6, 5, 4, this));
        }

        public void Next()
        {
            // Order matters!

            Array.ForEach(RS, station => station.Write());
            IsCDBFree = true;

            Array.ForEach(RS, station => station.Execute());

            if (PC / 4 == instructions.Count)
            {
                reachedEnd = true;
            }
            else if (!IsBranching && instructions[PC / 4].TryEmit())
            {
                PC += 4;
            }

            Clock++;
        }

        public void FastForward()
        {
            // ...
        }

        public void Pause()
        {
            lock (this)
            {
                running = false;
            }
        }

        public void EnqueueInBuffer(int r)
        {
            bufferQueue.Enqueue(r);
        }

        public bool IsTopOfBuffer(int r)
        {
            bool result = false;
            if (bufferQueue.Peek() == r)
            {
                result = true;
                bufferQueue.Dequeue();
            }
            return result;
        }

        public int GetMemoryAt(int address)
        {
            return memory[address];
        }

        public void SetMemoryAt(int address, int value)
        {
            memory[address] = value;
        }

        /* Private Methods */

        private void Initialize()
        {
            running = false;
            memory = new int[4 * 1024];
            instructions = new List<IInstruction>();
            fileLoaded = false;
            reachedEnd = false;

            IsHardwareFree = new[] { true, true, true, true };
            IsCDBFree = true;
            IsBranching = false;
            bufferQueue = new Queue<int>();

            RS = new ReserveStation[ReserveStationsAmount+1];
            Regs = new int[RegistersAmount];
            RegisterStat = new RegisterStat[RegistersAmount];
            Clock = 0;
            PC = 0;
            CompletedInstructions = 0;

            RS[0] = new NullStation();
            for (int i = 1; i <= 5; i++)
            {
                RS[i] = new ReserveStation(StationType.LoadStore, i);
            }
            for (int i = 1; i <= 3; i++)
            {
                RS[5 + i] = new ReserveStation(StationType.Add, 5 + i);
            }
            for (int i = 1; i <= 3; i++)
            {
                RS[8 + i] = new ReserveStation(StationType.Mult, 8 + i);
            }
            for (int i = 1; i <= 1; i++)
            {
                RS[11 + i] = new ReserveStation(StationType.Branch, 11 + i);
            }
        }
    }
}
