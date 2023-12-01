using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 01, "")]
class Day01 : ASolution
{

    public Day01() : base(false)
    {
            
    }

    protected override void ParseInput()
    {

    }

    protected override object SolvePartOneRaw()
    {
        return Input.SplitByNewline(false, true)
            .Select(s => s.Where(char.IsDigit).ToArray())
            .Where(arr => arr.Length >= 2)
            .Select(arr => $"{arr.First()}{arr.Last()}")
            .Select(int.Parse)
            .Sum();
    }

    protected override object SolvePartTwoRaw()
    {
        string[] numbers = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
        int[] numberMap = [1, 2, 3, 4, 5, 6, 7, 8, 9];

        int sum = 0;
        foreach(string line in Input.SplitByNewline(false, true)) {
            List<int> lineNums = [];

            for(int i = 0; i < line.Length; i++) {
                if (char.IsDigit(line[i])) lineNums.Add(int.Parse(line[i].ToString()));
                else {
                    for(int np = 0; np < numbers.Length; np++) {
                        string num = numbers[np];
                        if (i + num.Length > line.Length) continue;
                        if ( num == line[i..(i+num.Length)] ) {
                            lineNums.Add(numberMap[np]);
                            break;
                        }
                    }
                }
            }
            sum += lineNums.First() * 10 + lineNums.Last();
        }
        return sum;
    }
}
