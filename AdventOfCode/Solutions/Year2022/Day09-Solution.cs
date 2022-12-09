using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Security.Authentication.Web.Provider;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 09, "Rope Bridge")]
class Day09 : ASolution
{
    public Day09() : base(false) { }

    List<(string dir, int count)> moves = new();
    protected override void ParseInput() {
        foreach(var line in Input.SplitByNewline()) {
            var move = line.Split(' ');
            moves.Add((move[0], int.Parse(move[1])));
        }
    }
    protected override string SolvePartOne() {
        HashSet<IntCoord> visited = new() { (0, 0) };
        IntCoord head = new(0, 0);
        IntCoord tail = new(0, 0);

        PrintState(head, tail);
        foreach(var move in moves) {
            var delta = move.dir switch {
                "U" => IntCoord.Up,
                "D" => IntCoord.Down,
                "L" => IntCoord.Left,
                "R" => IntCoord.Right,
                _ => throw new Exception("unknown dir")
            };
            //Debug.WriteLine($"Direction: {move.dir} Count: {move.count}");

            for(int counter = move.count; counter > 0; counter--) {
                var lastHead = head;
                head += delta;
                if(Math.Sqrt(Math.Pow(head.X - tail.X, 2) + Math.Pow(head.Y - tail.Y, 2)) > 1.5) {
                    tail = lastHead;
                    visited.Add(tail);
                }
                PrintState(head, tail);
            }
        }
        return visited.Count.ToString();
    }
    protected override string SolvePartTwo() {
        HashSet<IntCoord> visited = new() { (0, 0) };
        IntCoord[] rope = new IntCoord[10];
        for (int i = 0; i < 10; i++) { rope[i] = new(0, 0); }

        //PrintState2(rope);
        foreach (var move in moves) {
            var delta = move.dir switch {
                "U" => IntCoord.Up,
                "D" => IntCoord.Down,
                "L" => IntCoord.Left,
                "R" => IntCoord.Right,
                _ => throw new Exception("unknown dir")
            };
            //Debug.WriteLine($"Direction: {move.dir} Count: {move.count}");

            for (int counter = move.count; counter > 0; counter--) {
                rope[0] += delta;
                for (int i = 1; i < 10; i++) {
                    var head = rope[i - 1];
                    var tail = rope[i];
                    if (Math.Sqrt(Math.Pow(head.X - tail.X, 2) + Math.Pow(head.Y - tail.Y, 2)) > 1.5) {
                        var xDelta = head.X - tail.X;
                        if (xDelta > 0) tail += IntCoord.Right;
                        else if( xDelta < 0) tail += IntCoord.Left;
                        var yDelta = head.Y - tail.Y;
                        if (yDelta > 0) tail += IntCoord.Down;
                        else if (yDelta < 0) tail += IntCoord.Up;

                        rope[i] = tail;
                        if( i == 9 ) visited.Add(tail);
                    }
                    //PrintState2(rope);
                }
                //Debug.WriteLine("===");
            }
        }
        return visited.Count.ToString();
    }

    void PrintState(IntCoord head, IntCoord tail) {
        return;
        const int rad = 5;
        IntCoord min = (-rad, -rad), max = (rad, rad), zero = (0,0);
        StringBuilder sb = new();
        for(int y = min.Y; y <= max.Y; y++) {
            for(int x = min.X; x <= max.X; x++) {
                IntCoord at = (x, y);
                if (at == head) sb.Append('H');
                else if (at == tail) sb.Append('T');
                else if (at == zero) sb.Append('s');
                else sb.Append('.');
            }
            sb.AppendLine();
        }
        Debug.WriteLine(sb.ToString());
    }

    void PrintState2(IntCoord[] rope) {
        const int rad = 5;
        IntCoord min = (-rad, -rad), max = (rad, rad), zero = (0, 0);
        StringBuilder sb = new();
        for (int y = min.Y; y <= max.Y; y++) {
            for (int x = min.X; x <= max.X; x++) {
                IntCoord at = (x, y);
                int sbLen = sb.Length;
                for(int i = 0; i < rope.Length; i++) {
                    if(rope[i] == at) {
                        if (i == 0) sb.Append('H');
                        else sb.Append(i);
                        break;
                    }
                }
                if( sbLen == sb.Length ) {
                    if (at == zero) sb.Append('s');
                    else sb.Append('.');
                }
            }
            sb.AppendLine();
        }
        Debug.WriteLine(sb.ToString());
    }
}
