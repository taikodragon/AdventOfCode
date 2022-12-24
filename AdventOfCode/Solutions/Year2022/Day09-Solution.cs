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
        HashSet<Int2> visited = new() { (0, 0) };
        Int2 head = new(0, 0);
        Int2 tail = new(0, 0);

        PrintState(head, tail);
        foreach(var move in moves) {
            var delta = move.dir switch {
                "U" => Int2.Up,
                "D" => Int2.Down,
                "L" => Int2.Left,
                "R" => Int2.Right,
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
        HashSet<Int2> visited = new() { (0, 0) };
        Int2[] rope = new Int2[10];
        for (int i = 0; i < 10; i++) { rope[i] = new(0, 0); }

        PrintState2(rope);
        foreach (var move in moves) {
            var delta = move.dir switch {
                "U" => Int2.Up,
                "D" => Int2.Down,
                "L" => Int2.Left,
                "R" => Int2.Right,
                _ => throw new Exception("unknown dir")
            };

            for (int counter = move.count; counter > 0; counter--) {
                rope[0] += delta;
                for (int i = 1; i < 10; i++) {
                    var head = rope[i - 1];
                    var tail = rope[i];
                    if (Math.Sqrt(Math.Pow(head.X - tail.X, 2) + Math.Pow(head.Y - tail.Y, 2)) > 1.5) {
                        var xDelta = head.X - tail.X;
                        if (xDelta > 0) tail += Int2.Right;
                        else if (xDelta < 0) tail += Int2.Left;
                        var yDelta = head.Y - tail.Y;
                        if (yDelta > 0) tail += Int2.Down;
                        else if (yDelta < 0) tail += Int2.Up;

                        rope[i] = tail;
                        if (i == 9) visited.Add(tail);
                    }
                    else break;
                    PrintState2(rope);
                }
            }
        }
        return visited.Count.ToString();
    }

    void PrintState(Int2 head, Int2 tail) {
        if( !(UseDebugInput || OutputAlways) ) return;
        const int rad = 5;
        Int2 min = (-rad, -rad), max = (rad, rad), zero = (0,0);
        StringBuilder sb = new();
        for(int y = min.Y; y <= max.Y; y++) {
            for(int x = min.X; x <= max.X; x++) {
                Int2 at = (x, y);
                if (at == head) sb.Append('H');
                else if (at == tail) sb.Append('T');
                else if (at == zero) sb.Append('s');
                else sb.Append('.');
            }
            sb.AppendLine();
        }
        WriteLine(sb);
    }

    void PrintState2(Int2[] rope) {
        if (!(UseDebugInput || OutputAlways)) return;
        const int rad = 5;
        Int2 min = (-rad, -rad), max = (rad, rad), zero = (0, 0);
        StringBuilder sb = new();
        for (int y = min.Y; y <= max.Y; y++) {
            for (int x = min.X; x <= max.X; x++) {
                Int2 at = (x, y);
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
        WriteLine(sb);
    }
}
