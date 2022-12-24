using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 14, "Regolith Reservoir")]
class Day14 : ASolution
{
    const char Rock = '#', Sand = 'o';
    public Day14() : base(false)
    {
    }

    Int2 min, max, spawn = (500,0);
    int killY = 0, floorY = 0;
    Dictionary<Int2, char> scan = new();
    Int2[] dirs = new Int2[] { Int2.Down, Int2.Down + Int2.Left, Int2.Down + Int2.Right };
    protected override void ParseInput() {
        scan.Clear();
        foreach(var line in Input.SplitByNewline(false, true)) {
            var points = line.Split(" -> ")
                .Select(c => {
                    var xy = c.Split(',');
                    return (int.Parse(xy[0]), int.Parse(xy[1]));
                })
                .ToList();

            for(int i = 0; i < points.Count - 1; i++) {
                Int2 start = points[i], end = points[i + 1];
                int xDelta = 0, yDelta = 0;
                if (start.X > end.X) xDelta = -1;
                else if( start.X < end.X) xDelta = 1;
                if (start.Y > end.Y) yDelta = -1;
                else if( start.Y < end.Y) yDelta = 1;

                Int2 at = start, delta = (xDelta, yDelta);
                while( at != end ) {
                    scan[at] = Rock;
                    at += delta;
                }
                scan[end] = Rock;
            }
        }

        int yMin = int.MaxValue; int yMax = int.MinValue;
        int xMin = int.MaxValue; int xMax = int.MinValue;
        foreach (var (pt, type) in scan) {
            if (pt.Y < yMin) yMin = pt.Y;
            if (pt.Y > yMax) yMax = pt.Y;
            if (pt.X < xMin) xMin = pt.X;
            if (pt.X > xMax) xMax = pt.X;
        }
        min = (xMin, yMin);
        max = (xMax, yMax);
        killY = yMax + 1;
        floorY = yMax + 2;
    }
    protected override object SolvePartOneRaw()
    {
        Print();

        bool didKill = false;
        while(!didKill) {
            Int2 falling = spawn;
            bool didMove;
            do {
                didMove = false;
                foreach (var dir in dirs) {
                    if (!scan.ContainsKey(falling + dir)) {
                        didMove = true;
                        falling += dir;
                        break;
                    }
                }
                if( falling.Y == killY ) {
                    didKill = true;
                    break;
                }

            } while (didMove);

            if(!didKill) {
                scan[falling] = Sand;
            }
            Print();
        }
        return scan.Count(kv => kv.Value == Sand);
    }

    protected override object SolvePartTwoRaw()
    {
        while (true) {
            if (scan.ContainsKey(spawn)) break;
            Int2 falling = spawn;
            bool didMove;
            do {
                didMove = false;
                foreach (var dir in dirs) {
                    if (!scan.ContainsKey(falling + dir)) {
                        if ((falling + dir).Y == floorY) break; // can't fall through floor
                        didMove = true;
                        falling += dir;
                        break;
                    }
                }

            } while (didMove);

            scan[falling] = Sand;
            Print();
        }
        return scan.Count(kv => kv.Value == Sand);
    }

    void Print() {
        if (!(UseDebugInput || OutputAlways)) return;
        int yMin = int.MaxValue; int yMax = int.MinValue;
        int xMin = int.MaxValue; int xMax = int.MinValue;
        foreach(var (pt,type) in scan) {
            if( pt.Y < yMin ) yMin = pt.Y;
            if( pt.Y > yMax ) yMax = pt.Y;
            if( pt.X < xMin ) xMin = pt.X;
            if( pt.X > xMax ) xMax = pt.X;
        }
        if (yMax < floorY) yMax = floorY;


        StringBuilder sb = new StringBuilder((yMax - yMin) * (xMax - xMin));
        for(int y = yMin; y <= yMax; y++) {
            for(int x = xMin; x <= xMax; x++) {
                var value = scan.GetValueOrDefault((x, y));
                if (y == floorY) value = Rock;
                sb.Append( value == default ? '.' : value.ToString() );
            }
            sb.AppendLine();
        }
        WriteLine(sb);
    }
}
