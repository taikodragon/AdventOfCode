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
    class ProcessorState
    {
        public ProcessorState(List<Instruction> prog)
        {
            Program = prog;
        }

        public List<Instruction> Program { get; }
        public int InstructionPointer { get; set; }
        public Instruction CurrentInstruction {
            get {
                return InstructionPointer < Program.Count ? Program[InstructionPointer] : null;
            }
        }
        public long Accumulator { get; set; }
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

        public void RunOp(ProcessorState ps)
        {
            switch(Op) {
                case Operation.acc: Op_Acc(ps); break;
                case Operation.jmp: Op_Jmp(ps); break;
                case Operation.nop: Op_Nop(ps); break;
            }
        }

        void Op_Nop(ProcessorState ps)
        {
            ps.InstructionPointer++;
        }
        void Op_Acc(ProcessorState ps)
        {
            ps.Accumulator += Arg;
            ps.InstructionPointer++;
        }
        void Op_Jmp(ProcessorState ps)
        {
            ps.InstructionPointer += (int)Arg;
        }
    }
    class AsmProcessor
    {
        public static List<Instruction> ParseProgram(List<string> programLines)
        {
            return programLines.Select(l => new Instruction(l)).ToList();
        }

        public long Run(List<Instruction> program, out bool didFault) {
            List<int> pcVisited = new List<int>();

            ProcessorState ps = new ProcessorState(program);
            while( ps.InstructionPointer < program.Count) {
                if( pcVisited.Contains(ps.InstructionPointer) ) {
                    didFault = true;
                    return ps.Accumulator;
                }
                else pcVisited.Add(ps.InstructionPointer);

                Instruction inst = ps.CurrentInstruction;
                if( inst == null ) break;
                inst?.RunOp(ps);
            }
            didFault = false;
            return ps.Accumulator;
        }
    }
}
