using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using CD = AdventOfCode.Solutions.CompassDirection;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 22, "Monkey Map")]
class Day22 : ASolution
{
    const int Left = -1, Right = -2;

    public Day22() : base(false)
    {
        if (UseDebugInput) {
            edgeLength = 4;
            // Edge - 1 West -- 3 North
            edgePairs.Add((new(8, 5), CD.W, new(9, 4), CD.N));
            // Edge - 1 North -- 2 North
            edgePairs.Add((new(4, 5), CD.W, new(9, 1), CD.E));
            // Edge - 1 East -- 6 East
            edgePairs.Add((new(12, 4), CD.N, new(16, 9), CD.S));
            // Edge - 4 East -- 6 North
            edgePairs.Add((new(12, 8), CD.N, new(13, 9), CD.E));
            // Edge - 2 West -- 6 South
            edgePairs.Add((new(1, 5), CD.S, new(16, 12), CD.W));
            // Edge - 3 South -- 5 West
            edgePairs.Add((new(8, 8), CD.W, new(9, 9), CD.S));
            // Edge - 2 South -- 5 South
            edgePairs.Add((new(4, 8), CD.W, new(9, 12), CD.E));
        }
        else {
            edgeLength = 50;

            // Edge - 1 West -- 3 South
            edgePairs.Add((new(101, 50), CD.E, new(100, 51), CD.S));
            // Edge - 1 East -- 5 North
            edgePairs.Add((new(50, 101), CD.W, new(51, 100), CD.N));
            // Edge - 2 West -- 5 West
            edgePairs.Add((new(1, 101), CD.S, new(51, 50), CD.N));
            // Edge - 2 North -- 6 West
            edgePairs.Add((new(1, 151), CD.S, new(51, 1), CD.E));
            // Edge - 3 East -- 4 East
            edgePairs.Add((new(150, 50), CD.N, new(100, 101), CD.S));
            // Edge - 4 South -- 6 East
            edgePairs.Add((new(51,150), CD.E, new(50, 151), CD.S));
            // Edge - 3 North -- 6 South
            edgePairs.Add((new(1, 200), CD.E, new(101, 1), CD.E));


            //    faces.Add((new(51, 51), new(100, 100), 1));
            //    faces.Add((new(51, 1), new(100, 50), 2));
            //    faces.Add((new(101, 1), new(150, 50), 3));
            //    faces.Add((new(51, 101), new(100, 150), 4));
            //    faces.Add((new(1, 101), new(50, 150), 5));
            //    faces.Add((new(1, 151), new(50, 200), 6));
        }
    }

    HashSet<Int2> walls = new();
    Dictionary<int, (int min, int max)> rangeX = new();
    Dictionary<int, (int min, int max)> rangeY = new();

    List<int> instructions = new();
    Int2 initial, at;
    CD facing = CD.N;

    //List<(Int2 min, Int2 max, int faceId)> faces = new();
    //Dictionary<(int faceId, CD dir), (Func<Int2, Int2> tx, CD facing)> faceTransform = new();
    Dictionary<(Int2 from, CD dir), (Int2 to, CD dir)> edgeMap = new();
    int edgeLength;
    List<(Int2 start1, CD walk1, Int2 start2, CD walk2)> edgePairs = new();

    protected override void ParseInput() {
        Int2 xy = new(1, 1);
        bool collectingBoard = true;
        foreach(var line in Input.SplitByNewline(true, false)) {
            if( line == string.Empty) {
                collectingBoard = false;
                continue;
            }
            if (collectingBoard) {
                xy.X = 1;
                int min = -1, max = -1;
                foreach(var c in line) {
                    if(c == '.' || c == '#') {
                        if (min == -1) min = xy.X;
                        if (max < xy.X) max = xy.X;

                        if(c == '#') {
                            walls.Add(xy);
                        }

                        // push out Y max for a given X
                        if (rangeY.TryGetValue(xy.X, out var minMax)) {
                            rangeY[xy.X] = (minMax.min, Math.Max(minMax.max, xy.Y));
                        }
                        else {
                            rangeY[xy.X] = (xy.Y, xy.Y);
                        }
                    }
                    xy.X++;
                }
                if (min == -1 || max == 0) throw new Exception("impossible board");
                rangeX.Add(xy.Y, (min, max));
            }
            else {
                string num = string.Empty;
                foreach(char c in line) {
                    if( char.IsNumber(c) ) {
                        num += c;
                    } else if( char.IsLetter(c) ) {
                        instructions.Add(int.Parse(num));
                        num = string.Empty;
                        if (c == 'L') instructions.Add(Left);
                        else if (c == 'R') instructions.Add(Right);
                    }
                }
                if (num.Length > 0) {
                    instructions.Add(int.Parse(num));
                }
            }
            xy.Y++;
        }

        initial = new(rangeX[1].min, 1);

        foreach(var set in edgePairs) {
            WalkEdge(set.start1, set.walk1, set.start2, set.walk2);
        }



    }
    protected override object SolvePartOneRaw()
    {
        at = initial;
        facing = CD.E;

        foreach(int num in instructions) {
            if (num > 0) Move(num);
            else if (num == Left) facing = RotateLeft(facing);
            else if (num == Right) facing = RotateRight(facing);
        }

        int facingScore = facing switch {
            CD.E => 0,
            CD.S => 1,
            CD.W => 2,
            CD.N => 3,
            _ => throw new Exception("unknown facing direction")
        };

        return 1000 * at.Y + 4 * at.X + facingScore;
    }

    Dictionary<Int2, char> headings = new();
    protected override object SolvePartTwoRaw()
    {
        at = initial;
        facing = CD.E;

        foreach (int num in instructions) {
            if (num > 0) MoveP2(num);
            else if (num == Left) facing = RotateLeft(facing);
            else if (num == Right) facing = RotateRight(facing);
        }

        int facingScore = facing switch {
            CD.E => 0,
            CD.S => 1,
            CD.W => 2,
            CD.N => 3,
            _ => throw new Exception("unknown facing direction")
        };

        return 1000 * at.Y + 4 * at.X + facingScore;
    }

    void Move(int count) {
        Int2 last = at;
        for(; count > 0; count--) {
            var next = Offset(last, facing);

            if(facing == CD.N || facing == CD.S) {
                // loop in the Y direction
                var range = rangeY[next.X];
                if (next.Y > range.max)  next.Y = range.min;
                else if (next.Y < range.min) next.Y = range.max;
            }
            else {
                // loop in the X direction
                var range = rangeX[next.Y];
                if (next.X < range.min) next.X = range.max;
                else if (next.X > range.max) next.X = range.min;

            }

            if( walls.Contains(next) ) {
                break;
            }

            headings[last] = facing switch {
                CD.N => '^',
                CD.E => '>',
                CD.S => 'V',
                CD.W => '<',
                _ => throw new Exception("unknown heading")
            };
            last = next;
        }
        at = last;
    }

    void MoveP2(int count) {
        Int2 last = at;
        for (; count > 0; count--) {
            var next = Offset(last, facing);

            if (!IsDefined(next)) {
                var nextPair = edgeMap[(last, facing)];
                next = nextPair.to;
                if (walls.Contains(next)) {
                    break;
                }
                facing = nextPair.dir;
            }
            else if (walls.Contains(next)) {
                break;
            }
            last = next;
        }
        at = last;
    }

    void WalkEdge(Int2 start1, CD dir1, Int2 start2, CD dir2) {
        Int2 at1 = start1, at2 = start2;

        CD inward1 = IsDefined(Offset(at1, RotateLeft(dir1))) ? RotateLeft(dir1) : RotateRight(dir1);
        CD inward2 = IsDefined(Offset(at2, RotateLeft(dir2))) ? RotateLeft(dir2) : RotateRight(dir2);
        CD outward1 = RotateLeft(RotateLeft(inward1));
        CD outward2 = RotateLeft(RotateLeft(inward2));

        for(int count = edgeLength; count > 0; count--) {
            // cache out the edge map and coordinates
            edgeMap.Add((at1, outward1), (at2, inward2));
            edgeMap.Add((at2, outward2), (at1, inward1));

            at1 = Offset(at1, dir1);
            at2 = Offset(at2, dir2);
        }
    }
    bool IsDefined(Int2 pt) {
        if( rangeY.TryGetValue(pt.X, out var yMinMax) ) {
            if(pt.Y >= yMinMax.min && pt.Y <= yMinMax.max) {
                if(rangeX.TryGetValue(pt.Y, out var xMinMax) ) {
                    return pt.X >= xMinMax.min && pt.X <= xMinMax.max;
                }
            }
        }
        return false;
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

    static CD RotateRight(in CD dir) {
        return dir switch {
            CD.N => CD.E,
            CD.E => CD.S,
            CD.S => CD.W,
            CD.W => CD.N,
            _ => throw new Exception("Unsupport direction")
        };
    }

    static CD RotateLeft(in CD dir) {
        return dir switch {
            CD.N => CD.W,
            CD.E => CD.N,
            CD.S => CD.E,
            CD.W => CD.S,
            _ => throw new Exception("Unsupport direction")
        };
    }

}
