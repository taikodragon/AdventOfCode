using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{
    enum Operation
    {
        nop, acc, jmp
    }
    class Instruction
    {
        public Instruction() => Op = Operation.nop;
        public Instruction(string line)
        {
            string[] parts = line.Split(' ');
            Op = (Operation)Enum.Parse(typeof(Operation), parts[0]);
            Arg = int.Parse(parts[1].Replace("+", string.Empty));
        }
        public Operation Op { get; set; }
        public long Arg { get; set; }
    }
    class AsmProcessor
    {
        public static List<Instruction> ParseProgram(List<string> programLines)
        {
            return programLines.Select(l => new Instruction(l)).ToList();
        }

        public long Run(List<Instruction> program, out bool didFault) {
            List<int> pcVisited = new List<int>();

            long accumulator = 0;
            for(int pc = 0; pc < program.Count;) {
                if( pcVisited.Contains(pc) ) {
                    didFault = true;
                    return accumulator;
                }
                else pcVisited.Add(pc);
                Instruction inst = program[pc];

                switch(inst.Op) {
                    case Operation.nop:
                        pc++;
                        break;
                    case Operation.acc:
                        accumulator += inst.Arg;
                        pc++;
                        break;
                    case Operation.jmp:
                        pc += (int)inst.Arg;
                        break;
                    default:
                        throw new Exception("Invalid Op");
                }
            }
            didFault = false;
            return accumulator;
        }
    }
}
