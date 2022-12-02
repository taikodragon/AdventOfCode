using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 10, "Adapter Array")]
    class Day10 : ASolution
    {
        List<int> lines;

        public Day10() : base(false)
        {
            

            lines = Input.SplitByNewline().Select(s => int.Parse(s)).ToList();
            lines.Add(0);
            lines.Sort();
            lines.Add(lines[^1] + 3); // my adapter

        }

        protected override string SolvePartOne()
        {
            int sourceJoltage = 0;
            int ones = 0, threes = 0;
            foreach(int adapter in lines) {
                switch(adapter - sourceJoltage) {
                    case 1:
                        ones++;
                        break;
                    case 3:
                        threes++;
                        break;
                    //default: return "FAIL";
                }
                sourceJoltage = adapter;
            }

            return $"{ones} 1s and {threes} threes for {ones * threes}";
        }

        bool ValidateSet(List<int> adapters) {
            int sourceJoltage = 0;
            foreach( int adapter in adapters ) {
                if( adapter - sourceJoltage > 3) {
                    return false;
                }
                sourceJoltage = adapter;
            }
            return true;
        }

        static long ComputePaths(List<byte> set, int at, Dictionary<int,long> cache) {
            if( cache.TryGetValue(at, out var pathCombination) ) {
                return pathCombination;
            }
            long myPaths = 0;
            if( at+3 < set.Count && set[at+3] - set[at] <= 3 ) {
                myPaths = ComputePaths(set, at + 3, cache);
            }
            if( at + 2 < set.Count && set[at+2] - set[at] <= 3) {
                myPaths += ComputePaths(set, at + 2, cache);
            }
            if( at + 1 < set.Count && set[at+1] - set[at] <= 3) {
                myPaths += ComputePaths(set, at + 1, cache);
            }
            cache.Add(at, myPaths);
            return myPaths;
        }

        protected override string SolvePartTwo()
        {
            List<byte> src = lines.Select(n => (byte)n).ToList();
            Dictionary<int, long> cache = new Dictionary<int, long>() {
                { src.Count-1, 0 },
                { src.Count-2, 1 }
            };

            return ComputePaths(src, 0, cache).ToString();

            // [ Awesome iterative solution was here ]
        }
    }
}
