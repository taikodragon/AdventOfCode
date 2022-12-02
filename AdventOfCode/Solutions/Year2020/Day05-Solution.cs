using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 05, "Binary Boarding")]
    class Day05 : ASolution
    {
        List<string> lines = new List<string>();
        public Day05() : base(false)
        {
            
            lines = Input.SplitByNewline();
        }

        int LocateSeat(string seatId)
        {
            seatId = seatId.ToUpperInvariant();
            int rowLower = 0, rowUpper = 127, colLower = 0, colUpper = 7;
            foreach(var c in seatId) {
                switch(c) {
                    case 'F':
                        rowUpper = (rowUpper - rowLower) / 2 + rowLower;
                        break;
                    case 'B':
                        rowLower = ((rowUpper - rowLower) / 2) + 1 + rowLower;
                        break;
                    case 'L':
                        colUpper = (colUpper - colLower) / 2 + colLower;
                        break;
                    case 'R':
                        colLower = ((colUpper - colLower) / 2) + 1 + colLower;
                        break;
                }
            }
            if( rowLower != rowUpper )
                throw new Exception("Search Failed Row");
            if( colLower != colUpper )
                throw new Exception("Seach Failed Col");
            return rowLower * 8 + colLower;
        }

        protected override string SolvePartOne()
        {
            int maxSeatId = 0;

            foreach(string line in lines) {
                maxSeatId = Math.Max(maxSeatId, LocateSeat(line));
            }

            return maxSeatId.ToString();
        }

        protected override string SolvePartTwo()
        {
            List<int> seatIds = new List<int>();

            foreach( string line in lines ) {
                seatIds.Add(LocateSeat(line));
            }
            seatIds.Sort();
            for(int i = 0; i < seatIds.Count - 1; i++) {
                if( seatIds[i + 1] - seatIds[i] == 2 )
                    return (seatIds[i] + 1).ToString();
            }

            return "Not found";
        }
    }
}
