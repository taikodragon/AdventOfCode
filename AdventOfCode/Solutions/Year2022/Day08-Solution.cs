using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 08, "Treetop Tree House")]
class Day08 : ASolution
{
    int width = 5, height = 5;


    public Day08() : base(false) {
    }

    Dictionary<Int2, int> grid = new Dictionary<Int2, int>();
    protected override void ParseInput() {
        var arr = Input.SplitByNewline();
        width = arr[0].Length;
        height = arr.Count;
        string input = string.Concat(arr);
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                grid[new(x, y)] = int.Parse(input[y * width + x].ToString());
            }
        }
    }
    protected override string SolvePartOne() {
        HashSet<Int2> visLog = new() {
            new(0, 0),
            new(width - 1, 0),
            new(0, height - 1),
            new(width - 1, height - 1),
        };
        Queue<(Int2 start, Int2 stride)> walks = new();
        for(int x = 1; x < width - 1; x++) {
            walks.Enqueue((new(x, 0), new(0, 1)));
            walks.Enqueue((new(x, height - 1), new(0, -1)));
        }
        for (int y = 1; y < height - 1; y++) {
            walks.Enqueue((new(0, y), new(1, 0)));
            walks.Enqueue((new(width - 1, y), new(-1, 0)));
        }

        while(walks.Count > 0) {
            var walk = walks.Dequeue();
            Int2 at = walk.start + walk.stride;
            visLog.Add(walk.start);
            int last = grid[walk.start];
            while(at.X < width && at.X >= 0 && at.Y < height && at.Y >= 0) {
                if(last < grid[at]) {
                    visLog.Add(at);
                }
                last = Math.Max(last, grid[at]);
                at += walk.stride;
            }
        }

        return visLog.Count.ToString();
    }
    protected override string SolvePartTwo() {
        (Int2 xy, int score) highestView = (new(-1, -1), 0);
        Int2 up = new(0, -1), down = new(0, 1), left = new(-1, 0), right = new(1, 0);
        foreach(var kv in grid) {
            var start = kv.Key;
            var refHeight = kv.Value;
            Queue<(Int2 start, Int2 stride)> walks = new();
            walks.Enqueue((start + up, up));
            walks.Enqueue((start + down, down));
            walks.Enqueue((start + left, left));
            walks.Enqueue((start + right, right));

            int score = 1;
            while(walks.Count > 0 && score > 0) {
                var walk = walks.Dequeue();
                int walkScore = 0;
                Int2 at = walk.start;
                while (at.X < width && at.X >= 0 && at.Y < height && at.Y >= 0) {
                    walkScore++;
                    if (grid[at] >= refHeight) {
                        break;
                    }
                    at += walk.stride;
                }
                score *= walkScore;
            }

            if( highestView.score < score ) {
                highestView = (start, score);
            }
        }

        return highestView.score.ToString();
    }
}
