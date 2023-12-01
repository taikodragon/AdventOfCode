using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CD = AdventOfCode.Solutions.CompassDirection;
using Board = System.Collections.Generic.Dictionary<AdventOfCode.Solutions.Int2, System.Collections.Generic.List<AdventOfCode.Solutions.CompassDirection>>;
using Windows.Storage.BulkAccess;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 24, "Blizzard Basin")]
class Day24 : ASolution
{
    public Day24() : base(false) {
    }


    Board initialBlizzards = new();
    Dictionary<int, Board> blizzSets = new();
    Int2 start, end, max;
    int maxMinutes = 0;
    protected override void ParseInput() {
        Int2 xy = new(0, 1);
        
        var lines = Input.SplitByNewline();
        foreach (var line in lines.Skip(1).Take(lines.Count - 2)) {
            xy.X = 0;
            foreach(var c in line) {
                var dir = c switch {
                    '^' => CD.N,
                    'v' => CD.S,
                    '>' => CD.E,
                    '<' => CD.W,
                    '.' => CD.NW,
                    _ => CD.NE
                };
                if (dir == CD.NE) {
                    xy.X++;
                    continue;
                }
                initialBlizzards.Add(xy, new());
                if  (dir == CD.NW) {
                    xy.X++;
                    continue;
                }
                initialBlizzards[xy].Add(dir);
                xy.X++;
            }
            xy.Y++;
        }
        start = new(1, 0);
        end = new(lines[0].Length - 2, lines.Count - 1);
        max = xy - (2,1);
        initialBlizzards.Add(end, new());
        initialBlizzards.Add(start, new());

        HashSet<string> boardPrints = new() { BuildBoardPrint(start, initialBlizzards) };
        blizzSets.Add(0, initialBlizzards);
        int minute = 1;
        Board last = initialBlizzards;
        while(true) {
            var next = SimulateBlizz(last);
            string print = BuildBoardPrint(start, next);
            if (boardPrints.Contains(print)) break;
            boardPrints.Add(print);
            blizzSets.Add(minute, next);
            minute++;
            last = next;
        }
        maxMinutes = minute;
    }
    protected override object SolvePartOneRaw() {
        int minutes = 0;

        Int2 nextTeam = start;
        Board nextBlizz = initialBlizzards;
        Print(nextTeam, nextBlizz);

        while (nextTeam != end) {
            WriteLine($"== Minute {minutes + 1} == ");
            (nextTeam, _) = Simulate(nextTeam, minutes);
            Print(nextTeam, blizzSets[(minutes + 1) % maxMinutes]);
            minutes++;
        }

        return minutes;
    }
    protected override object SolvePartTwoRaw() { return null; }

    Board SimulateBlizz(Board fromState) {
        Board nextBlizz = new();
        foreach (var key in fromState.Keys) {
            nextBlizz[key] = new();
        }
        // Update blizzards
        foreach (var (pos, list) in fromState) {

            foreach (var dir in list) {
                var next = Offset(pos, dir);
                if (next.X < 1) next.X = max.X;
                if (next.X > max.X) next.X = 1;
                if (next.Y < 1) next.Y = max.Y;
                if (next.Y > max.Y) next.Y = 1;

                nextBlizz[next].Add(dir);
            }
        }
        return nextBlizz;
    }


    HashSet<(Int2 at, int setMinute)> state = new();
    CD[] movements = new CD[] { CD.N, CD.S, CD.E, CD.W };
    (Int2 newTeam, int dist) Simulate(Int2 team, int minute, int depth = 0) {
        int setMinute = (minute + 1) % maxMinutes;
        Board nextBlizz = blizzSets[setMinute];
        Int2? nextPos = null;
        int nextDist = int.MaxValue;
        foreach(var posDir in movements) {
            var next = Offset(team, posDir);
            if (nextBlizz.TryGetValue(next, out var list) && list.Count == 0 && !state.Contains((next, setMinute))) {
                int dist = Utilities.ManhattanDistance(next, end);
                if (depth < 2) {
                    // accumulate future distance too
                    var future = Simulate(next, minute + 1, depth + 1);
                    dist += future.dist;
                }

                if (dist < nextDist) {
                    nextDist = dist;
                    nextPos = next;
                }
            }
        }
        // Simulate a wait, when there won't be a blizzard there
        if (depth < 3 && blizzSets[(minute + 2) % maxMinutes][team].Count == 0) {
            // accumulate future distance too
            var future = Simulate(team, minute + 1, depth + 1);
            int futureDist = Utilities.ManhattanDistance(team, end) + future.dist;

            if( futureDist < nextDist ) {
                nextDist = futureDist;
                nextPos = null;
            }
        }


        // ensure we have a distance
        if (nextDist == int.MaxValue) { nextDist = Utilities.ManhattanDistance(team, end); }

        if( depth == 0 ) {
            if (nextPos is null) WriteLine($"Waiting this round");
            else WriteLine($"Moving to {nextPos} with dist {nextDist}");

            state.Add((nextPos ?? team, setMinute));
        }


        return (nextPos ?? team, nextDist);
    }



    void Print(Int2 team, Board blizzards) {
        if(!OutputAlways) { return; }
        WriteLine(BuildBoardPrint(team, blizzards));
    }

    string BuildBoardPrint(Int2 team, Board blizzards) {
        StringBuilder sb = new StringBuilder();
        Int2 xy = new(1, 1);
        for (; xy.Y <= max.Y; xy.Y++) {
            xy.X = 1;
            for (; xy.X <= max.X; xy.X++) {
                var list = blizzards[xy];
                if (xy == team) sb.Append('E');
                else if (list.Count == 1) {
                    char c = list[0] switch {
                        CD.N => '^',
                        CD.S => 'v',
                        CD.E => '>',
                        CD.W => '<',
                        _ => throw new Exception("unsupported direction")
                    };
                    sb.Append(c);
                }
                else if (list.Count > 1) {
                    if (list.Count < 10) sb.Append(list.Count.ToString());
                    else sb.Append('#');
                }
                else sb.Append('.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    static Int2 Offset(in Int2 position, in CD dir) {
        return dir switch {
            CD.N => position + Int2.North,
            CD.S => position + Int2.South,
            CD.W => position + Int2.West,
            CD.E => position + Int2.East,
            _ => throw new Exception("Unsupport direction")
        };
    }

}
