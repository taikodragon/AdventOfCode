using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day08 : ASolution
    {
        AsmProcessor pc = new AsmProcessor();
        List<string> lines;
        public Day08() : base(08, 2020, "Handheld Halting")
        {
            UseDebugInput = false;

            lines = Input.SplitByNewline();
        }

        protected override string SolvePartOne()
        {
            bool didFault;
            return pc.Run(AsmProcessor.ParseProgram(lines), out didFault).ToString();
        }

        protected override string SolvePartTwo()
        {
            int modPos = 0;
            bool didFault = true;
            long acc;
            List<Instruction> program = AsmProcessor.ParseProgram(lines);
            do {
                acc = pc.Run(program, out didFault);
                if( didFault ) {
                    program = AsmProcessor.ParseProgram(lines);

                    int nextMod = program.FindIndex(modPos, i => (i.Op == Operation.nop && i.Arg != 0) || i.Op == Operation.jmp);
                    if( nextMod == -1 ) {
                        return "FAILED";
                    }

                    modPos = nextMod + 1;
                    program[nextMod].Op = program[nextMod].Op == Operation.nop ? Operation.jmp : Operation.nop;
                }
            } while( didFault );
            return acc.ToString();
        }
    }
}
