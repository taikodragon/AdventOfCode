using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day15 : ASolution
    {

        public Day15() : base(15, 2020, "")
        {
            UseDebugInput = false;



        }



        protected override string SolvePartOne()
        {
            List<int> spokenNumbers = Input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToList();

            while(spokenNumbers.Count < 2020) {
                int lastSpokenIndex = spokenNumbers.Count - 1;
                int lastSpoken = spokenNumbers[lastSpokenIndex];
                int distance = 0;
                for(int i = lastSpokenIndex - 1; i >= 0; --i ) {
                    if( spokenNumbers[i] == lastSpoken ) {
                        distance = lastSpokenIndex - i;
                        break;
                    }
                }
                spokenNumbers.Add(distance);
            }



            return spokenNumbers[spokenNumbers.Count - 1].ToString();
        }

        protected override string SolvePartTwo()
        {
            return Part2Caching();
            //return Part2Orderly();
        }

        string Part2Caching() {
            int idx = 0, lastSpoken = -1;
            Dictionary<int, int> spokeCache = new Dictionary<int, int>();
            foreach( int num in Input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)) ) {
                if( lastSpoken >= 0 ) {
                    spokeCache[lastSpoken] = idx - 1;
                }
                idx++;
                lastSpoken = num;
            }

            while( idx < 30000000 ) {
                int distance = 0;
                if( spokeCache.TryGetValue(lastSpoken, out int oldIdx) ) {
                    distance = (idx - 1) - oldIdx;
                }
                spokeCache[lastSpoken] = idx - 1;
                lastSpoken = distance;
                idx++;
            }



            return lastSpoken.ToString();
        }
        string Part2Orderly() {
            List<int> spokenNumbers = new List<int>(30000000);
            spokenNumbers.AddRange(Input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)));

            while( spokenNumbers.Count < 30000000 ) {
                int lastSpokenIndex = spokenNumbers.Count - 1;
                int lastSpoken = spokenNumbers[lastSpokenIndex];
                int distance = 0;
                for( int i = lastSpokenIndex - 1; i >= 0; --i ) {
                    if( spokenNumbers[i] == lastSpoken ) {
                        distance = lastSpokenIndex - i;
                        break;
                    }
                }
                spokenNumbers.Add(distance);
            }



            return spokenNumbers[spokenNumbers.Count - 1].ToString();
        }
    }
}
