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

        private const int ReserveStationsAmount = 11;
        private const int RegistersAmount = 32;

        /* Fields */

        private int[] memory;
        private List<IInstruction> instructions;
        private bool fileLoaded;
        private bool running;
        private bool reachedEnd;

        /* Properties */

        public bool[] IsHardwareFree { get; private set; }
        public bool IsCDBFree { get; set; }

        public ReserveStation[] RS { get; private set; }
        public int[] Regs { get; private set; }
        public RegisterStat[] RegisterStat { get; private set; }
        public int Clock { get; private set; }
        public int PC { get; private set; }
        public int CompletedInstructions { get; private set; }
        
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
            instructions.Add(new Add(3, 2, 1, this));
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
            else if (instructions[PC / 4].TryEmit())
            {
                PC += 4;
            }
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

        /* Private Methods */

        private void Initialize()
        {
            running = false;
            memory = new int[4 * 1024];
            instructions = new List<IInstruction>();
            fileLoaded = false;
            reachedEnd = false;

            IsHardwareFree = new[] { true, true, true };
            IsCDBFree = true;

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
        }
    }
}
