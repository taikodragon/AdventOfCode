using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    [DayInfo(2021, 11, "Dumbo Octopus")]
    class Day11 : ASolution
    {
        IntCoord[] adjacent = new IntCoord[] {
            new IntCoord(1, -1),
            new IntCoord(1, 0),
            new IntCoord(1, 1),
            new IntCoord(0, 1),
            new IntCoord(-1, 1),
            new IntCoord(-1, 0),
            new IntCoord(-1, -1),
            new IntCoord(0, -1)
        };

        public Day11() : base(false)
        {


        }

        (Dictionary<IntCoord, int>, int maxX, int maxY) Parse() {
            Dictionary<IntCoord, int> octos = new(100);
            var lines = Input.SplitByNewline();
            int x = 0, y = 0;
            for (x = 0; x < lines.Count; x++) {
                var line = lines[x];
                for (y = 0; y < line.Length; y++) {
                    octos[new IntCoord(x, y)] = int.Parse(line[y].ToString());
                }
            }
            return (octos, x, y);
        }

        protected override string SolvePartOne()
        {
            int flashes = 0;
            var (octos, maxX, maxY) = Parse();
            HashSet<IntCoord> hasFlashed = new(octos.Count);
            Queue<IntCoord> didChange = new(octos.Count);

            void TryFlash(IntCoord at) {
                if( octos[at] > 9 && !hasFlashed.Contains(at) ) {
                    flashes++;
                    hasFlashed.Add(at);
                    octos[at] = 0;
                    foreach(var alt in adjacent) {
                        var atalt = at + alt;
                        if (octos.ContainsKey(atalt) && !hasFlashed.Contains(atalt)) {
                            octos[atalt]++;
                            didChange.Enqueue(atalt);
                        }
                    }
                }
            }

            Print(-1, octos, hasFlashed, maxX, maxY);

            for (int step = 0; step < 100; step++) {
                hasFlashed.Clear();
                for (int x = 0; x < maxX; x++) {
                    for (int y = 0; y < maxY; y++) {
                        IntCoord at = new IntCoord(x, y);
                        octos[at]++;
                        TryFlash(at);
                    }
                }

                while (didChange.Count > 0) {
                    var at = didChange.Dequeue();
                    TryFlash(at);
                }

                Print(step, octos, hasFlashed, maxX, maxY);
            }
            return flashes.ToString();
        }

        void Print(int step, Dictionary<IntCoord, int> octos, HashSet<IntCoord> flashed, int maxX, int maxY) {
            if (!UseDebugInput) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"After step {step + 1}");
            for (int x = 0; x < maxX; x++) {
                for (int y = 0; y < maxY; y++) {
                    IntCoord at = new IntCoord(x, y);
                    sb.Append(' ').Append(octos[at]);
                }
                sb.Append(" -- ");
                for (int y = 0; y < maxY; y++) {
                    IntCoord at = new IntCoord(x, y);
                    if (flashed.Contains(at)) { sb.Append('#'); }
                    else { sb.Append('-');  }
                }
                sb.AppendLine();
            }
            Trace.WriteLine(sb);
        }
        protected override string SolvePartTwo()
        {
            int flashes = 0;
            var (octos, maxX, maxY) = Parse();
            HashSet<IntCoord> hasFlashed = new(octos.Count);
            Queue<IntCoord> didChange = new(octos.Count);

            void TryFlash(IntCoord at) {
                if (octos[at] > 9 && !hasFlashed.Contains(at)) {
                    flashes++;
                    hasFlashed.Add(at);
                    octos[at] = 0;
                    foreach (var alt in adjacent) {
                        var atalt = at + alt;
                        if (octos.ContainsKey(atalt) && !hasFlashed.Contains(atalt)) {
                            octos[atalt]++;
                            didChange.Enqueue(atalt);
                        }
                    }
                }
            }

            Print(-1, octos, hasFlashed, maxX, maxY);
            int step = 0;
            for (; hasFlashed.Count < octos.Count; step++) {
                hasFlashed.Clear();
                for (int x = 0; x < maxX; x++) {
                    for (int y = 0; y < maxY; y++) {
                        IntCoord at = new IntCoord(x, y);
                        octos[at]++;
                        TryFlash(at);
                    }
                }

                while (didChange.Count > 0) {
                    var at = didChange.Dequeue();
                    TryFlash(at);
                }

                Print(step, octos, hasFlashed, maxX, maxY);
            }
            return step.ToString();
        }
    }
}
