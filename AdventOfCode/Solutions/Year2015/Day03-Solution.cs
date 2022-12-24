using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 03, "Perfectly Spherical Houses in a Vacuum")]
    class Day03 : ASolution
    {
        

        public Day03() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            Dictionary<Int2, int> deliveries = new Dictionary<Int2, int>() {
                { new Int2(0, 0), 1 }
            };
            Int2 at = new Int2(0, 0);
            foreach(char c in Input) {
                switch(c) {
                    case '^': at.Y++; break;
                    case 'v': at.Y--; break;
                    case '>': at.X++; break;
                    case '<': at.X--; break;
                }
                var atClone = new Int2(at.X, at.Y);
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
            Dictionary<Int2, int> deliveries = new Dictionary<Int2, int>() {
                { new Int2(0, 0), 2 }
            };
            int idx = 0;
            Int2 santa = new Int2(0, 0), robosanta = new Int2(0, 0);
            foreach( char c in Input ) {
                Int2 at = (idx % 2) == 0 ? santa : robosanta;
                idx++;
                switch( c ) {
                    case '^': at.Y++; break;
                    case 'v': at.Y--; break;
                    case '>': at.X++; break;
                    case '<': at.X--; break;
                }
                var atClone = new Int2(at.X, at.Y);
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
