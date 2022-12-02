using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{
    [DayInfo(2020, 11, "Seating System")]
    class Day11 : ASolution
    {
        
        const char floor = '.', empty = 'L', occupied = '#';

        char[,] map;
        public Day11() : base(false)
        {
            

            var lines = Input.SplitByNewline();
            map = new char[lines.Count, lines[0].Length];
            for( int x = 0; x < map.GetLength(0); ++x ) {
                for( int y = 0; y < map.GetLength(1); ++y ) {
                    map[x, y] = lines[x][y];
                }
            }
        }


        static IntCoord[] ComputeAdjacent(IntCoord fromCoord) {
            return new IntCoord[] {
                new IntCoord(fromCoord.X - 1, fromCoord.Y - 1),
                new IntCoord(fromCoord.X - 1, fromCoord.Y),
                new IntCoord(fromCoord.X - 1, fromCoord.Y + 1),
                new IntCoord(fromCoord.X, fromCoord.Y + 1),
                new IntCoord(fromCoord.X, fromCoord.Y - 1),
                new IntCoord(fromCoord.X + 1, fromCoord.Y - 1),
                new IntCoord(fromCoord.X + 1, fromCoord.Y),
                new IntCoord(fromCoord.X + 1, fromCoord.Y + 1),
            };
        }

        enum OccupiedResult
        {
            Occupied, Empty, Floor, Wall
        }
        static OccupiedResult IsOccupied(char[,] map, IntCoord at) {
            if( at.X < 0 || at.Y < 0) return OccupiedResult.Wall;
            if( at.X >= map.GetLength(0) || at.Y >= map.GetLength(1) ) return OccupiedResult.Wall;
            if( map[at.X, at.Y] == floor ) return OccupiedResult.Floor;
            if( map[at.X,at.Y] == occupied ) return OccupiedResult.Occupied;
            return OccupiedResult.Empty;
        }

        static char[,] SimulateRound(char[,] now) {
            char[,] next = (char[,])now.Clone();
            for(int x = 0; x < next.GetLength(0); ++x ) {
                for(int y = 0; y < next.GetLength(1); ++y ) {
                    IntCoord at = new IntCoord(x, y);
                    int adjacentCount = ComputeAdjacent(at).Count(c => IsOccupied(now, c) == OccupiedResult.Occupied);
                    if( now[x,y] == empty && adjacentCount == 0 )
                        next[x,y] = occupied;
                    else if( now[x,y] == occupied && adjacentCount >= 4 )
                        next[x,y] = empty;
                }
            }
            return next;
        }

        static string AsComparable(char[,] map) {
            StringBuilder sb = new StringBuilder(map.Length);
            for( int x = 0; x < map.GetLength(0); ++x ) {
                for( int y = 0; y < map.GetLength(1); ++y ) {
                    sb.Append(map[x, y]);
                }
                sb.Append('\n');
            }

            return sb.ToString();
            //return string.Concat(map.Flatten().Select(row => new string(row) + "\n"));
        }


        protected override string SolvePartOne()
        {
            char[,] state = (char[,])map.Clone();
            string before = string.Empty, after = AsComparable(state); 
            while( before != after ) {
                before = after;
                char[,] newMap = SimulateRound(state);
                after = AsComparable(newMap);
                state = newMap;
            }


            return before.Count(c => c == occupied).ToString();
        }


        enum Direction
        {
            N, NE, E, SE, S, SW, W, NW
        }
        static IntCoord GetAdjacentCoord(IntCoord at, Direction dir, int dist) {
            switch(dir) {
                case Direction.N:
                    return new IntCoord(at.X - dist, at.Y);
                case Direction.NE:
                    return new IntCoord(at.X - dist, at.Y + dist);
                case Direction.NW:
                    return new IntCoord(at.X - dist, at.Y - dist);
                case Direction.E:
                    return new IntCoord(at.X, at.Y + dist);
                case Direction.W:
                    return new IntCoord(at.X, at.Y - dist);
                case Direction.S:
                    return new IntCoord(at.X + dist, at.Y);
                case Direction.SE:
                    return new IntCoord(at.X + dist, at.Y + dist);
                case Direction.SW:
                    return new IntCoord(at.X + dist, at.Y - dist);
            }
            return at;
        }

        static int FindAdjacentOccupied(char[,] map, IntCoord at) {
            int occupiedCount = 0;
            Direction[] dirs = new Direction[] { Direction.N, Direction.NE, Direction.E, Direction.SE, Direction.S, Direction.SW, Direction.W, Direction.NW };
            foreach(var dir in dirs) {
                int dist = 1;
                OccupiedResult result;
                bool notDone = true;
                while(notDone) {
                    var adCoord = GetAdjacentCoord(at, dir, dist);
                    result = IsOccupied(map, adCoord);
                    switch( result ) {
                        case OccupiedResult.Occupied:
                            occupiedCount++;
                            notDone = false;
                            break;
                        case OccupiedResult.Empty:
                        case OccupiedResult.Wall:
                            notDone = false;
                            break;
                        case OccupiedResult.Floor:
                            dist++;
                            break;
                    }
                }
            }
            return occupiedCount;
        }

        static char[,] SimulateRoundPart2(char[,] now) {
            char[,] next = (char[,])now.Clone();
            for( int x = 0; x < next.GetLength(0); ++x ) {
                for( int y = 0; y < next.GetLength(1); ++y ) {
                    IntCoord at = new IntCoord(x, y);
                    
                    int adjacentCount = FindAdjacentOccupied(now, at);
                    if( now[x, y] == empty && adjacentCount == 0 )
                        next[x, y] = occupied;
                    else if( now[x, y] == occupied && adjacentCount >= 5 )
                        next[x, y] = empty;
                }
            }
            return next;
        }

        protected override string SolvePartTwo()
        {
            char[,] state = (char[,])map.Clone();
            string before = string.Empty, after = AsComparable(state);
            while( before != after ) {
                before = after;
                char[,] newMap = SimulateRoundPart2(state);
                after = AsComparable(newMap);
                state = newMap;
            }


            return before.Count(c => c == occupied).ToString();
        }
    }
}

