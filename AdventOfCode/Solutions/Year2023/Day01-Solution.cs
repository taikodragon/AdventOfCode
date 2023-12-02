using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 01, "Trebuchet?!")]
class Day01 : ASolution
{

    public Day01() : base(false)
    {
            
    }

    List<string> lines;
    protected override void ParseInput()
    {
        lines = Input.SplitByNewline(false, true);
    }

    protected override object SolvePartOneRaw()
    {
        return lines
            .Select(s => s.Where(char.IsDigit).ToArray())
            .Where(arr => arr.Length >= 1)
            .Select(arr => ((arr[0] - '0') * 10) + (arr[^1] - '0'))
            .Sum();
    }

    protected override object SolvePartTwoRaw()
    {
        string[] numbers = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

        int sum = 0;
        foreach(string line in lines) {
            List<int> lineNums = [];

            for(int i = 0; i < line.Length; i++) {
                if (char.IsDigit(line[i])) lineNums.Add(line[i] - '0'); // '1' - '0' = 1 because '0' is 48 and '1' is 49
                else {
                    for(int np = 0; np < numbers.Length; np++) {
                        string num = numbers[np];
                        if ( line[i..].StartsWith(num) ) {
                            lineNums.Add(np + 1); // shift index to one base
                            i += num.Length - 2; // jump over the content of the matched number
                            break;
                        }
                    }
                }
            }
            sum += lineNums[0] * 10 + lineNums[^1];
        }
        return sum;
    }
}
