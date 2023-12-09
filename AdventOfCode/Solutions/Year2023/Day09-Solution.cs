using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 09, "Mirage Maintenance")]
class Day09 : ASolution
{

    public Day09() : base(false)
    {
            
    }

    List<List<int>> input = new();
    long p1Total, p2Total;
    protected override void ParseInput()
    {
        foreach(var origin in Input.SplitByNewline(false, true)
            .Select(l => l.Split(" ").Select(int.Parse).ToList()) ) {

            List<List<int>> resolve = [origin];
            while (!resolve[^1].TrueForAll(n => n == 0)) {
                var fromSet = resolve[^1];
                // Create a new generation
                List<int> newGen = new();

                for (int i = 0; i < fromSet.Count - 1; i++) {
                    newGen.Add(fromSet[i + 1] - fromSet[i]);
                }
                resolve.Add(newGen);
            }
            // Now predict
            for (int i = resolve.Count - 1; i >= 0; i--) {
                if (i == resolve.Count - 1) {
                    resolve[i].Add(0);
                    resolve[i].Insert(0, 0);
                }
                else {
                    var at = resolve[i];
                    var prev = resolve[i + 1];
                    at.Add(at[^1] + prev[^1]);
                    at.Insert(0, (at[0] - prev[0]));
                }
            }

            WriteLine(resolve[0]);
            p1Total += resolve[0][^1];
            p2Total += resolve[0][0];

        }
    }

    protected override object SolvePartOneRaw()
    {
        return p1Total;
    }

    protected override object SolvePartTwoRaw()
    {
        return p2Total;
    }
}
