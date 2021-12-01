using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day17 : ASolution
    {

        public Day17() : base(17, 2015, "No Such Thing as Too Much", false)
        {
        }

        protected override string SolvePartOne()
        {
            List<int> containers;
            int sumTo;

            if( UseDebugInput ) {
                containers = new List<int> { 20, 15, 10, 5, 5 };
                sumTo = 25;
            } else {
                containers = Input.SplitByNewline().Select(int.Parse).ToList();
                sumTo = 150;
            }
            int len = containers.Count;

            int count = 0;
            int[] idxes = Enumerable.Range(0, len).ToArray();
            List<int> perm = new List<int>(len);
            for( int i = 0; i < len; i++ ) {
                for( int j = i+1; j < len; j++ ) {
                    perm.Clear();
                    perm.Add(idxes[i]);
                    for( int k = j; k < len; k++ ) {
                        perm.Add(idxes[(k) % len]);
                        Trace.WriteLine(string.Join(", ", perm));
                        if( perm.Select(pi => containers[pi]).Sum() == sumTo ) count++;
                    }

                }
            }

            return count.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
