using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day06 : ASolution
    {
        List<string> lines = new List<string>();

        public Day06() : base(06, 2020, "Custom Customs")
        {
            UseDebugInput = false;

            lines = Input.SplitByNewline();
        }

        protected override string SolvePartOne()
        {
            List<string> groups = new List<string>();
            string thisGroup = string.Empty;
            foreach(string ga in lines) {
                if( ga == string.Empty ) {
                    groups.Add(thisGroup);
                    thisGroup = string.Empty;
                    continue;
                }
                foreach( char a in ga ) {
                    if( !thisGroup.Contains(a) ) {
                        thisGroup = string.Concat(thisGroup, a);
                    }
                }
            }
            if(thisGroup != string.Empty) {
                groups.Add(thisGroup);
            }
            return groups.Select(s => s.Length).Sum().ToString();
        }

        protected override string SolvePartTwo()
        {
            List<string> groups = new List<string>();
            string thisGroup = null;
            foreach( string ga in lines ) {
                if( ga == string.Empty ) {
                    groups.Add(thisGroup);
                    thisGroup = null;
                    continue;
                }
                if( thisGroup == null ) thisGroup = ga;
                else thisGroup = string.Concat(thisGroup.Where(c => ga.Contains(c)));
            }
            if( thisGroup != string.Empty ) {
                groups.Add(thisGroup);
            }
            return groups.Select(s => s.Length).Sum().ToString();
        }
    }
}
