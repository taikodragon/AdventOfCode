using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 05, "Supply Stacks")]
class Day05 : ASolution
{
    Dictionary<int, List<char>> crates = new();
    List<(int numCrates, int fromCol, int toCol)> commands = new();

    public Day05() : base(false)
    {
        
    }

    protected override void ParseInput() {
        var lines = Input.SplitByNewline();
;
        List<string> crateLines = new(), commandLines = new();

        bool crateComplete = false;
        foreach (var line in lines) {
            if( line == string.Empty) {
                crateComplete = true;
            }
            else if( !crateComplete) {
                crateLines.Add(line);
            }
            else {
                commandLines.Add(line);
            }
        }

        foreach (var line in crateLines) {
            int column = 1;
            foreach(var chunk in line.Chunk(4)) {
                if (chunk[0] == '[') {
                    if( !crates.ContainsKey(column) ) { crates[column] = new(); }
                    crates[column].Add(chunk[1]);
                }
                column++;
            }
        }

        foreach(var line in commandLines) {
            var parts = line.Split(' ');
            commands.Add((int.Parse(parts[1]), int.Parse(parts[3]), int.Parse(parts[5])));
        }
    }

    Stack<char> buffer = new();
    void LiftCrates2(int from, int count) {
        buffer.Clear();
        var stack = crates[from];
        for(int i = 0; i < count; i++) {
            buffer.Push(stack[0]);
            stack.RemoveAt(0);
        }
    }
    void LiftCrates(int from, int count) {
        buffer.Clear();
        var stack = crates[from];
        for (int i = count - 1; i >= 0; i--) {
            buffer.Push(stack[i]);
        }
        stack.RemoveRange(0, count);
    }
    void DropCrates(int to) {
        var stack = crates[to];
        while(buffer.Count > 0) {
            stack.Insert(0, buffer.Pop());
        }
    }
    protected override string SolvePartOne()
    {
        foreach(var com in commands) {
            LiftCrates(com.fromCol, com.numCrates);
            DropCrates(com.toCol);
        }
        string r = "";
        foreach (var key in crates.Keys.OrderBy(k => k)) {
            var stack = crates[key];
            r += stack[0];
        }
        return r;
    }

    protected override string SolvePartTwo()
    {
        crates.Clear(); commands.Clear();
        ParseInput();

        foreach (var com in commands) {
            LiftCrates2(com.fromCol, com.numCrates);
            DropCrates(com.toCol);
        }
        string r = "";
        foreach (var key in crates.Keys.OrderBy(k => k)) {
            var stack = crates[key];
            r += stack[0];
        }
        return r;
    }
}
