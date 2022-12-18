using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022
{


    [DayInfo(2022, 18, "Boiling Boulders")]
    class Day18 : ASolution
    {

        public Day18() : base(false)
        {
            
        }

        HashSet<Int3> points = new();
        protected override void ParseInput() {
            foreach(var pt in Input.SplitByNewline(false, true)
                .Select(l => {
                    var pts = l.Split(',').Select(int.Parse).ToArray();
                    return new Int3(pts);
                })) {
                points.Add(pt);
            }
        }

        protected override object SolvePartOneRaw()
        {
            Int3[] neighborDeltas = new Int3[] {
                new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(0, 0, -1), new(0, -1, 0), new(-1, 0, 0)
            };

            int surface = 0;
            foreach(var pt in points) {
                surface += neighborDeltas.Count(d => !points.Contains(pt + d));
            }
            return surface;
        }

        protected override object SolvePartTwoRaw()
        {
            Int3[] neighborDeltas = new Int3[] {
                new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(0, 0, -1), new(0, -1, 0), new(-1, 0, 0)
            };

            Int3 min = points.First(), max = points.First();
            foreach(var pt in points) {
                Int3 pto = pt;
                Int3.Min(ref min, ref pto, out min);
                Int3.Max(ref max, ref pto, out max);
            }
            min -= Int3.One;
            max += Int3.One;

            List<List<Int3>> empties = new();
            int surface = 0;
            foreach (var pt in points) {
                int mySurface = 0;
                foreach(var delta in neighborDeltas) {
                    if (points.Contains(pt + delta)) continue; // don't point if this face is next to a block

                    Int3 walkEnd = pt;
                    if (delta.X > 0) walkEnd.X = max.X + 1;
                    else if (delta.X < 0) walkEnd.X = min.X;
                    else if (delta.Y > 0) walkEnd.Y = max.Y;
                    else if (delta.Y < 0) walkEnd.Y = min.Y;
                    else if (delta.Z > 0) walkEnd.Z = max.Z;
                    else if (delta.Z < 0) walkEnd.Z = min.Z;
                    Int3 walker = pt + delta;
                    List<Int3> emptyLocal = new();
                    while(walker != walkEnd) {
                        if( !points.Contains(walker) ) { emptyLocal.Add(walker); }
                        else { empties.Add(emptyLocal); break; } // we reached the other side
                        walker += delta;
                    }
                    mySurface++;
                }
                surface += mySurface;
            }

            List<HashSet<Int3>> searchedCavity = new();
            foreach (var cavity in empties) {
                HashSet<Int3> visited = new();
                HashSet<Int3> visitedEmpties = new();
                Queue<Int3> search = new();
                foreach (var c in cavity) { search.Enqueue(c); visited.Add(c); }
                bool isEnclosed = true;
                while (search.Count > 0) {
                    var at = search.Dequeue();
                    if (at.X == min.X || at.X == max.X || at.Y == min.Y || at.Y == max.Y || at.Z == min.Z || at.Z == max.Z) {
                        isEnclosed = false; break;
                    }
                    if (points.Contains(at)) continue;
                    visitedEmpties.Add(at);
                    foreach (var pt in neighborDeltas.Select(d => at + d)) {
                        if (!visited.Contains(pt)) {
                            search.Enqueue(pt);
                            visited.Add(pt);
                        }
                    }
                }
                if (isEnclosed) {
                    if (!searchedCavity.Any(c => c.SetEquals(visitedEmpties))) {
                        searchedCavity.Add(visitedEmpties);
                    }
                }
            }
            int trappedSurface = searchedCavity.Sum(v => v.Sum(pt => neighborDeltas.Count(d => !v.Contains(pt + d))));
            return surface - trappedSurface;
        }

        //public static bool IsOpposingProjectionCube(ArmorFace other) {
        //    if (other == null) return false;
        //    if (other.Normal != -Normal) return false;
        //    if (other.Vertices.Length != Vertices.Length) return false;
        //    Int3 normal = DimensionUtil.SnapToInt3(Normal);
        //    Int3 xMask = new(0, 1, 1), yMask = new(1, 0, 1), zMask = new(1, 1, 0);
        //    Int3 mask;
        //    if (normal.X > 0) mask = xMask;
        //    else if (normal.Y > 0) mask = yMask;
        //    else mask = zMask;

        //    for (int i = orderedVertices.Length - 1; i > -1; --i) {
        //        if (Int3Multiply(Vertices[orderedVertices[i]], mask) != Int3Multiply(other.Vertices[other.orderedVertices[i]], mask)) return false;
        //    }
        //    return true;
        //}

    }
}
