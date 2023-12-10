using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 10, "")]
class Day10 : ASolution
{
    class PipeSegment {
        public CompassDirection[] ExitDirs = new CompassDirection[2];
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

    Int2 start;
    HashSet<Int2> ground = new();
    Dictionary<Int2, PipeSegment> map = new();
    protected override void ParseInput()
    {
        Int2 at = Int2.Zero;
        foreach(var line in Input.SplitByNewline()) {
            at.X = 0;
            foreach(char c in line) {
                if (c == '.') { ground.Add(at); }
                else if (c == 'S') { start = at; }
                else { map[at] = new PipeSegment(c, at); }
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
        HashSet<Int2> groundBoth = new(ground.Count * 4);
        HashSet<Int2> mapDouble = new(map.Count * 2);
        Int2 max = (-1, -1);
        void UpdateMax(Int2 cMax) {
            max.X = Math.Max(max.X, cMax.X);
            max.Y = Math.Max(max.Y, cMax.Y);
        }
        foreach(var pipe in map.Values) {
            if (!loopOnly.Contains(pipe.Position)) continue;
            var dPos = pipe.Position * 2;
            UpdateMax(dPos);
            // Add self
            mapDouble.Add(dPos);
            // add halfsteps
            for(int i = 0; i < 2; i++) {
                var halfPos = Int2.Offset(dPos, pipe.ExitDirs[i]);
                UpdateMax(halfPos);
                mapDouble.Add(halfPos);
            }
        }
        foreach(var g in ground) {
            groundBoth.Add(g * 2);
            UpdateMax(g * 2);
        }
        // fill in half grounds
        for(Int2 at = Int2.Zero; at.Y <= max.Y; at.Y++) {
            for(at.X = 0; at.X <= max.X; at.X++) {
                if (!mapDouble.Contains(at)) {
                    groundBoth.Add(at);
                    UpdateMax(at);
                }
            }
        }


        HashSet<Int2> seenGround = new();
        HashSet<Int2> touchOutside = new();
        Dictionary<(Int2, CompassDirection), Int2?> terminations = new();

        Int2? WalkToTerminal(Int2 start, CompassDirection dir) {
            if (terminations.TryGetValue((start, dir), out var term)) return term;

            if (touchOutside.Contains(start)) return null;
            else if( mapDouble.Contains(start) ) {
                return start;
            }
            else if( groundBoth.Contains(start) ) {
                var result = WalkToTerminal(Int2.Offset(start, dir), dir);
                terminations.Add((start, dir), result);
                return result;
            } else { // not pipe or ground, so it outside
                return null;
            }
        }

        var walkDirs = new CompassDirection[] { CompassDirection.N, CompassDirection.E, CompassDirection.S, CompassDirection.W };

        int outsideGroundCount = -1, gens = 0;
        while(touchOutside.Count != outsideGroundCount) {
            // copy first so changes are detectable
            outsideGroundCount = touchOutside.Count;
            foreach (var at in groundBoth) {
                if (touchOutside.Contains(at)) continue;
                foreach (var dir in walkDirs) {
                    Int2? result = null;
                    result = WalkToTerminal(at, dir);
                    if (result is null) { touchOutside.Add(at); break; }
                }
            }
            terminations.Clear();
            gens++;

            if (gens <= 2) {
                StringBuilder sb = new(max.X);
                for (Int2 at = Int2.Zero; at.Y <= max.Y; at.Y += 1) {
                    sb.Clear();
                    for (at.X = 0; at.X <= max.X; at.X += 1) {
                        if (touchOutside.Contains(at)) {
                            sb.Append('O');
                        }
                        else if (groundBoth.Contains(at)) {
                            if (at.X % 2 == 1 || at.Y % 2 == 1)
                                sb.Append(',');
                            else
                                sb.Append('.');
                        }
                        else if (mapDouble.Contains(at)) sb.Append('#'); //sb.Append(map[(at.X/2, at.Y/2)].Symbol);
                        else sb.Append('!');
                    }
                    WriteLine(sb);
                }
                WriteLine($"gens: {gens}");
            }
        }

        var inside = ground.Select(g => g * 2).Where(at => !touchOutside.Contains(at)).OrderBy(at => at.X).ThenBy(at => at.Y).ToList();

        //StringBuilder sb = new(max.X);
        //for (Int2 at = Int2.Zero; at.Y <= max.Y; at.Y += 1) {
        //    sb.Clear();
        //    for (at.X = 0; at.X <= max.X; at.X += 1) {
        //        if (inside.Contains(at)) sb.Append('I');
        //        else if (touchOutside.Contains(at)) sb.Append('~');
        //        else if (groundBoth.Contains(at)) sb.Append('.');
        //        else if (mapDouble.Contains(at)) sb.Append('#');
        //        else sb.Append('!');
        //    }
        //    WriteLine(sb);
        //}
        //WriteLine($"gens: {gens}");
        return inside.Count;
    }
}
