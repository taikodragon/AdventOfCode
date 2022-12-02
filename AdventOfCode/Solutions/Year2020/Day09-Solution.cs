using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 09, "Encoding Error")]
    class Day09 : ASolution
    {
        List<long> numbers;
        int preambleLength;
        public Day09() : base(false)
        {
            
            preambleLength = UseDebugInput ? 5 : 25;

            numbers = Input.SplitByNewline().Select(s => long.Parse(s)).ToList();
        }

        protected override string SolvePartOne()
        {

            for(int i = preambleLength; i < numbers.Count; ++i) {
                bool found = false;
                for( int j = i - preambleLength; j <= i - 2 && !found; ++j) {
                    for( int k = j + 1; k <= i - 1 && !found; ++k ) {
                        if(numbers[j] + numbers[k] == numbers[i]) {
                            found = true;
                        }
                    }
                }
                if( !found ) {
                    return numbers[i].ToString();
                }
            }
            return "FAIL";
        }

        protected override string SolvePartTwo()
        {
            long selected = 0;
            for( int i = preambleLength; i < numbers.Count; ++i ) {
                bool found = false;
                for( int j = i - preambleLength; j <= i - 2 && !found; ++j ) {
                    for( int k = j + 1; k <= i - 1 && !found; ++k ) {
                        if( numbers[j] + numbers[k] == numbers[i] ) {
                            found = true;
                        }
                    }
                }
                if( !found ) {
                    selected = numbers[i];
                    break;
                }
            }
            if( selected == 0 )
                return "FAIL";

            for(int i = 0; i < numbers.Count; ++i) {
                long sum = numbers[i];
                int j = i + 1;
                for(; j < numbers.Count - 1 && sum < selected; ++j ) {
                    sum += numbers[j];
                }
                if( sum == selected ) {
                    var set = numbers.GetRange(i, j - i);
                    return (set.Min() + set.Max()).ToString();
                }
            }
            return "FAIL";


        }
    }
}
