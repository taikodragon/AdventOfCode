using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 08, "Matchsticks")]
    class Day08 : ASolution
    {


        public Day08() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            string inputRaw = Regex.Replace(Input, "[ \t]+", string.Empty);
            string inputCode = Regex.Replace(inputRaw, "[\n\r]+", string.Empty);
            string inputMem = string.Concat(inputRaw.SplitByNewline().Select(s => s.Trim('"')));
            inputMem = inputMem.Replace("\\\"", "\"");
            inputMem = inputMem.Replace("\\\\", "\\");
            inputMem = Regex.Replace(inputMem, "\\\\x[0-9a-fA-F]{2}", "~");

            return $"{inputCode.Length - inputMem.Length}";
        }

        protected override string SolvePartTwo()
        {
            string inputRaw = Regex.Replace(Input, "[ \t]+", string.Empty);
            string inputCode = Regex.Replace(inputRaw, "[\n\r]+", string.Empty);

            string inputMem = string.Concat(
                inputRaw
                .SplitByNewline()
                .Select(s => s.Replace("\\", "\\\\").Replace("\"", "\\\"") )
                .Select(s => string.Concat("\"", s, "\"") )
                );
            //inputMem = Regex.Replace(inputMem, "\\\\x[0-9a-fA-F]{2}", "\\\\x~~");

            return $"{inputMem.Length - inputCode.Length}";
        }
    }
}
