using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.Devices.SmartCards;
using Windows.Security.EnterpriseData;

namespace AdventOfCode.Solutions.Year2022;



[DayInfo(2022, 17, "")]
class Day17 : ASolution
{
    const int xMin = 0, xMax = 8, yMax = 0;

    public Day17() : base(true)
    {
        materializeCache = new IntCoord[][] {
            new IntCoord[pieces[0].Length],
            new IntCoord[pieces[1].Length],
            new IntCoord[pieces[2].Length],
            new IntCoord[pieces[3].Length],
            new IntCoord[pieces[4].Length],
        };
    }
    IntCoord[][] pieces = new IntCoord[][] {
        new IntCoord[]{ new(0,0), new(1, 0), new(2, 0), new(3, 0) }, // Straight
        new IntCoord[]{ new(1,0), new(1, -1), new(0, -1), new(2, -1), new(1, -2)}, // Cross
        new IntCoord[]{ new(0,0), new(1, 0), new(2, 0), new(2, -1), new(2, -2)}, // L-Shape
        new IntCoord[]{ new(0,0), new(0, -1), new(0, -2), new(0, -3)}, // Vertical Straight
        new IntCoord[]{ new(0,0), new(1, 0), new(0, -1), new(1, -1)} // Square
    };
    IntCoord[][] materializeCache;

    protected override object SolvePartOneRaw() {
        return RunSim(2022, false);
    }
    protected override object SolvePartTwoRaw() {
        //return RunSim(1_000_000_000_000, false);
        return RunSim(2_000_000, true);
    }



    protected object RunSim(long iterations, bool shouldPrint)
    {
        OutputAlways = shouldPrint;

        string jets = Input.Trim();

        int jetsLen = jets.Length;
        int jetsAt = 0, nextPiece = 0, yMin = 0;

        IntCoord left = IntCoord.Left, right = IntCoord.Right;
        Dictionary<int, int> rightHandExterma = new(pieces.Select((p,i) => new KeyValuePair<int, int>(i, p.Max(xy => xy.X))));

        List<long> marks = new();
        HashSet<IntCoord> shifted = new(300);
        HashSet<IntCoord> placed = new(300);
        bool falling;
        long rocksPlaced = 0, height = 0;
        Stopwatch sw = new(), mk = new();
        while(rocksPlaced < iterations) {
            sw.Restart();
            int pieceIdx = nextPiece;
            nextPiece = (nextPiece + 1) % pieces.Length;
            int rightSideDelta = rightHandExterma[pieceIdx];

            IntCoord pos = (3, yMin - 4);
            falling = true;
            while(falling) {
                // jet time
                var jetDir = jets[jetsAt] switch {
                    '>' => right,
                    '<' => left,
                    _ => throw new Exception("unknown jet direction")
                };
                jetsAt = (jetsAt + 1) % jets.Length;

                var nextPos = pos + jetDir;
                if( nextPos.X > xMin && nextPos.X + rightSideDelta <  xMax ) {
                    IntCoord[] jetCoords = MaterializeShape(nextPos, pieceIdx);
                    if (!jetCoords.Any(c => placed.Contains(c))) {
                        pos = nextPos;
                    }
                }

                //Print(placed, pos, shape, yMin);

                IntCoord[] matCoords = MaterializeShape(pos + IntCoord.Down, pieceIdx);
                bool overlap = false;
                for(int i = matCoords.Length - 1; i > -1 && !overlap; i--) {
                    overlap = placed.Contains(matCoords[i]) || matCoords[i].Y == yMax;
                }
                if (overlap) {
                    foreach (var c in MaterializeShape(pos, pieceIdx)) {
                        placed.Add(c);
                        if (c.Y < yMin) {
                            yMin = c.Y;
                        }
                    }
                    falling = false;

                    if (yMin < -75) {
                        //Print(placed, pos, pieceIdx, yMin);
                        WriteLine($"Attemping to shift board... (cnt: {placed.Count}");
                        mk.Restart();
                        int newYMax = yMin;
                        IntCoord xy = (xMin + 1, 0);
                        for (; xy.X < xMax; xy.X++) {
                            xy.Y = yMin;
                            for (; xy.Y < yMax; xy.Y++) {
                                if (placed.Contains(xy)) {
                                    break;
                                }
                            }
                            if (xy.Y > newYMax) {
                                newYMax = xy.Y;
                            }
                        }
                        sw.Stop();
                        WriteLine(string.Concat("newYMax", sw.ElapsedTicks));
                        if (newYMax < yMax - 1) {
                            sw.Restart();
                            newYMax++;
                            shifted.Clear();
                            foreach (var c in placed) {
                                if (c.Y < newYMax) {
                                    shifted.Add(new(c.X, c.Y - newYMax));
                                }
                            }
                            var tmpShifted = placed;
                            placed = shifted;
                            shifted = tmpShifted;
                            yMin -= newYMax;
                            height += newYMax;

                            sw.Stop();
                            WriteLine($"Shifted board by {newYMax} (cnt {placed.Count}) shift: {sw.ElapsedTicks}");
                            //Print(placed, pos, pieceIdx, yMin);
                            //Thread.Sleep(1500);
                        }
                    }
                }
                else pos += IntCoord.Down;

            }
            rocksPlaced++;
            sw.Stop();
            WriteLine($"Placed {rocksPlaced} with height {height + yMin} and time {sw.ElapsedTicks:G}");
            if (rocksPlaced % 1_000_000 == 0) Console.WriteLine($"{rocksPlaced} placed");
            //Thread.Sleep(500);
        }

        return Math.Abs(height + yMin);
    }


    IntCoord[] MaterializeShape(in IntCoord pos, int shapeIdx) {
        var final = materializeCache[shapeIdx];
        var shape = pieces[shapeIdx];
        for(int i = shape.Length - 1; i > -1; --i) {
            final[i] = pos + shape[i];
        }
        return final;
    }
    void Print(HashSet<IntCoord> placed, IntCoord pos, int shapeIdx, int yMin) {
        if (!OutputAlways) return;
        yMin -= 6;
        HashSet<IntCoord> pieceTranslated = new(MaterializeShape(pos, shapeIdx));
        StringBuilder sb = new();
        IntCoord at = new(0, 0);
        int yPrintMax = Math.Min(yMax, yMin + 20);
        for(int y = yMin; y <= yPrintMax; y++) {
            at.Y = y;
            for(int x = xMin; x <= xMax; x++) {
                at.X = x;
                if (x == xMin || x == xMax) {
                    if (y == yMax)
                        sb.Append('+');
                    else
                        sb.Append('|');
                }
                else if (pieceTranslated.Contains(at)) sb.Append('@');
                else if (placed.Contains(at)) sb.Append('#');
                else if (y == yMax) sb.Append('-');
                else sb.Append('.');
            }
            sb.AppendLine(y.ToString());
        }
        WriteLine(sb);
    }
}
