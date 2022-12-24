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
        public Int2 Start { get; set; }
        public Int2 End { get; set; }

        public List<Int2> Points { get; set; } = new List<Int2>();

        public void MakePointStraight() {
            Points.Clear();

            if( Start.X == End.X ) {
                int yMax = Math.Max(Start.Y, End.Y), yMin = Math.Min(Start.Y, End.Y);
                for(int y = yMin; y <= yMax; y++) {
                    Points.Add(new Int2(Start.X, y));
                }
            } else {
                int xMax = Math.Max(Start.X, End.X), xMin = Math.Min(Start.X, End.X);
                for (int x = xMin; x <= xMax; x++) {
                    Points.Add(new Int2(x, Start.Y));
                }
            }
        }
        public void MakePointDiagonal() {
            Points.Clear();

            int yWalk = Start.Y > End.Y ? -1 : 1;

            if (Start.X < End.X) {
                for (int x = Start.X, y = Start.Y; x <= End.X; x += 1, y += yWalk) {
                    Points.Add(new Int2(x, y));
                }
            } else {
                for (int x = Start.X, y = Start.Y; x >= End.X; x += -1, y += yWalk) {
                    Points.Add(new Int2(x, y));
                }
            }
        }
    }
    [DayInfo(2021, 05, "Hydrothermal Venture")]
    class Day05 : ASolution
    {
        List<Line> input;
        Dictionary<Int2, int> cloud = new Dictionary<Int2, int>();

        public Day05() : base(false)
        {



            input = Input.SplitByNewline()
                .Select(st => {
                    var coords = st.Split(new string[] { " -> ", "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray();
                    var line =  new Line {
                        Start = new Int2(coords[0], coords[1]),
                        End = new Int2(coords[2], coords[3])
                    };
                    if (line.IsStraight) line.MakePointStraight();
                    else line.MakePointDiagonal();
                    return line;
                })
                .ToList();
        }

        protected override string SolvePartOne()
        {
            foreach(var line in input) {
                if (!line.IsStraight) continue;
                foreach (var pt in line.Points) {
                    if( cloud.ContainsKey(pt) ) {
                        cloud[pt]++;
                    } else {
                        cloud.Add(pt, 1);
                    }
                }
            }

            return cloud.Count(kv => kv.Value >= 2).ToString();
        }

        protected override string SolvePartTwo()
        {
            if (cloud.Count == 0) SolvePartOne();

            foreach (var line in input) {
                if (line.IsStraight) continue;
                foreach (var pt in line.Points) {
                    if (cloud.ContainsKey(pt)) {
                        cloud[pt]++;
                    }
                    else {
                        cloud.Add(pt, 1);
                    }
                }
            }

            return cloud.Count(kv => kv.Value >= 2).ToString();
        }

    }
}
