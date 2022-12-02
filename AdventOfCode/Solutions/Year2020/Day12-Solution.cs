using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 12, "Rain Risk")]
    class Day12 : ASolution
    {
        enum Direction
        {
            N = 0, E = 90, S = 180, W = 270, L = 500, R = 501, F = 502
        }


        List<(Direction dir, int value)> instructions;

        public Day12() : base(false)
        {
            

            instructions = Input.SplitByNewline().Select(s => {
                return ((Direction)Enum.Parse(typeof(Direction), s[0].ToString()), int.Parse(s[1..]));
            }).ToList();
        }

        void HandleInstruction((Direction dir, int value) instr, ref Direction shipDirection, ref int east, ref int north) {
            switch( instr.dir ) {
                case Direction.N:
                    north += instr.value;
                    break;
                case Direction.E:
                    east += instr.value;
                    break;
                case Direction.S:
                    north -= instr.value;
                    break;
                case Direction.W:
                    east -= instr.value;
                    break;
                case Direction.L:
                    if( (int)shipDirection < instr.value )
                        shipDirection = (Direction)(((int)shipDirection - instr.value + 360) % 360);
                    else
                        shipDirection = (Direction)(((int)shipDirection - instr.value) % 360);
                    break;
                case Direction.R:
                    shipDirection = (Direction)(((int)shipDirection + instr.value) % 360);
                    break;
                case Direction.F:
                    HandleInstruction((shipDirection, instr.value), ref shipDirection, ref east, ref north);
                    break;
            }
        }

        protected override string SolvePartOne()
        {
            Direction shipDirection = Direction.E;
            int east = 0, north = 0;

            foreach(var instr in instructions ) {
                HandleInstruction(instr, ref shipDirection, ref east, ref north);
            }

            return (Math.Abs(east) + Math.Abs(north)).ToString();
        }


        void RotateCoord(IntCoord at, int degrees) {
            switch(degrees) {
                default: break;
                case -90:
                case 90:
                case 180:
                case 270:
                case -180:
                case -270:
                    double radians = degrees / 180.0 * Math.PI;
                    int tempx = at.X;
                    at.X = (int)Math.Round((tempx * Math.Cos(radians) - at.Y * Math.Sin(radians)));
                    at.Y = (int)Math.Round((tempx * Math.Sin(radians) + at.Y * Math.Cos(radians)));
                    break;
            }
        }

        void HandleInstructionPart2((Direction dir, int value) instr, IntCoord waypoint, IntCoord shipAt) {
            switch( instr.dir ) {
                case Direction.N:
                    waypoint.Y += instr.value;
                    break;
                case Direction.E:
                    waypoint.X += instr.value;
                    break;
                case Direction.S:
                    waypoint.Y -= instr.value;
                    break;
                case Direction.W:
                    waypoint.X -= instr.value;
                    break;
                case Direction.L:
                    RotateCoord(waypoint, instr.value);
                    break;
                case Direction.R:
                    RotateCoord(waypoint, -instr.value);
                    break;
                case Direction.F:
                    shipAt.X += waypoint.X * instr.value;
                    shipAt.Y += waypoint.Y * instr.value;
                    break;
            }
        }

        protected override string SolvePartTwo()
        {
            IntCoord waypoint = new IntCoord(10, 1);
            IntCoord shipAt = new IntCoord(0, 0);

            foreach(var instr in instructions) {
                HandleInstructionPart2(instr, waypoint, shipAt);
            }


            return Utilities.ManhattanDistance((0,0), (shipAt.X, shipAt.Y)).ToString();
        }
    }
}
