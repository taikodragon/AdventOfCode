using Microsoft.UI.Xaml;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Devices.Enumeration;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 10, "Cathode-Ray Tube")]
class Day10 : ASolution
{
    class Instruction {
        const string Noop = "noop", Addx = "addx";

        public Instruction(string line) {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Code = parts[0];
            if(parts.Length > 1)
                Argument = int.Parse(parts[1]);
            CycleTime = Code switch {
                Noop => 1,
                Addx => 2,
                _ => throw new Exception("Unknown code type")
            };
        }
        public string Code { get; }
        public int CycleTime { get; }
        public int? Argument { get; }
        public bool Complete => remainingCycles == 0;
        int remainingCycles { get; set; } = -1;
        public void Reset() {
            remainingCycles = -1;
        }
        public void BeginCycle() {
            if(remainingCycles == -1) {
                remainingCycles = CycleTime;
            }
        }
        public int EndCycle(int regX) {
            remainingCycles--;
            if(Complete) {
                if( Code == Addx ) {
                    regX += Argument.Value;
                }
            }
            return regX;
        }
    }
    public Day10() : base(false) {
    }


    List<Instruction> instructions = new();
    protected override void ParseInput() {
        instructions = Input.SplitByNewline()
            .Select(line => new Instruction(line))
            .ToList();
    }
    protected override string SolvePartOne() {
        Dictionary<int,int> registerLog = new();
        Dictionary<int, int> samples = new();
        int regX = 1;

        int clock = 0;
        Queue<Instruction> queue = new Queue<Instruction>(instructions);
        Instruction current = null;
        while(queue.Count > 0 ) {
            current ??= queue.Dequeue();
            while(current != null) {
                clock++;
                current.BeginCycle();
                registerLog.Add(clock, regX);
                if( (clock + 20) % 40 == 0) { samples.Add(clock, regX); }
                regX = current.EndCycle(regX);

                if (current.Complete) current = null;
            }
        }

        instructions.ForEach(i => i.Reset());


        return samples.Select(kv => kv.Key * kv.Value).Sum().ToString();
    }
    protected override string SolvePartTwo() {
        Dictionary<int, int> registerLog = new();
        Dictionary<int, int> samples = new();
        StringBuilder screen = new StringBuilder();
        int regX = 1;

        int clock = 0;
        Queue<Instruction> queue = new Queue<Instruction>(instructions);
        Instruction current = null;
        while (queue.Count > 0) {
            current ??= queue.Dequeue();
            while (current != null) {
                clock++;
                current.BeginCycle();
                registerLog.Add(clock, regX);

                int pixel = ((clock - 1) % 40);
                int spriteMin = regX - 1, spriteMax = regX + 1;
                if( pixel >= spriteMin && pixel <= spriteMax ) {
                    screen.Append('#');
                } else {
                    screen.Append('.');
                }
                if (clock % 40 == 0) { screen.AppendLine(); }


                regX = current.EndCycle(regX);

                if (current.Complete) current = null;
            }
        }

        instructions.ForEach(i => i.Reset());


        return screen.ToString();
    }
}
