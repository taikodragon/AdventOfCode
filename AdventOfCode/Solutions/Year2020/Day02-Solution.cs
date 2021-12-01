using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{

    class Day02 : ASolution
    {
        public Day02() : base(02, 2020, "Password Philosophy", false)
        {
            
        }

        Regex extract = new Regex(@"(?<min>\d+)-(?<max>\d+) (?<letter>\w): (?<password>\w+)");
        protected override string SolvePartOne()
        {
            var groups = Input.SplitByNewline()
                .Select(line => extract.Match(line).Groups)
                .ToArray();

            var lineData = groups
                .Select(group => new {
                    Min = int.Parse(group["min"].Value),
                    Max = int.Parse(group["max"].Value),
                    Letter = group["letter"].Value.First(),
                    Password = group["password"].Value
                })
                .ToArray();

            var validCount = lineData
                .Select(data => new {
                    data.Min,
                    data.Max,
                    LetterCount = data.Password.Count(c => c == data.Letter)
                })
                .Count(data => data.LetterCount >= data.Min && data.LetterCount <= data.Max);

            return validCount.ToString();
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
