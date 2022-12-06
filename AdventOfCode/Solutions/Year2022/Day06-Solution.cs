using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022
{

    [DayInfo(2022, 06, "Tuning Trouble")]
    class Day06 : ASolution
    {

        public Day06() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            string input = Input;
            int pos = -1;
            for(int i = 0; i < input.Length - 4; i++) {
                if (input.Substring(i, 4).Distinct().Count() == 4) {
                    pos = i + 4;
                    break;
                }
            }

            return pos.ToString();
        }

        protected override string SolvePartTwo()
        {
            string input = Input;
            int pos = -1;
            for (int i = 0; i < input.Length - 14; i++) {
                if (input.Substring(i, 14).Distinct().Count() == 14) {
                    pos = i + 14;
                    break;
                }
            }

            return pos.ToString();
        }
    }
}
