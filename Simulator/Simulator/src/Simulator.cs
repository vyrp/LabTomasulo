﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SI = System.Collections.Generic.KeyValuePair<LabTomasulo.StationType, int>;

namespace LabTomasulo
{
    class Simulator
    {
        /* Constants */

        public const int ReserveStationsAmount = 12;
        private const int RegistersAmount = 32;

        private readonly IReadOnlyList<SI> stationsConfig = new List<SI>()
        {
            new SI(StationType.LoadStore, 5),
            new SI(StationType.Add, 3),
            new SI(StationType.Mult, 3),
            new SI(StationType.Branch, 1),
        };

        /* Fields */

        private int[] memory;
        private List<IInstruction> instructions;
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

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines) {
                string opcode = line.Substring(0, 6);

                IInstruction instruction = null;

                switch (opcode)
                {
                    case "000000":
                    {
                        int rs, rt, rd;
                        ParseInstructionTypeR(line, out rs, out rt, out rd);
                        string shamt = line.Substring(21, 5);
                        string function = line.Substring(26, 6);

                        switch (function)
                        {
                            case "100000":
                            {
                                instruction = new Add(rs, rt, rd, this);
                                break;
                            }
                            case "011000":
                            {
                                instruction = new Mul(rs, rt, rd, this);
                                break;
                            }
                            case "000000":
                            {
                                instruction = new Nop();
                                break;
                            }
                            case "100010":
                            {
                                instruction = new Sub(rs, rt, rd, this);
                                break;
                            }
                        }
                        break;
                    }

                    case "001000":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Addi(rs, rt, imm, this);
                        break;
                    }
                    case "000101":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Beq(rs, rt, imm, this);
                        break;
                    }
                    case "000111":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Ble(rs, rt, imm, this);
                        break;
                    }
                    case "000100":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Ble(rs, rt, imm, this);
                        break;
                    }
                    case "100011":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Lw(rs, rt, imm, this);
                        break;
                    }
                    case "101011":
                    {
                        int rs, rt, imm;
                        ParseInstructionTypeI(line, out rs, out rt, out imm);
                        instruction = new Sw(rs, rt, imm, this);
                        break;
                    }

                    case "000010":
                    {
                        int targetAddr;
                        ParseInstructionTypeJ(line, out targetAddr);
                        instruction = new Jmp(targetAddr, this);
                        break;
                    }
                }

                instructions.Add(instruction);
            }
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
            else if (!IsBranching && instructions[PC / 4].Clone().TryEmit())
            {
                PC += 4;
            }

            Clock++;
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
            memory = new int[4 * 1024];
            instructions = new List<IInstruction>();
            reachedEnd = false;

            IsHardwareFree = new[] { false, true, true, true, true };
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
            int i = 1;
            foreach (var config in stationsConfig)
            {
                for (int j = 0; j < config.Value; j++)
                {
                    RS[i++] = new ReserveStation(config.Key, i-1);
                }
            }
        }

        private void ParseInstructionTypeR(String line, out int rs, out int rt, out int rd)
        {
            rs = Convert.ToInt32(line.Substring(6, 5), 2);
            rt = Convert.ToInt32(line.Substring(11, 5), 2);
            rd = Convert.ToInt32(line.Substring(16, 5), 2);
        }

        private void ParseInstructionTypeI(String line, out int rs, out int rt, out int imm)
        {
            rs = Convert.ToInt32(line.Substring(6, 5), 2);
            rt = Convert.ToInt32(line.Substring(11, 5), 2);
            imm = Convert.ToInt16(line.Substring(16, 16), 2);
        }

        private void ParseInstructionTypeJ(String line, out int targetAddr)
        {
            targetAddr = Convert.ToInt32(line.Substring(6, 26), 2);
        }
    }
}
