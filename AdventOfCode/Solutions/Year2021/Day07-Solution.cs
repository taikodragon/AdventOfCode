using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day07 : ASolution
    {
        List<int> crabPositions;

        public Day07() : base(07, 2021, "The Treachery of Whales", false)
        {
            crabPositions = Input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToList();

        }

        int FuelAtHoriz(int h) {
            int sum = 0;
            foreach (var crab in crabPositions) {
                sum += Math.Abs(crab - h);
            }
            return sum;
        }

        protected override string SolvePartOne()
        {
            int middlish = (crabPositions.Count % 2) == 1 ? crabPositions.Count / 2 + 1 : crabPositions.Count / 2 ;

            int median = (crabPositions[middlish] + crabPositions[middlish+1])/2;

            Dictionary<int, int> result = new();

            for (int i = -2; i < 3; i++) {
                result[median + i] = FuelAtHoriz(median + i);
            }



            return result.Min(kv => kv.Value).ToString();
        }

        Dictionary<int, int> distCache = new Dictionary<int, int>();
        int FuelAtHoriz2(int h) {
            int sum = 0;
            foreach (var crab in crabPositions) {
                int dist = Math.Abs(crab - h);
                int fuel = 0;
                if (distCache.ContainsKey(dist)) {
                    fuel = distCache[dist];
                } else if(dist > 0) {
                    for (int i = 1; i <= dist; i++) {
                        fuel += i;
                    }
                    distCache[dist] = fuel;
                }
                sum += fuel;
            }
            return sum;
        }

        protected override string SolvePartTwo()
        {
            Dictionary<int, int> result = new();

            int min = crabPositions.Min(), max = crabPositions.Max();
            for (int i = min; i <= max; i++) {
                result[i] = FuelAtHoriz2(i);
            }



            return result.Min(kv => kv.Value).ToString();
        }
    }
}
