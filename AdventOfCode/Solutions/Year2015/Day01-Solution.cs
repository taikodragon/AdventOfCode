using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2015, "Not Quite Lisp", false)
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
