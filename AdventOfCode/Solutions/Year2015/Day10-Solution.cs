using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 10, "Elves Look, Elves Say")]
    class Day10 : ASolution
    {

        public Day10() : base(false)
        {
            
        }

        StringBuilder RunInterations(int liveGenCap) {
            int i = 0, gen = 0, genCap = UseDebugInput ? 5 : liveGenCap;
            StringBuilder bld = new StringBuilder(10485760), next = new StringBuilder(10485760);
            next.Append(UseDebugInput ? "1" : Input);

            for( ; gen < genCap; gen++ ) {
                bld.Clear();
                char cc = next[0];
                int cnt = 1;
                for( i = 1; i < next.Length; i++ ) {
                    if( next[i] == cc ) { cnt++; }
                    else {
                        bld.Append(cnt);
                        bld.Append(cc);
                        cc = next[i];
                        cnt = 1;
                    }
                }
                bld.Append(cnt);
                bld.Append(cc);

                if( UseDebugInput ) Trace.WriteLine($"{next} => {bld}");
                var tmp = next;
                next = bld;
                bld = tmp;
            }
            return next;
        }
        protected override string SolvePartOne()
        {
            return RunInterations(40).Length.ToString();
        }

        protected override string SolvePartTwo()
        {
            return RunInterations(50).Length.ToString();
        }
    }
}
