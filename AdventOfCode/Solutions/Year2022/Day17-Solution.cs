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



[DayInfo(2022, 17, "Pyroclastic Flow")]
class Day17 : ASolution
{
    const int xMin = 0, xMax = 8, yMax = 0;

    public Day17() : base(true)
    {
        materializeCache = new Int2[][] {
            new Int2[pieces[0].Length],
            new Int2[pieces[1].Length],
            new Int2[pieces[2].Length],
            new Int2[pieces[3].Length],
            new Int2[pieces[4].Length],
        };
    }
    Int2[][] pieces = new Int2[][] {
        new Int2[]{ new(0,0), new(1, 0), new(2, 0), new(3, 0) }, // Straight
        new Int2[]{ new(1,0), new(1, -1), new(0, -1), new(2, -1), new(1, -2)}, // Cross
        new Int2[]{ new(0,0), new(1, 0), new(2, 0), new(2, -1), new(2, -2)}, // L-Shape
        new Int2[]{ new(0,0), new(0, -1), new(0, -2), new(0, -3)}, // Vertical Straight
        new Int2[]{ new(0,0), new(1, 0), new(1, -1), new(0, -1)} // Square
    };
    Int2[][] materializeCache;

    protected override object SolvePartOneRaw() {
        return RunSim(2022, false);
    }
    protected override object SolvePartTwoRaw() {
        //return RunSim(1_000_000_000_000, false);
        return RunSim(1_000_000, true);
    }



    protected object RunSim(long iterations, bool shouldPrint)
    {
        OutputAlways = shouldPrint;

        Int2 left = Int2.Left, right = Int2.Right;
        Int2[] jetDirs = Input.Trim().Select(c => c switch {
                '>' => right,
                '<' => left,
                _ => throw new Exception("unknown jet direction")
            })
            .ToArray();
        int jetsLen = jetDirs.Length;
        int jetsAt = 0, yMin = 0;

        int[] rightHandExtrema = pieces.Select((p) => p.Max(xy => xy.X)).ToArray();

        int[] heights = new int[7];
        HashSet<Int2> shifted = new(300);
        HashSet<Int2> placed = new(300);
        bool falling;
        long rocksPlaced = 0, height = 0;
        Stopwatch sw = new(), mk = new();

        while(rocksPlaced < iterations) {
            int pieceIdx = (int)(rocksPlaced % pieces.Length);

            Int2 pos = (3, yMin - 4);
            falling = true;
            while(falling) {
                // jet time
                var jetDir = jetDirs[jetsAt];
                jetsAt = (jetsAt + 1) % jetDirs.Length;

                var nextPos = pos + jetDir;
                bool overlap = false;
                if( nextPos.X > xMin && nextPos.X + rightHandExtrema[pieceIdx] <  xMax ) {
                    Int2[] jetCoords = MaterializeShape(nextPos, pieceIdx);
                    for (int i = jetCoords.Length - 1; i > -1 && !overlap; i--) {
                        overlap = placed.Contains(jetCoords[i]);
                    }
                    if (!overlap) pos = nextPos;
                }

                //Print(placed, pos, shape, yMin);

                nextPos = pos + Int2.Down;
                Int2[] matCoords = MaterializeShape(nextPos, pieceIdx);
                overlap = nextPos.Y == yMax;
                for(int i = matCoords.Length - 1; i > -1 && !overlap; i--) {
                    overlap = placed.Contains(matCoords[i]);
                }
                if (overlap) {
                    foreach (var c in MaterializeShape(pos, pieceIdx)) {
                        placed.Add(c);
                        if (c.Y < yMin) {
                            yMin = c.Y;
                        }
                        if (c.Y < heights[c.X - 1]) {
                            heights[c.X - 1] = c.Y;
                        }
                    }
                    falling = false;
                }
                else pos = nextPos;

            }

            // Do cycle detection here
            // Use has for 'heights' for cycle detection


            rocksPlaced++;



            //WriteLine($"Placed {rocksPlaced} with height {height + yMin} and time {sw.ElapsedTicks:G}");
            if (rocksPlaced % 1_000_000 == 0) Console.WriteLine($"{rocksPlaced} placed");
            
        }

        return Math.Abs(height + yMin);
    }


    Int2[] MaterializeShape(in Int2 pos, int shapeIdx) {
        var final = materializeCache[shapeIdx];
        var shape = pieces[shapeIdx];
        for(int i = shape.Length - 1; i > -1; --i) {
            final[i] = pos + shape[i];
        }
        return final;
    }
    void Print(HashSet<Int2> placed, Int2 pos, int shapeIdx, int yMin) {
        if (!OutputAlways) return;
        yMin -= 6;
        HashSet<Int2> pieceTranslated = new(MaterializeShape(pos, shapeIdx));
        StringBuilder sb = new();
        Int2 at = new(0, 0);
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
