using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Services.Maps;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 08, "")]
class Day08 : ASolution
{

    public Day08() : base(false)
    {

    }

    string dirs;
    Dictionary<string, (string L, string R)> map = new();
    protected override void ParseInput()
    {
        var lines = Input.SplitByNewline(true, true);
        dirs = lines[0];

        foreach(var line in lines[2..]) {
            var parts = line.TrimEnd(')').Split(new string[] { " = (", ", " }, StringSplitOptions.None);
            map.Add(parts[0], (parts[1], parts[2]));
        }
    }

    protected override object SolvePartOneRaw()
    {
        return null;
        int steps = 0;
        int i = 0;
        string at = "AAA";
        while(at != "ZZZ") {
            steps++;
            char dir = dirs[i];
            i = (i + 1) % dirs.Length;

            if (dir == 'L')
                at = map[at].L;
            else
                at = map[at].R;
        }

        return steps;
    }

    protected override object SolvePartTwoRaw()
    {
        return SolvePart2Fast();
        long steps = 0;
        int i = 0;
        List<string> ats = map.Keys.Where(s => s[2] == 'A').ToList();
        //List<List<string>> history = new(ats.Count);
        //history.AddRange(ats.Select(a => new List<string> { a }));
        //List<string> starts = new(ats);



        while (!ats.TrueForAll(s => s[2] == 'Z')) {
            steps++;
            char dir = dirs[i];
            i = (i + 1) % dirs.Length;

            for(int j = ats.Count - 1; j >= 0; j--) {
                if (dir == 'L')
                    ats[j] = map[ats[j]].L;
                else
                    ats[j] = map[ats[j]].R;
            }
        }

        return steps;
    }

    object SolvePart2Fast() {
        long steps = 0;
        int i = 0;
        List<string> ats = map.Keys.Where(s => s[2] == 'A').ToList();
        List<long> history = new(ats.Count);
        history.AddRange(ats.Select(a => 0L));
        List<(long period, long start)> periods = new(ats.Count);
        periods.AddRange(ats.Select(a => (0L, 0L)));

        while (true) {
            char dir = dirs[i];

            bool didCompute = false;
            for (int j = ats.Count - 1; j >= 0; j--) {
                if (periods[j].period == 0) didCompute = true; // we found the period so no need to compute
                if (ats[j][2] == 'Z') {
                    if(history[j] == 0) {
                        history[j] = steps;
                    }
                    else if(periods[j].period == 0) {
                        periods[j] = (steps - history[j], history[j]);
                    }
                }
                // move next
                if (dir == 'L')
                    ats[j] = map[ats[j]].L;
                else
                    ats[j] = map[ats[j]].R;
            }

            if (!didCompute) break;

            steps++;
            i = (i + 1) % dirs.Length;
        }

        long start = 0, sPeriod = 0;
        for(i = 0; i < periods.Count; i++) {
            if(periods[i].period > sPeriod ) {
                start = periods[i].start;
                sPeriod = periods[i].period;
            }
        }
        steps = start;
        while( !periods.TrueForAll( (p) => ((steps - p.start) % p.period) == 0)) {
            steps += sPeriod;
        }

        return steps;
    }
}
