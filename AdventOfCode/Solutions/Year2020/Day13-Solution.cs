using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions.Year2020
{

    class Day13 : ASolution
    {

        public Day13() : base(13, 2020, "Shuttle Search")
        {
            UseDebugInput = false;

            
        }

        protected override string SolvePartOne()
        {
            List<string> lines = Input.SplitByNewline();
            int minDepartTime = int.Parse(lines[0]);
            int[] busses = lines[1].Split(',').Where(s => s != "x").Select(s => int.Parse(s)).ToArray();

            for(int time = minDepartTime; true; time++ ) {
                var foundBus = busses.Select(b => (b, time % b)).FirstOrDefault(p => p.Item2 == 0);
                if( foundBus != default((int,int)) ) {
                    return (foundBus.b * (time - minDepartTime)).ToString();
                }
            }
            return "FAIL";
        }

        long finalTime;

        protected override string SolvePartTwo()
        {
            int i;
            List<string> lines = Input.SplitByNewline();
            int[] busses = lines[1].Split(',').Select(s => s == "x" ? -1 : int.Parse(s)).ToArray();
            Dictionary<int, int> routes = new Dictionary<int, int>() {  };
            for(i = 0; i < busses.Length; i++ ) {
                if( busses[i] != -1 ) {
                    routes.Add(i, busses[i]);
                }
            }
            int[] keys = routes.Keys.ToArray();
            long jumpBy = busses[0];

            long time = busses[0], lastSuccessfulTime = 7;
            int searchKeyAt = 0, matches, nextMatches = 2;
            while( finalTime == 0 ) {
                for( i = searchKeyAt, matches = 0; i >= 0; --i ) {
                    if( ((time + (keys[i])) % routes[keys[i]]) == 0 ) matches++;
                }
                if( searchKeyAt == matches - 1 ) {
                    if( searchKeyAt + 1 == keys.Length ) {
                        finalTime = time;
                    }

                    if( nextMatches == matches ) {
                        jumpBy = time - lastSuccessfulTime;
                        searchKeyAt++;
                        if( searchKeyAt >= keys.Length ) {
                            finalTime = time;
                        }
                    }
                    nextMatches = matches;
                    lastSuccessfulTime = time;

                }
                time += jumpBy;
            }

            return finalTime.ToString();
        }
    }
}
