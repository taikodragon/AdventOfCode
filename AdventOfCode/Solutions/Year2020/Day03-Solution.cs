using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2020, "Toboggan Trajectory", false)
        {
            
        }

        int TreeSearch(char[][] map, int slopeColumn, int slopeRow)
        {
            int r = 0, c = 0, trees = 0;
            for( ; r < map.Length; r += slopeRow, c += slopeColumn ) {
                c %= map[0].Length;
                if( map[r][c] == '#' ) trees++;
            }
            return trees;
        }
        protected override string SolvePartOne()
        {
            char[][] map = Input.SplitByNewline().Select(str => str.ToArray()).ToArray();

            return TreeSearch(map, 3, 1).ToString();
        }

        protected override string SolvePartTwo()
        {
            char[][] map = Input.SplitByNewline().Select(str => str.ToArray()).ToArray();

            int treeMult = 1;
            foreach( var slope in new List<(int, int)> {
                (1, 1), (3, 1), (5, 1), (7, 1), (1, 2)
            } ) {
                treeMult *= TreeSearch(map, slope.Item1, slope.Item2);
            }


            return treeMult.ToString();
        }
    }
}
