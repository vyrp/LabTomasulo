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

        /* Properties */

        public ReserveStation[] RS { get; private set; }
        public int[] Regs { get; private set; }
        public RegisterStat[] RegisterStat { get; private set; }
        public int Clock { get; private set; }
        public int PC { get; private set; }
        public int CompletedInstructions { get; private set; }
        public bool Completed { get; private set; }

        public float CPI
        {
            get
            {
                return CompletedInstructions > 0 ? ((float)Clock) / CompletedInstructions : float.NaN;
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

        }

        public void Next()
        {
            
        }

        public void FastForward()
        {

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
            Completed = false;

            RS = new ReserveStation[ReserveStationsAmount];
            Regs = new int[RegistersAmount];
            RegisterStat = new RegisterStat[RegistersAmount];
            Clock = 0;
            PC = 0;
            CompletedInstructions = 0;

            for (int i = 0; i < 5; i++)
            {
                RS[i] = new ReserveStation("Load/Store");
            }
            for (int i = 0; i < 3; i++)
            {
                RS[5 + i] = new ReserveStation("Add");
            }
            for (int i = 0; i < 3; i++)
            {
                RS[8 + i] = new ReserveStation("Mult");
            }
        }
    }
}
