using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Media.Playback;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 10, "")]
class Day10 : ASolution
{
    class PipeSegment {
        public CompassDirection[] ExitDirs;
        public Int2[] Exits = new Int2[2];
        public Int2 Position = Int2.Zero;
        public PipeSegment[] PipeExits = new PipeSegment[2];
        public char Symbol;

        public PipeSegment(char symbol, Int2 pos)
        {
            Position = pos;
            Symbol = symbol;
            ExitDirs = SymbolToExits(symbol);
            Exits[0] = Int2.Offset(pos, ExitDirs[0]);
            Exits[1] = Int2.Offset(pos, ExitDirs[1]);
        }


        public static CompassDirection[] SymbolToExits(char symbol) {
            return symbol switch {
                '|' => [CompassDirection.N, CompassDirection.S],
                '-' => [CompassDirection.E, CompassDirection.W],
                'L' => [CompassDirection.N, CompassDirection.E],
                'J' => [CompassDirection.N, CompassDirection.W],
                '7' => [CompassDirection.S, CompassDirection.W],
                'F' => [CompassDirection.S, CompassDirection.E],
                _ => throw new Exception("Unknown pipe: " + symbol)

            };
        }
        public static CompassDirection[] SymbolToSides(char symbol, CompassDirection heading) {
            // ALways take right hand of heading
            switch(symbol) {
                case '|':
                    return heading switch {
                        CompassDirection.N => [CompassDirection.E],
                        CompassDirection.S => [CompassDirection.W],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                case '-':
                    return heading switch {
                        CompassDirection.E => [CompassDirection.S],
                        CompassDirection.W => [CompassDirection.N],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                case 'L':
                    return heading switch {
                        CompassDirection.S => [CompassDirection.W, CompassDirection.SW, CompassDirection.S],
                        CompassDirection.W => [CompassDirection.NE],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                case 'J':
                    return heading switch {
                        CompassDirection.S => [CompassDirection.NW],
                        CompassDirection.E => [CompassDirection.S, CompassDirection.SE, CompassDirection.E],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                case '7':
                    return heading switch {
                        CompassDirection.N => [CompassDirection.E, CompassDirection.NE, CompassDirection.N],
                        CompassDirection.E => [CompassDirection.SW],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                case 'F':
                    return heading switch {
                        CompassDirection.N => [CompassDirection.SE],
                        CompassDirection.W => [CompassDirection.N, CompassDirection.NW, CompassDirection.W],
                        _ => throw new Exception($"symbol {symbol} heading {heading} undefined")
                    };
                default: throw new Exception("Unknown symbol: " + symbol);
            }
        }
        public static char ExitsToSymbol(CompassDirection[] exits) {
            bool Has(CompassDirection dir1, CompassDirection dir2) {
                return (exits[0] == dir1 && exits[1] == dir2) || (exits[1] == dir1 && exits[0] == dir2);
            }

            if (Has(CompassDirection.N, CompassDirection.S))
                return '|';
            if (Has(CompassDirection.E, CompassDirection.W))
                return '-';
            if (Has(CompassDirection.N, CompassDirection.E))
                return 'L';
            if (Has(CompassDirection.N, CompassDirection.W))
                return 'J';
            if (Has(CompassDirection.S, CompassDirection.W))
                return '7';
            if (Has(CompassDirection.S, CompassDirection.E))
                return 'F';
            throw new Exception($"Uknown exit set: {exits[0]} {exits[1]}");
        }
    }


    public Day10() : base(false)
    {
        OutputAlways = true;
    }

    Int2 start, max;
    HashSet<Int2> ground = new();
    Dictionary<Int2, PipeSegment> map = new();
    protected override void ParseInput()
    {
        Int2 at = Int2.Zero;
        void UpdateMax() { max.X = Math.Max(max.X, at.X); max.Y = Math.Max(max.Y, at.Y); }
        foreach(var line in Input.SplitByNewline()) {
            at.X = 0;
            foreach(char c in line) {
                if (c == '.') { ground.Add(at); }
                else if (c == 'S') { start = at; }
                else { map[at] = new PipeSegment(c, at); }
                UpdateMax();
                at.X++;
            }
            at.Y++;
        }
        if (start == Int2.Zero) throw new Exception("No start location");

        List<CompassDirection> startNeighbors = new();
        foreach (var nbr in new Int2[] { start + Int2.North, start + Int2.South, start + Int2.East, start + Int2.West}) {
            if (map.TryGetValue(nbr, out var nbrPipe)) {
                var exits = nbrPipe.Exits;
                if (exits[0] == start) {
                    startNeighbors.Add(nbrPipe.ExitDirs[0].CompassRight90().CompassRight90());
                } else if (exits[1] == start) {
                    startNeighbors.Add(nbrPipe.ExitDirs[1].CompassRight90().CompassRight90());
                }
            }
        }
        char sym = PipeSegment.ExitsToSymbol(startNeighbors.ToArray());
        map.Add(start, new PipeSegment(sym, start));

        List<Int2> removals = new();
        // Wire up pipes
        foreach(var kv in map) {
            var pipe = kv.Value;

            bool WireValid(int i) {
                if (pipe.PipeExits[i] is not null) return true;
                var matching = map.GetValueOrDefault(pipe.Exits[i], null);
                if (matching is null) return false;
                if (matching.Exits[0] == pipe.Position) {
                    matching.PipeExits[0] = pipe;
                    pipe.PipeExits[i] = matching;
                    return true;
                }
                else if (matching.Exits[1] == pipe.Position) {
                    matching.PipeExits[1] = pipe;
                    pipe.PipeExits[i] = matching;
                    return true;
                }
                else return false;
            }

            if (WireValid(0) && WireValid(1)) { }
            else {
                // bad pipes might as well be ground
                ground.Add(pipe.Position);
                removals.Add(pipe.Position);
            }
        }

        removals.ForEach(p => map.Remove(p));
    }

    protected override object SolvePartOneRaw()
    {
        int maxSteps = 0;
        HashSet<Int2> seen = new();
        Queue<(PipeSegment pipe, int steps)> pending = new();
        pending.Enqueue((map[start], 0));

        while (pending.Count > 0) {
            var (pipe, step) = pending.Dequeue();

            if (!seen.Contains(pipe.Exits[0])) {
                seen.Add(pipe.Exits[0]);
                pending.Enqueue((pipe.PipeExits[0], step + 1));
            }
            if (!seen.Contains(pipe.Exits[1])) {
                seen.Add(pipe.Exits[1]);
                pending.Enqueue((pipe.PipeExits[1], step + 1));
            }
            maxSteps = Math.Max(maxSteps, step);
        }

        loopOnly = seen;
        return maxSteps;
    }

    HashSet<Int2> loopOnly;
    protected override object SolvePartTwoRaw()
    {
        PipeSegment startPipe = map[start];
        var walkDirs = Utilities.CardinalDirections;

        HashSet<Int2> sideTiles = new();
        foreach(CompassDirection startHeading in startPipe.ExitDirs) {
            bool isOutside = false;
            sideTiles.Clear();
            // Get initial side tiles

            CompassDirection[] sides = null;
            switch (startPipe.Symbol) {
                case '|':
                case '-':
                    sides = PipeSegment.SymbolToSides(startPipe.Symbol, startHeading);
                    break;
                case 'L':
                case 'J':
                case '7':
                case 'F':
                    sides = PipeSegment.SymbolToSides(startPipe.Symbol, startPipe.ExitDirs.First(d => d != startHeading).Compass180());
                    break;
                default: throw new Exception("Unknown symbol: " + startPipe.Symbol);
            }
            foreach(var d in sides) { sideTiles.Add(Int2.Offset(startPipe.Position, d)); }

            PipeSegment at = map[Int2.Offset(startPipe.Position, startHeading)];
            CompassDirection heading = startHeading;

            while (at.Position != start) {
                // Process me
                sides = PipeSegment.SymbolToSides(at.Symbol, heading);
                foreach (var d in sides) { sideTiles.Add(Int2.Offset(at.Position, d)); }

                // Move next
                var nextHeading = at.ExitDirs[0] == heading.Compass180() ? at.ExitDirs[1] : at.ExitDirs[0];
                heading = nextHeading;
                at = map[Int2.Offset(at.Position, nextHeading)];
            }

            sideTiles.ExceptWith(loopOnly);
            HashSet<Int2> sideTilesTouched = new();
            List<HashSet<Int2>> fills = new();
            
            Queue<Int2> fillQueue = new();

            foreach(var tile in sideTiles) {
                if (sideTilesTouched.Contains(tile)) continue; // don't refill when I already saw this tile
                HashSet<Int2> fillEach = new();
                fills.Add(fillEach);
                fillQueue.Clear();
                fillQueue.Enqueue(tile);
                fillEach.Add(tile);

                while(fillQueue.Count > 0) {
                    var atTile = fillQueue.Dequeue();
                    // this fill hits outside, so this path hits outside
                    isOutside = atTile.X < 0 || atTile.Y < 0 || atTile.X > max.X || atTile.Y > max.Y;
                    if( isOutside ) break;

                    if (sideTiles.Contains(atTile)) sideTilesTouched.Add(atTile);

                    foreach(var d in Utilities.CardinalDirections) {
                        var nbr = Int2.Offset(atTile, d);
                        // only fill into non-loop tiles
                        if (!loopOnly.Contains(nbr) && !fillEach.Contains(nbr)) {
                            fillQueue.Enqueue(nbr);
                            fillEach.Add(nbr);
                        }
                    }
                }
                if (isOutside) break;
            }
            if (isOutside) continue;

            return fills.Sum(fe => fe.Count);
        }

        return "Broken";
    }
}
