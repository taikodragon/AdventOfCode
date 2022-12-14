using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Security.Authentication.Web.Core;
using Windows.Storage.AccessCache;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 11, "")]
class Day11 : ASolution
{
    class Monkey {
        public int Inspections { get; set; } = 0;
        public Queue<long> Items { get; } = new();
        public char Operation { get; set; }
        public long? OperationArg { get; set; }
        public (uint testArg, int onTrue, int onFalse) Test { get; set; }

        public Monkey Copy() {
            var result = new Monkey() {
                Operation = Operation,
                OperationArg = OperationArg,
                Test = Test
            };
            foreach(var item in Items) { result.Items.Enqueue(item); }
            return result;
        }


    }

    public Day11() : base(false) {
    }

    Dictionary<int, Monkey> monkeys = new();
    protected override void ParseInput() {
        foreach (var lines in Input.SplitByNewline().Chunk(7)) {
            //Monkey 0:
            int monkeyNum = int.Parse(lines[0][7..].TrimEnd(':'));
            string opArg = lines[2][25..];
            Monkey current = new Monkey() {
                //  Operation: new = old * 19
                Operation = lines[2][23],
                OperationArg = opArg == "old" ? null : long.Parse(opArg),
                //  Test: divisible by 23
                //    If true: throw to monkey 2
                //    If false: throw to monkey 3
                Test = (uint.Parse(lines[3][21..]), int.Parse(lines[4][29..]), int.Parse(lines[5][30..]))
            };
            //  Starting items: 79, 98
            foreach (var itemStr in lines[1][18..].Split(',', StringSplitOptions.TrimEntries)) {
                current.Items.Enqueue(long.Parse(itemStr));
            }

            monkeys.Add(monkeyNum, current);
        }
    }

    void DoTurn(Dictionary<int,Monkey> sim, Monkey current, bool worryReduction = true, long mod = 0) {
        while(current.Items.Count > 0) {
            long item = current.Items.Dequeue();
            // track inspections
            current.Inspections++;
            // Apply Op
            if (current.Operation == '*') item *= current.OperationArg ?? item;
            else if (current.Operation == '+') item += current.OperationArg ?? item;
            // Reduce worry
            if (worryReduction) item /= 3;
            else item %= mod;
            // Test and throw
            var test = current.Test;
            if( item % test.testArg == 0 ) {
                sim[test.onTrue].Items.Enqueue(item);
            } else {
                sim[test.onFalse].Items.Enqueue(item);
            }
        }
    }
    protected override string SolvePartOne() {
        Dictionary<int, Monkey> sim = new();
        foreach(var kv in monkeys) {
            sim.Add(kv.Key, kv.Value.Copy());
        }

        int round = 0, maxRounds = 20;
        for(; round < maxRounds; round++) {
            foreach(var kv in sim) {
                DoTurn(sim, kv.Value);
            }
        }

        var topTwo = sim
            .Select(kv => (kv.Key, kv.Value.Inspections))
            .OrderByDescending(p => p.Inspections)
            .Take(2)
            .ToList();

        return (topTwo[0].Inspections * topTwo[1].Inspections).ToString();
    }

    protected override string SolvePartTwo() {
        Dictionary<int, Monkey> sim = new();
        foreach (var kv in monkeys) {
            sim.Add(kv.Key, kv.Value.Copy());
        }

        long mod = Utilities.FindLCM(monkeys.Select(kv => (long)(kv.Value.Test.testArg)));
        int round = 0, maxRounds = 10000;
        for (; round < maxRounds; round++) {
            foreach (var kv in sim) {
                DoTurn(sim, kv.Value, false, mod);
            }
            if (round % 100 == 0) Console.WriteLine($"Completed round {round}");
        }

        if( UseDebugInput || OutputAlways ) {
            foreach(var pair in sim.Select(kv => (kv.Key, kv.Value.Inspections)).OrderBy(kv => kv.Key)) {
                WriteLine(pair);
            }
        }

        var topTwo = sim
            .Select(kv => (kv.Key, kv.Value.Inspections))
            .OrderByDescending(p => p.Inspections)
            .Take(2)
            .ToList();

        return ((long)topTwo[0].Inspections * topTwo[1].Inspections).ToString();
    }


}
