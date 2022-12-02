using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 01, "Not Quite Lisp")]
    class Day01 : ASolution
    {

        public Day01() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            return (Input.Count(c => c == '(') - Input.Count(c => c == ')')).ToString();
        }

        protected override string SolvePartTwo()
        {
            int idx = 0, atFloor = 0;
            foreach(char c in Input) {
                idx++;
                switch( c ) {
                    case '(': atFloor++; break;
                    case ')': atFloor--; break; 
                }
                if( atFloor == -1 ) return idx.ToString();
            }
            return "FAIL";
        }
    }
}
