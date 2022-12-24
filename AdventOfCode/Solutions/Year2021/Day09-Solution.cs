using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    [DayInfo(2021, 09, "Smoke Basin")]
    class Day09 : ASolution
    {
        Dictionary<Int2, int> heightMap = new();
        List<Int2> minima = new();
        int maxRow, maxCol;
        Int2 up = new Int2(1, 0),
            down = new Int2(-1, 0),
            left = new Int2(0, -1),
            right = new Int2(0, 1);

        public Day09() : base(false)
        {
            int row = 0;
            foreach(string line in Input.SplitByNewline()) {
                int col = 0;
                foreach (char cell in line) {
                    int height = int.Parse(cell.ToString());
                    heightMap.Add(new Int2(row, col), height);
                    col++;
                }
                if( col > maxCol ) maxCol = col;
                row++;
            }
            maxRow = row;
        }

        protected override string SolvePartOne()
        {
            Int2 at;
            int sum = 0;
            for(int x = 0; x < maxRow; x++) {
                for (int y = 0; y < maxCol; y++) {
                    at = new Int2(x, y);
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
            Int2[] directions = new Int2[] {
                up, down, left, right
            };
            List<int> areas = new();
            foreach(var low in minima) {
                int cellCount = 0;
                Queue<Int2> pending = new();
                pending.Enqueue(low);

                HashSet<Int2> visited = new();
                visited.Add(low);
                while(pending.Count > 0) {
                    Int2 at = pending.Dequeue();
                    cellCount++;
                    foreach(var dir in directions) {
                        Int2 next = at + dir;
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
