using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2020, "Report Repair")
        {
            UseDebugInput = false;
        }

        protected override string SolvePartOne()
        {
            string input = Input;

            int[] numbers = input.ToIntArray("\n");
            int result = 0;
            for(int i = 0; i < numbers.Length; ++i ) {
                for( int j = i+1; j < numbers.Length; ++j ) {
                    if( numbers[i] + numbers[j] == 2020 ) {
                        result = numbers[i] * numbers[j];
                        break;
                    }

                }
            }

            return result.ToString();
        }

        protected override string SolvePartTwo()
        {
            string input = Input;

            var numbers = new List<int>(input.ToIntArray("\n"));
            numbers.Sort();

            int result = 0, iv, jv, kv;
            for( int i = 0; i < numbers.Count && result == 0; ++i ) {
                iv = numbers[i];
                for( int j = numbers.Count - 1; i < j && result == 0; --j ) {
                    jv = numbers[j];
                    for( int k = i + 1; k < j && result == 0; ++k ) {
                        kv = numbers[k];
                        if( iv + jv + kv == 2020 ) {
                            result = iv * jv * kv;
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
