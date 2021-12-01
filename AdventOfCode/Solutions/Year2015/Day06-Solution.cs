using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day06 : ASolution
    {
        enum LightState { On, Off, Toggle };
        class Instruction
        {
            public LightState NewState { get; set; }
            public IntCoord Lower { get; set; }
            public IntCoord Upper { get; set; }
        }
        List<Instruction> instructions = new List<Instruction>();
        public Day06() : base(06, 2015, "Probably a Fire Hazard", false)
        {
            

            foreach(string line in Input.SplitByNewline()) {
                string[] lineParts = line.Replace("turn ", string.Empty).Split(new char[] { ' ', ',' });
                instructions.Add(new Instruction {
                    NewState = (LightState)Enum.Parse(typeof(LightState), lineParts[0], true),
                    Lower = new IntCoord(int.Parse(lineParts[1]), int.Parse(lineParts[2])),
                    Upper = new IntCoord(int.Parse(lineParts[4]), int.Parse(lineParts[5]))
                });
            }
        }

        void DoInstructionForRange(Instruction instruction, Action<int,int,LightState> action) {
            for( int ix = instruction.Lower.X; ix <= instruction.Upper.X; ++ix ) {
                for( int iy = instruction.Lower.Y; iy <= instruction.Upper.Y; ++iy ) {
                    action(ix, iy, instruction.NewState);
                }
            }
        }

        protected override string SolvePartOne()
        {
            bool[,] lights = new bool[1000, 1000];

            foreach(var inst in instructions) {
                DoInstructionForRange(inst, (ix, iy, newState) =>
                    lights[ix, iy] = inst.NewState == LightState.Toggle ? !lights[ix, iy] : inst.NewState == LightState.On
                );
            }

            StringBuilder sb = new StringBuilder(1000 * 1000 + 1000);
            DoInstructionForRange(new Instruction { Lower = new IntCoord(0, 0), Upper = new IntCoord(999, 999) },
                (ix, iy, _) => sb.Append(string.Concat(iy == 0 ? "\n" : "", lights[ix, iy] ? '#' : '-')));
            Trace.WriteLine(sb);

            int lit = 0;
            foreach( bool light in lights ) lit = light ? lit + 1 : lit;
            return lit.ToString();

        }

        protected override string SolvePartTwo()
        {
            int[,] lights = new int[1000, 1000];

            foreach( var inst in instructions ) {
                DoInstructionForRange(inst, (ix, iy, newState) =>
                    lights[ix, iy] = Math.Max(0, lights[ix,iy] + (inst.NewState == LightState.Toggle ? 2 : (inst.NewState == LightState.On ? 1 : -1)))
                );
            }

            StringBuilder sb = new StringBuilder(1000 * 1000 + 1000);
            DoInstructionForRange(new Instruction { Lower = new IntCoord(0, 0), Upper = new IntCoord(999, 999) },
                (ix, iy, _) => sb.Append(string.Concat(iy == 0 ? "\n" : "", lights[ix, iy] == 0 ? "-" : Math.Min(9, lights[ix,iy]).ToString())));
            Trace.WriteLine(sb);

            int brightness = 0;
            foreach( int light in lights ) brightness += light;
            return brightness.ToString();
        }
    }
}
