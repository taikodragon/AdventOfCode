using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Race = (int millis, int record);
namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 06, "Wait For It")]
class Day06 : ASolution
{

    public Day06() : base(false)
    {
            
    }

    List<Race> races = new();
    protected override void ParseInput()
    {
        var lines = Input.SplitByNewline(true, true)
            .Select(l => {
                return l.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Skip(1)
                    .Select(int.Parse)
                    .ToArray();
            })
            .ToList();
        var times = lines[0];
        var records = lines[1];
        for(int i = 0; i < times.Length; i++) {
            races.Add((times[i], records[i]));
        }
    }

    protected override object SolvePartOneRaw()
    {
        long score = 1;
        foreach(var race in races) {
            int wins = 0;
            for(int i = 0; i < race.millis; i++) {
                int dist = (race.millis - i) * i;
                if (dist > race.record) wins++;
            }
            score *= wins;
        }

        return score;
    }

    protected override object SolvePartTwoRaw()
    {
        var lines = Input.SplitByNewline(true, true);
        long millis = long.Parse(lines[0].Replace("Time:", string.Empty).Replace(" ", string.Empty));
        long record = long.Parse(lines[1].Replace("Distance:", string.Empty).Replace(" ", string.Empty));

        long winFirst = 0, winSecond = 0;
        for (long i = 0; i < millis && winFirst == 0; i++) {
            long dist = (millis - i) * i;
            if (dist > record) winFirst = i;
        }
        for(long i = millis - winFirst + 3; i > 0 && winSecond == 0; i--) {
            long dist = (millis - i) * i;
            if (dist > record) winSecond = i;
        }
        return winSecond - winFirst + 1;
    }
}
