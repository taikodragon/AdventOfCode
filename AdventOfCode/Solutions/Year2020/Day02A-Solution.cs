using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{

    class Day02A : ASolution
    {
        public Day02A() : base(02, 2020, "Password Philosophy", false)
        {
        }

        protected override string SolvePartOne()
        {
            int validPasswords = 0;
            foreach(string password in Input.SplitByNewline()) {
                var parts = password.Split(new char[] { ':', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                int lower, upper;
                lower = int.Parse(parts[0]);
                upper = int.Parse(parts[1]);
                char letter = parts[2][0];
                int count = parts[3].Count(c => c == letter);

                if( count >= lower && count <= upper ) {
                    validPasswords++;
                }
            }

            return validPasswords.ToString();
        }

        protected override string SolvePartTwo()
        {
            int validPasswords = 0;
            foreach( string password in Input.SplitByNewline() ) {
                var parts = password.Split(new char[] { ':', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                int lower, upper;
                lower = int.Parse(parts[0]) - 1;
                upper = int.Parse(parts[1]) - 1;
                char letter = parts[2][0];

                bool hasLower = parts[3][lower] == letter;
                bool hasUpper = parts[3][upper] == letter;

                if( (hasLower && !hasUpper) || (!hasLower && hasUpper) ) {
                    validPasswords++;
                }
            }

            return validPasswords.ToString();
        }
    }
}
