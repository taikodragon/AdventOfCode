using AdventOfCode.Solutions.Year2021;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 11, "")]
class Day11 : ASolution
{

    public Day11() : base(false)
    {
            
    }


    List<Int2> galaxies = new();
    HashSet<int> emptyRows = new(), emptyCols = new();
    protected override void ParseInput()
    {
        var lines = Input.SplitByNewline();
        HashSet<int> galaxyX = new();
        for(int i = lines.Count - 1; i > -1; i--) {
            bool foundOne = false;
            for(int x = lines[i].Length - 1; x > -1; x--) {
                if(lines[i][x] == '#') {
                    galaxyX.Add(x);
                    foundOne = true;
                    galaxies.Add((x, i));
                }
            }
            if( !foundOne ) {
                emptyRows.Add(i);
            }
        }
        for(int x = lines[0].Length - 1; x > -1; x--) {
            if (galaxyX.Contains(x)) continue;
            else emptyCols.Add(x);
        }
    }

    List<Int2> Expand(int magnitude) {
        magnitude--; // we already have one row of each empty
        List<Int2> result = new(galaxies);
        foreach(var col in emptyCols.OrderDescending()) {
            for(int i = result.Count - 1; i > -1; i--) {
                var at = result[i];
                if (at.X >= col) {
                    at.X += magnitude;
                    result[i] = at;
                }
            }
        }
        foreach(var row in emptyRows.OrderDescending()) {
            for (int i = result.Count - 1; i > -1; i--) {
                var at = result[i];
                if (at.Y >= row) {
                    at.Y += magnitude;
                    result[i] = at;
                }
            }
        }
        return result;
    }

    void Print(List<Int2> galaxies) {
        Int2 min = galaxies[0], max = galaxies[0];
        foreach(var at in galaxies) {
            min.X = Math.Min(min.X, at.X);
            min.Y = Math.Min(min.Y, at.Y);
            max.X = Math.Max(max.X, at.X);
            max.Y = Math.Max(max.Y, at.Y);
        }
        HashSet<Int2> known = new(galaxies);
        StringBuilder sb = new();
        for(Int2 at = min; at.Y <= max.Y; at.Y++) {
            for(at.X = min.X; at.X <= max.X; at.X++) {
                if (known.Contains(at)) sb.Append('#');
                else sb.Append('.');
            }
            sb.AppendLine();
        }
        WriteLine(sb);
    }

    protected override object SolvePartOneRaw()
    {
        var myGalaxies = Expand(2);
        Dictionary<(int a, int b), int> seen = new();
        (int a, int b) MakeKey(int a, int b) {
            return (Math.Min(a, b), Math.Max(a, b));
        }

        for (int a = 0; a < myGalaxies.Count; a++) {
            for (int b = 0; b < myGalaxies.Count; b++) {
                if (a == b) continue;
                var key = MakeKey(a, b);
                // check for duplicate
                if (seen.ContainsKey(key)) {
                    continue;
                }
                seen[key] = Int2.ManhattanDistance(myGalaxies[a], myGalaxies[b]);
            }
        }

        WriteLine(seen.Count);
        return seen.Values.Sum();
    }

    protected override object SolvePartTwoRaw()
    {
        var myGalaxies = Expand(1_000_000);
        Dictionary<(int a, int b), long> seen = new();
        (int a, int b) MakeKey(int a, int b) {
            return (Math.Min(a, b), Math.Max(a, b));
        }

        for (int a = 0; a < myGalaxies.Count; a++) {
            for (int b = 0; b < myGalaxies.Count; b++) {
                if (a == b) continue;
                var key = MakeKey(a, b);
                // check for duplicate
                if (seen.ContainsKey(key)) {
                    continue;
                }
                seen[key] = Int2.ManhattanDistance(myGalaxies[a], myGalaxies[b]);
            }
        }

        return seen.Values.Sum();
    }
}
