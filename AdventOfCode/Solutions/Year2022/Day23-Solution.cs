using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CD = AdventOfCode.Solutions.CompassDirection;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 23, "Unstable Diffusion")]
class Day23 : ASolution
{
    public Day23() : base(false) { }


    List<SimElf> elves = new();
    Dictionary<Int2, SimElf> elfPositions = new();
    protected override void ParseInput() {
        Int2 xy = new(0, 0);
        foreach(var line in Input.SplitByNewline()) {
            xy.X = 0;
            foreach(var c in line) {
                if( c == '#' ) {
                    _ = new SimElf(xy, elves, elfPositions);
                }

                xy.X++;
            }

            xy.Y++;
        }
    }
    protected override object SolvePartOneRaw() {
        Print();

        for(int round = 0; round < 10; round++) {
            SimulateRound();
            WriteLine(string.Concat("Round ", round + 1));
            Print();
        }

        var (min, max) = ComputeBounds();
        int yDelta = max.Y - min.Y + 1, xDelta = max.X - min.X + 1;
        int area = yDelta * xDelta;

        return area - elves.Count;
    }
    protected override object SolvePartTwoRaw() {
        Print();
        int round = 10;
        for (int moves = 1; moves > 0; round++) {
            moves = SimulateRound();
            WriteLine(string.Concat("Round ", round + 1));
            Print();
        }

        return round;
    }

    int SimulateRound() {
        // Consider new positions
        Dictionary<Int2, List<SimElf>> proposals = new(elves.Count);
        foreach(var elf in elves) {
            var nextMaybe = elf.ConsiderNext();
            if (nextMaybe is null) continue;

            if (proposals.ContainsKey(nextMaybe.Value)) {
                proposals[nextMaybe.Value].Add(elf);
            }
            else proposals.Add(nextMaybe.Value, new() { elf });
        }
        int moves = 0;
        // Move when there isn't a collision
        foreach(var (next, list) in proposals) {
            if (list.Count > 1) continue;
            moves++;
            list[0].Move(next);
        }
        // Rotate the directions the elves will consider
        foreach(var elf in elves) {
            elf.RotateConsider();
        }
        return moves;
    }

    (Int2 min, Int2 max) ComputeBounds() {
        Int2 min = new(int.MaxValue, int.MaxValue), max = new(int.MinValue, int.MinValue);

        foreach(Int2 pos in elfPositions.Keys) {
            min.X = Math.Min(min.X, pos.X);
            min.Y = Math.Min(min.Y, pos.Y);
            max.X = Math.Max(max.X, pos.X);
            max.Y = Math.Max(max.Y, pos.Y);
        }

        return (min, max);
    }

    void Print() {
        if (!OutputAlways) return;
        var (min, max) = ComputeBounds();
        int yDelta = max.Y - min.Y, xDelta = max.X - min.X;
        int area = yDelta * xDelta;
        StringBuilder sb = new(area + yDelta * 2);
        for(int y = min.Y - 1; y <= max.Y + 1; y++) {
            for(int x = min.X - 1; x <= max.X + 1; x++) {
                if(elfPositions.ContainsKey(new(x,y))) {
                    sb.Append('#');
                } else {
                    sb.Append('.');
                }
            }
            sb.AppendLine();
        }
        WriteLine(sb);
    }

    class SimElf {
        readonly List<SimElf> elves;
        readonly Dictionary<Int2, SimElf> elfPositions;

        Int2 position;
        readonly Queue<(CD main, CD diag1, CD diag2)> consider = new();

        public SimElf(in Int2 initialPosition, List<SimElf> elves, Dictionary<Int2, SimElf> elfPositions) {
            consider.Enqueue((CD.N, CD.NW, CD.NE));
            consider.Enqueue((CD.S, CD.SW, CD.SE));
            consider.Enqueue((CD.W, CD.NW, CD.SW));
            consider.Enqueue((CD.E, CD.NE, CD.SE));
            position = initialPosition;
            this.elves = elves;
            this.elfPositions = elfPositions;

            elves.Add(this);
            elfPositions.Add(position, this);
        }

        Int2 Offset(in CD dir) {
            return dir switch {
                CD.N => position + Int2.North,
                CD.S => position + Int2.South,
                CD.W => position + Int2.West,
                CD.E => position + Int2.East,
                CD.NE => position + Int2.NorthEast,
                CD.SE => position + Int2.SouthEast,
                CD.SW => position + Int2.SouthWest,
                CD.NW => position + Int2.NorthWest,
                _ => throw new Exception("Unsupport direction")
            };
        }
        public bool Move(Int2 next) {
            if(elfPositions.TryGetValue(next, out var other)) {
                if (other == this) throw new Exception("I'm in two locations!");
                return false;
            }
            elfPositions.Remove(position);
            elfPositions.Add(next, this);
            position = next;

            return true;
        }



        public Int2? ConsiderNext() {
            bool nearby = elfPositions.ContainsKey(Offset(CD.N));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.NE));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.E));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.SE));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.S));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.SW));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.W));
            nearby = nearby || elfPositions.ContainsKey(Offset(CD.NW));
            if( !nearby ) return null;

            Int2? next = null;
            for(int i = 0; i < consider.Count; i++) {
                var (dir, diag1, diag2) = RotateConsider();
                if(next is null) {
                    if (elfPositions.ContainsKey(Offset(dir)) ||
                        elfPositions.ContainsKey(Offset(diag1)) ||
                        elfPositions.ContainsKey(Offset(diag2))) {
                        // do nothing
                    }
                    else next = Offset(dir);
                }
            }
            return next;
        }

        public (CD main, CD diag1, CD diag2) RotateConsider() {
            var result = consider.Dequeue();
            consider.Enqueue(result);
            return result;
        }
    }

}
