using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 16, "Aunt Sue")]
    class Day16 : ASolution
    {
        Dictionary<int, Dictionary<string, int?>> sues = new Dictionary<int, Dictionary<string, int?>>();

        public Day16() : base(false)
        {
            

            Input
                .Replace("Sue ", string.Empty)
                .Replace(": ", " ")
                .Replace(", ", " ")
                .SplitByNewline(false, true)
                .Select(s => s.Split(' '))
                .ToList()
                .ForEach(p => sues.Add(int.Parse(p[0]), new Dictionary<string, int?> {
                    { p[1], int.Parse(p[2]) },
                    { p[3], int.Parse(p[4]) },
                    { p[5], int.Parse(p[6]) }
                }));
        }

        protected override string SolvePartOne()
        {
            Dictionary<string, int> analysis = new Dictionary<string, int> {
                { "children", 3 },
                { "cats", 7 },
                { "samoyeds", 2 },
                { "pomeranians", 3 },
                { "akitas", 0 },
                { "vizslas", 0 },
                { "goldfish", 5 },
                { "trees", 3 },
                { "cars", 2 },
                { "perfumes", 1 }
            };

            return sues.Select(kv => (kv.Key, analysis.Count(akv => akv.Value == kv.Value.GetValueOrDefault(akv.Key))))
                .OrderByDescending(t => t.Item2)
                .Select(t => t.Key)
                .First()
                .ToString();
        }

        protected override string SolvePartTwo()
        {
            Dictionary<string, int> analysis = new Dictionary<string, int> {
                { "children", 3 },
                { "cats", 7 },
                { "samoyeds", 2 },
                { "pomeranians", 3 },
                { "akitas", 0 },
                { "vizslas", 0 },
                { "goldfish", 5 },
                { "trees", 3 },
                { "cars", 2 },
                { "perfumes", 1 }
            };

            return sues.Select(kv => {
                var kvv = kv.Value;
                int cnt = 0;
                if( analysis["children"] == kvv.GetValueOrDefault("children") ) cnt++;
                if( analysis["cats"] < kvv.GetValueOrDefault("cats") ) cnt++;
                if( analysis["samoyeds"] == kvv.GetValueOrDefault("samoyed") ) cnt++;
                if( analysis["pomeranians"] > kvv.GetValueOrDefault("pomeranians") ) cnt++;
                if( analysis["akitas"] == kvv.GetValueOrDefault("akitas") ) cnt++;
                if( analysis["vizslas"] == kvv.GetValueOrDefault("vizslas") ) cnt++;
                if( analysis["goldfish"] > kvv.GetValueOrDefault("goldfish") ) cnt++;
                if( analysis["trees"] < kvv.GetValueOrDefault("trees") ) cnt++;
                if( analysis["cars"] == kvv.GetValueOrDefault("cars") ) cnt++;
                if( analysis["perfumes"] == kvv.GetValueOrDefault("perfumes") ) cnt++;

                return (kv.Key, cnt);
                })
                .OrderByDescending(t => t.Item2)
                .Select(t => t.Key)
                .First()
                .ToString();
        }
    }
}
