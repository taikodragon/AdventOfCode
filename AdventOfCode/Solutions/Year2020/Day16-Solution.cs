using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 16, "Ticket Translation")]
    class Day16 : ASolution
    {
        class Rule
        {
            public string Label;
            public Int2 RangeLow;
            public Int2 RangeHigh;
            public bool IsValidInRange(int num) {
                return (num >= RangeLow.X && num <= RangeLow.Y) || (num >= RangeHigh.X && num <= RangeHigh.Y);
            }
        }

        public Day16() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            List<string> groups = Input.Split("\n\n").ToList();
            List<Rule> rules = new List<Rule>();

            foreach( string line in groups[0].SplitByNewline() ) {
                var parts = line.Split(new string[] { ": ", " or ", "-" }, StringSplitOptions.RemoveEmptyEntries);
                rules.Add(new Rule {
                    Label = parts[0],
                    RangeLow = new Int2(int.Parse(parts[1]), int.Parse(parts[2])),
                    RangeHigh = new Int2(int.Parse(parts[3]), int.Parse(parts[4]))
                });
            }


            return groups[2]
                .SplitByNewline()
                .Skip(1)
                .SelectMany(s => s.Split(',').Select(n => int.Parse(n)))
                .Where(n => !rules.Any(r => r.IsValidInRange(n)))
                .Sum().ToString();
        }

        protected override string SolvePartTwo()
        {
            List<string> groups = Input.Split("\n\n").ToList();
            List<Rule> rules = new List<Rule>();

            groups[0].SplitByNewline()
                .Select(line => line.Split(new string[] { ": ", " or ", "-" }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(parts => rules.Add(new Rule {
                    Label = parts[0],
                    RangeLow = new Int2(int.Parse(parts[1]), int.Parse(parts[2])),
                    RangeHigh = new Int2(int.Parse(parts[3]), int.Parse(parts[4]))
                }));

            var others = groups[2]
                .SplitByNewline()
                .Skip(1)
                .Select(s => s.Split(',').Select(n => int.Parse(n)).ToList())
                .Where(l => l.TrueForAll(n => rules.Any(r => r.IsValidInRange(n))))
                .ToList();

            int tickLength = rules.Count;
            Dictionary<int, Rule> foundRules = new Dictionary<int, Rule>(20);

            List<int> vals = null;
            Func<Rule, bool> ruleTest = rule => vals.TrueForAll(n => rule.IsValidInRange(n));
            while(foundRules.Count < tickLength) {
                for(int i = 0; i < tickLength; ++i) {
                    if( foundRules.ContainsKey(i) ) continue;

                    vals = others.Select(l => l[i]).ToList();

                    if( rules.Count(ruleTest) == 1) {
                        foundRules[i] = rules.First(ruleTest);
                        rules.Remove(foundRules[i]);
                        continue;
                    }
                }
            }

            var mine = groups[1]
                .SplitByNewline()
                .Skip(1)
                .Select(s => s.Split(',').Select(n => int.Parse(n)).ToList())
                .First();

            long mult = 1;
            foreach(int ticketValue in foundRules.Where(p => p.Value.Label.StartsWith("departure") || (UseDebugInput && p.Value.Label == "seat")).Select(p => mine[p.Key])) {
                mult *= ticketValue;
                Trace.WriteLine(ticketValue);
            }

            return mult.ToString();
        }
    }
}
