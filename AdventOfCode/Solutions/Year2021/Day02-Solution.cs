using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day02 : ASolution
    {
        List<(string command, int value)> input;

        public Day02() : base(02, 2021, "Dive!", false)
        {
            input = Input.SplitByNewline(false, true)
                .Select(s => {
                    var parts = s.Split(' ');
                    return (parts[0], int.Parse(parts[1]));
                })
                .ToList();
        }

        protected override string SolvePartOne()
        {
            int horiz = 0, depth = 0;

            foreach(var (command, value) in input) {
                switch (command) {
                    case "forward": horiz += value; break;
                    case "down": depth += value; break;
                    case "up": depth -= value; break;
                    default: throw new Exception("unknown command");
                }
            }

            return (horiz * depth).ToString();
        }

        protected override string SolvePartTwo()
        {
            int horiz = 0, depth = 0, aim = 0;

            foreach (var (command, value) in input) {
                switch (command) {
                    case "forward": horiz += value; depth += aim * value; break;
                    case "down": aim += value; break;
                    case "up": aim -= value; break;
                    default: throw new Exception("unknown command");
                }
            }

            return (horiz * depth).ToString();
        }
    }
}
