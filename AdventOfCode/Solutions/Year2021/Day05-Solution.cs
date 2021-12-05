using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{
    class Line
    {
        public bool IsStraight => Start.X == End.X || Start.Y == End.Y;
        public IntCoord Start { get; set; }
        public IntCoord End { get; set; }

        public List<IntCoord> Points { get; set; } = new List<IntCoord>();

        public void MakePointStraight() {
            Points.Clear();

            if( Start.X == End.X ) {
                int yMax = Math.Max(Start.Y, End.Y), yMin = Math.Min(Start.Y, End.Y);
                for(int y = yMin; y <= yMax; y++) {
                    Points.Add(new IntCoord(Start.X, y));
                }
            } else {
                int xMax = Math.Max(Start.X, End.X), xMin = Math.Min(Start.X, End.X);
                for (int x = xMin; x <= xMax; x++) {
                    Points.Add(new IntCoord(x, Start.Y));
                }
            }
        }
        public void MakePointDiagonal() {
            Points.Clear();

            int yWalk = Start.Y > End.Y ? -1 : 1;

            if (Start.X < End.X) {
                for (int x = Start.X, y = Start.Y; x <= End.X; x += 1, y += yWalk) {
                    Points.Add(new IntCoord(x, y));
                }
            } else {
                for (int x = Start.X, y = Start.Y; x >= End.X; x += -1, y += yWalk) {
                    Points.Add(new IntCoord(x, y));
                }
            }
        }
    }
    class Day05 : ASolution
    {
        List<Line> input;
        public Day05() : base(05, 2021, "", false)
        {



            input = Input.SplitByNewline()
                .Select(st => {
                    var coords = st.Split(" -> ");
                    var c1 = coords[0].Split(',').Select(int.Parse).ToArray();
                    var c2 = coords[1].Split(',').Select(int.Parse).ToArray();
                    var line =  new Line {
                        Start = new IntCoord(c1[0], c1[1]),
                        End = new IntCoord(c2[0], c2[1])
                    };
                    if (line.IsStraight) line.MakePointStraight();
                    else line.MakePointDiagonal();
                    return line;
                })
                .ToList();
        }

        protected override string SolvePartOne()
        {
            var startLines = input.Where(l => l.IsStraight).ToList();
            Dictionary<IntCoord, int> cloud = new Dictionary<IntCoord, int>();

            foreach(var line in startLines) {
                foreach (var pt in line.Points) {
                    if( cloud.ContainsKey(pt) ) {
                        cloud[pt]++;
                    } else {
                        cloud.Add(pt, 1);
                    }
                }
            }

            return cloud.Where(kv => kv.Value >= 2).Count().ToString();
        }

        protected override string SolvePartTwo()
        {
            Dictionary<IntCoord, int> cloud = new Dictionary<IntCoord, int>();

            foreach (var line in input) {
                foreach (var pt in line.Points) {
                    if (cloud.ContainsKey(pt)) {
                        cloud[pt]++;
                    }
                    else {
                        cloud.Add(pt, 1);
                    }
                }
            }





            //foreach (var kv in cloud.Where(kv => kv.Value >= 2).OrderBy(kv => kv.Key.X).ThenBy(kv => kv.Key.Y)) {
            //    Trace.WriteLine(kv.ToString());
            //}


            return cloud.Where(kv => kv.Value >= 2).Count().ToString();
        }

    }
}
