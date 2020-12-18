using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day17 : ASolution
    {

        public Day17() : base(17, 2020, "")
        {
            UseDebugInput = false;


        }

        int ActiveNeighbors(int x,int y, int z, Dictionary<(int x, int y, int z), bool> now) {
            (int x, int y, int z) me = (x, y, z);
            int count = 0;
            for( int ix = -1; ix <= 1; ix++ ) {
                for( int iy = -1; iy <= 1; iy++ ) {
                    for( int iz = -1; iz <= 1; iz++ ) {
                        (int x, int y, int z) at = (x + ix, y + iy, z + iz);
                        if( at == me ) continue;
                        if( now.GetValueOrDefault(at, false) ) count++;
                    }
                }
            }
            return count;
        }





        Dictionary<(int x, int y, int z), bool> Simulate(Dictionary<(int x, int y, int z), bool> now) {
            Dictionary<(int x, int y, int z), bool> next = new Dictionary<(int x, int y, int z), bool>((int)Math.Round(now.Count* 1.25));
            var keys = now.Keys;
            int xMin = keys.Min(c => c.x) - 1,
                xMax = keys.Max(c => c.x) + 1,
                yMin = keys.Min(c => c.y) - 1,
                yMax = keys.Max(c => c.y) + 1,
                zMin = keys.Min(c => c.z) - 1,
                zMax = keys.Max(c => c.z) + 1;
            for(int x = xMin; x <= xMax; x++ ) {
                for(int y = yMin; y <= yMax; y++) {
                    for( int z = zMin; z <= zMax; z++ ) {
                        var at = (x, y, z);
                        bool myState = now.GetValueOrDefault(at, false);
                        int actives = ActiveNeighbors(x, y, z, now);
                        if( myState ) {
                            next[at] = (actives == 2 || actives == 3);
                        } else if( actives == 3 ) {
                            next[at] = true;
                        }
                    }
                }
            }
            return next;
        }

        protected override string SolvePartOne()
        {
            Dictionary<(int x, int y, int z), bool> gameSpace = new Dictionary<(int x, int y, int z), bool>();

            var lines = Input.SplitByNewline(false, true);
            for(int i = 0; i < lines.Count; ++i) {
                for(int j = 0; j < lines[i].Length; ++j) {
                    if( lines[i][j] == '#' )
                        gameSpace.Add((i, j, 0), true);
                }
            }

            ;
            for( int iter = 0; iter < 6; iter++ ) {
                gameSpace = Simulate(gameSpace);
            }

            return gameSpace.Values.Count(b => b).ToString();
        }

        int ActiveNeighbors4D(int x, int y, int z, int w, Dictionary<(int x, int y, int z, int w), bool> now) {
            var me = (x, y, z,w);
            int count = 0;
            for( int ix = -1; ix <= 1; ix++ ) {
                for( int iy = -1; iy <= 1; iy++ ) {
                    for( int iz = -1; iz <= 1; iz++ ) {
                        for(int iw = -1; iw <= 1; iw++) {
                            var at = (x + ix, y + iy, z + iz, w + iw);
                            if( at == me ) continue;
                            if( now.GetValueOrDefault(at, false) ) count++;
                        }
                    }
                }
            }
            return count;
        }





        Dictionary<(int x, int y, int z, int w), bool> Simulate4D(Dictionary<(int x, int y, int z,int w), bool> now) {
            Dictionary<(int x, int y, int z, int w), bool> next = new Dictionary<(int x, int y, int z, int w), bool>((int)Math.Round(now.Count * 1.25));
            var keys = now.Keys;
            int xMin = keys.Min(c => c.x) - 1,
                xMax = keys.Max(c => c.x) + 1,
                yMin = keys.Min(c => c.y) - 1,
                yMax = keys.Max(c => c.y) + 1,
                zMin = keys.Min(c => c.z) - 1,
                zMax = keys.Max(c => c.z) + 1,
                wMin = keys.Min(c => c.w) - 1,
                wMax = keys.Max(c => c.w) + 1;
            for( int x = xMin; x <= xMax; x++ ) {
                for( int y = yMin; y <= yMax; y++ ) {
                    for( int z = zMin; z <= zMax; z++ ) {
                        for( int w = wMin; w <= wMax; w++ ) {
                            var at = (x, y, z, w);
                            bool myState = now.GetValueOrDefault(at, false);
                            int actives = ActiveNeighbors4D(x, y, z, w, now);
                            if( myState ) {
                                next[at] = (actives == 2 || actives == 3);
                            }
                            else if( actives == 3 ) {
                                next[at] = true;
                            }
                        }
                    }
                }
            }
            return next;
        }


        protected override string SolvePartTwo()
        {
            Dictionary<(int x, int y, int z, int w), bool> gameSpace = new Dictionary<(int x, int y, int z, int w), bool>();

            var lines = Input.SplitByNewline(false, true);
            for( int i = 0; i < lines.Count; ++i ) {
                for( int j = 0; j < lines[i].Length; ++j ) {
                    if( lines[i][j] == '#' )
                        gameSpace.Add((i, j, 0, 0), true);
                }
            }

            ;
            for( int iter = 0; iter < 6; iter++ ) {
                gameSpace = Simulate4D(gameSpace);
            }

            return gameSpace.Values.Count(b => b).ToString();
        }
    }
}
