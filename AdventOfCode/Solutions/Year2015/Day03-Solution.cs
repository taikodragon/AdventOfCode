using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day03 : ASolution
    {
        

        public Day03() : base(03, 2015, "Perfectly Spherical Houses in a Vacuum")
        {
            UseDebugInput = false;
        }

        protected override string SolvePartOne()
        {
            Dictionary<IntCoord, int> deliveries = new Dictionary<IntCoord, int>() {
                { new IntCoord(0, 0), 1 }
            };
            IntCoord at = new IntCoord(0, 0);
            foreach(char c in Input) {
                switch(c) {
                    case '^': at.North++; break;
                    case 'v': at.North--; break;
                    case '>': at.East++; break;
                    case '<': at.East--; break;
                }
                var atClone = new IntCoord(at.X, at.Y);
                if( deliveries.TryGetValue(at, out int deliveryCount) ) {
                    deliveries[atClone] = deliveryCount + 1;
                } else {
                    deliveries[atClone] = 1;
                }
            }
            return deliveries.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            Dictionary<IntCoord, int> deliveries = new Dictionary<IntCoord, int>() {
                { new IntCoord(0, 0), 2 }
            };
            int idx = 0;
            IntCoord santa = new IntCoord(0, 0), robosanta = new IntCoord(0, 0);
            foreach( char c in Input ) {
                IntCoord at = (idx % 2) == 0 ? santa : robosanta;
                idx++;
                switch( c ) {
                    case '^': at.North++; break;
                    case 'v': at.North--; break;
                    case '>': at.East++; break;
                    case '<': at.East--; break;
                }
                var atClone = new IntCoord(at.X, at.Y);
                if( deliveries.TryGetValue(at, out int deliveryCount) ) {
                    deliveries[atClone] = deliveryCount + 1;
                }
                else {
                    deliveries[atClone] = 1;
                }
            }
            return deliveries.Count.ToString();
        }
    }
}
