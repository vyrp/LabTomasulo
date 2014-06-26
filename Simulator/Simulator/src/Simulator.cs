using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTomasulo
{
    class Simulator
    {
        /* Constants */

        private const int ReserveStationsAmount = 11;
        private const int RegistersAmount = 32;

        /* Properties */

        public ReserveStation[] RS { get; private set; }
        public byte[] Regs { get; private set; }
        public RegisterStat[] RegisterStat { get; private set; }
        public int Clock { get; private set; }
        public int PC { get; private set; }
        public int CompletedInstructions { get; private set; }

        public float CPI
        {
            get
            {
                return ((float)Clock) / CompletedInstructions;
            }
        }

        /* Constructor */

        public Simulator()
        {
            RS = new ReserveStation[ReserveStationsAmount];
            Regs = new byte[RegistersAmount];
            RegisterStat = new RegisterStat[RegistersAmount];
            Clock = 0;
            PC = 0;
            CompletedInstructions = 0;

            // Initialize components

            for (int i = 0; i < 5; i++)
            {
                RS[i] = new ReserveStation("Load/Store");
            }
            for (int i=0; i < 3; i++)
            {
                RS[5+i] = new ReserveStation("Add");
            }
            for (int i = 0; i < 3; i++)
            {
                RS[8 + i] = new ReserveStation("Mult");
            }
        }
    }
}
