using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2015
{

    class Day08 : ASolution
    {


        public Day08() : base(08, 2015, "")
        {
            UseDebugInput = false;
        }

        protected override string SolvePartOne()
        {
            string inputRaw = Regex.Replace(Input, "[ \t]+", string.Empty);
            string inputCode = Regex.Replace(inputRaw, "[\n\r]+", string.Empty);
            string inputMem = Regex.Replace(inputRaw, "^\"", string.Empty);
            inputMem = Regex.Replace(inputMem, "\"$", string.Empty);
            inputMem = Regex.Replace(inputMem, "\\\"|\\\\\\\\|\\\\x[0-9a-fA-F]{2}", "~");

            return $"{inputCode.Length - inputMem.Length}";
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
