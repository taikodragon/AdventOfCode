using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day09 : ASolution
    {
        Dictionary<IntCoord, int> heightMap = new();
        List<IntCoord> minima = new();
        int maxRow, maxCol;
        IntCoord up = new IntCoord(1, 0),
            down = new IntCoord(-1, 0),
            left = new IntCoord(0, -1),
            right = new IntCoord(0, 1);

        public Day09() : base(09, 2021, "Smoke Basin", false)
        {
            int row = 0;
            foreach(string line in Input.SplitByNewline()) {
                int col = 0;
                foreach (char cell in line) {
                    int height = int.Parse(cell.ToString());
                    heightMap.Add(new IntCoord(row, col), height);
                    col++;
                }
                if( col > maxCol ) maxCol = col;
                row++;
            }
            maxRow = row;
        }

        protected override string SolvePartOne()
        {
            IntCoord at;
            int sum = 0;
            for(int x = 0; x < maxRow; x++) {
                for (int y = 0; y < maxCol; y++) {
                    at = new IntCoord(x, y);
                    int myHeight = heightMap[at];

                    if (heightMap.GetValueOrDefault(at + up, 10) <= myHeight ||
                        heightMap.GetValueOrDefault(at + down, 10) <= myHeight ||
                        heightMap.GetValueOrDefault(at + left, 10) <= myHeight ||
                        heightMap.GetValueOrDefault(at + right, 10) <= myHeight) {
                    } else {
                        sum += myHeight + 1;
                        minima.Add(at);
                    }
                }
            }
            return sum.ToString();
        }

        protected override string SolvePartTwo()
        {
            IntCoord[] directions = new IntCoord[] {
                up, down, left, right
            };
            List<int> areas = new();
            foreach(var low in minima) {
                int cellCount = 0;
                Queue<IntCoord> pending = new();
                pending.Enqueue(low);

                HashSet<IntCoord> visited = new();
                visited.Add(low);
                while(pending.Count > 0) {
                    IntCoord at = pending.Dequeue();
                    cellCount++;
                    foreach(var dir in directions) {
                        IntCoord next = at + dir;
                        if( heightMap.GetValueOrDefault(next, 10) < 9 && !visited.Contains(next) ) {
                            visited.Add(next);
                            pending.Enqueue(next);
                        }
                    }
                }
                areas.Add(cellCount);
            }
            long ans = 1;
            foreach(int count in areas.OrderByDescending(x => x).Take(3)) {
                ans *= count;
            }
            return ans.ToString();
        }
    }
}
