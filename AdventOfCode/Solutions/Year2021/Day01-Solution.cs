using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day01 : ASolution
    {
        List<int> input = new List<int>();

        public Day01() : base(01, 2021, "Sonar Sweep", false)
        {
            input = Input.SplitByNewline(false, true).Select(int.Parse).ToList();
        }

        protected override string SolvePartOne()
        {
            int count = 0;
            for(int i = 1; i < input.Count; i++) {
                if (input[i] > input[i - 1]) {
                    count++;
                }
            }
            return count.ToString();
        }

        protected override string SolvePartTwo()
        {
            List<int> groups = new List<int>();
            for (int i = 2; i < input.Count; i++) {
                groups.Add(input[i] + input[i - 1] + input[i - 2]);
            }

            int count = 0;
            for (int i = 1; i < groups.Count; i++) {
                if (groups[i] > groups[i - 1]) {
                    count++;
                }
            }

            return count.ToString();
        }
    }
}
