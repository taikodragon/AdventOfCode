using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Networking.NetworkOperators;

namespace AdventOfCode.Solutions.Year2022
{


    [DayInfo(2022, 21, "Monkey Math")]
    class Day21 : ASolution
    {

        public Day21() : base(false)
        {
            
        }

        Dictionary<string, Monkey> monkeys = new();

        protected override void ParseInput() {
            monkeys.Clear();
            foreach(var line in Input.SplitByNewline()) {
                var parts = line.Split(' ', StringSplitOptions.TrimEntries)
                    .Select(s => s.Trim(':'))
                    .ToArray();

                var mk = new Monkey() {
                    Name = parts[0]
                };
                if( parts.Length == 2 ) {
                    mk.ResolvedValue = int.Parse(parts[1]);
                }
                else {
                    mk.Left = parts[1];
                    mk.Operation= parts[2];
                    mk.Right = parts[3];
                }
                monkeys.Add(mk.Name, mk);
            }
        }


        protected override object SolvePartOneRaw()
        {
            HashSet<string> opset = new(monkeys.Count);
            Queue<string> tovisit = new();
            tovisit.Enqueue("root");
            while(tovisit.Count > 0) {
                var next = tovisit.Dequeue();
                var mk = monkeys[next];
                if (mk.Left is not null) tovisit.Enqueue(mk.Left);
                if (mk.Right is not null) tovisit.Enqueue(mk.Right);
                if (mk.ResolvedValue is null) opset.Add(mk.Name);
            }
            foreach (var name in opset) { tovisit.Enqueue(name); }

            var root = monkeys["root"];
            while(root.ResolvedValue is null && tovisit.Count > 0) {
                var next = tovisit.Dequeue();
                var mk = monkeys[next];
                if (mk.ResolvedValue is not null) continue; // skip resolved nodes
                var left = monkeys[mk.Left];
                var right = monkeys[mk.Right];
                if (left.ResolvedValue is null || right.ResolvedValue is null)
                    tovisit.Enqueue(mk.Name);
                else {
                    mk.ResolvedValue = mk.Operation switch {
                        "+" => left.ResolvedValue + right.ResolvedValue,
                        "-" => left.ResolvedValue - right.ResolvedValue,
                        "*" => left.ResolvedValue * right.ResolvedValue,
                        "/" => left.ResolvedValue / right.ResolvedValue,
                        _ => throw new Exception("unknow operator")
                    };
                }
            }
            return root.ResolvedValue;
        }

        protected override object SolvePartTwoRaw() {
            ParseInput(); // reset
            const string human = "humn";
            var root = monkeys["root"];
            var left = monkeys[root.Left];
            var right = monkeys[root.Right];
            var self = monkeys[human];

            string humanSide, nonHuman;
            {
                var leftDeps = GetDeps(left.Name);
                if (leftDeps.Contains(human)) {
                    humanSide = left.Name;
                    nonHuman = right.Name;
                }
                else {
                    humanSide = right.Name;
                    nonHuman = left.Name;
                }
            }

            List<Monkey> pathToHuman;
            {
                Stack<(Monkey at, bool leftDone, bool rightDone)> depGraph = new();
                var at = monkeys[humanSide];
                bool leftDone = false, rightDone = false;
                while (at.Name != human) {
                    if (!leftDone && at.Left is not null) {
                        depGraph.Push((at, true, false));
                        string atLeftName = at.Left;
                        at = monkeys[atLeftName];
                        leftDone = false;
                        rightDone = false;
                    }
                    else if (!rightDone && at.Right is not null) {
                        depGraph.Push((at, true, true));
                        string atRightName = at.Right;
                        at = monkeys[atRightName];
                        leftDone = false;
                        rightDone = false;
                    }
                    else { // finished searching this done, pop
                        if (depGraph.Count == 0) { throw new Exception("humn not found"); }
                        var up = depGraph.Pop();
                        at = up.at;
                        leftDone = up.leftDone;
                        rightDone = up.rightDone;
                    }
                }
                pathToHuman = depGraph.Select(t => t.at).ToList();
            }
            void ResetHumanPath() {
                foreach(var mk in pathToHuman) { mk.Reset();  }
            }

            var matchValue = ResolveFor(nonHuman);


            long humanSideValue;
            long humanSideValueRef1 = humanSideValue = ResolveFor(humanSide);
            self.ResolvedValue++;
            ResetHumanPath();
            var humanSideValueRef2 = ResolveFor(humanSide);
            self.ResolvedValue--;

            long delta = (Math.Abs(matchValue - humanSideValueRef1) > Math.Abs(matchValue - humanSideValueRef2)) ? -1 : 1;

            long stride = 1;
            for (int i = right.ResolvedValue.Value.ToString().Length - 2; i > 0; i--, stride *= 10) ;

            long lastDelta = Math.Abs(matchValue - humanSideValueRef1);

            while (matchValue != humanSideValue) {
                self.ResolvedValue += delta;


                ResetHumanPath();
                humanSideValue = ResolveFor(humanSide);
                long newDelta = Math.Abs(matchValue - humanSideValue);
                delta = 1;
                for (int i = newDelta.ToString().Length - 2; i > 0; i--, delta *= 10);


                lastDelta = newDelta;
                WriteLine(lastDelta - newDelta);
            }

            return self.ResolvedValue;
        }

        void Reset(string rootName) {
            Queue<string> tovisit = new Queue<string>();
            tovisit.Enqueue(rootName);
            while (tovisit.Count > 0) {
                var next = tovisit.Dequeue();
                var mk = monkeys[next];
                if (mk.Left is not null) tovisit.Enqueue(mk.Left);
                if (mk.Right is not null) tovisit.Enqueue(mk.Right);
                mk.Reset();
            }
        }

        Dictionary<string, HashSet<string>> depsCache = new();
        HashSet<string> GetDeps(string rootName) {
            if (depsCache.TryGetValue(rootName, out HashSet<string> deps)) return deps;

            HashSet<string> opset = new(monkeys.Count);
            Queue<string> tovisit = new();
            tovisit.Enqueue(rootName);
            while (tovisit.Count > 0) {
                var next = tovisit.Dequeue();
                var mk = monkeys[next];
                if (mk.Left is not null) tovisit.Enqueue(mk.Left);
                if (mk.Right is not null) tovisit.Enqueue(mk.Right);
                opset.Add(mk.Name);
            }

            depsCache.Add(rootName, opset);
            return opset;
        }

        long ResolveFor(string rootName) {
            Queue<string> tovisit = new();
            foreach (var name in GetDeps(rootName)) { tovisit.Enqueue(name); }

            var root = monkeys[rootName];
            while (root.ResolvedValue is null && tovisit.Count > 0) {
                var next = tovisit.Dequeue();
                var mk = monkeys[next];
                if (mk.ResolvedValue is not null) continue; // skip resolved nodes
                var left = monkeys[mk.Left];
                var right = monkeys[mk.Right];
                if (left.ResolvedValue is null || right.ResolvedValue is null)
                    tovisit.Enqueue(mk.Name);
                else {
                    mk.ResolvedValue = mk.Operation switch {
                        "+" => left.ResolvedValue + right.ResolvedValue,
                        "-" => left.ResolvedValue - right.ResolvedValue,
                        "*" => left.ResolvedValue * right.ResolvedValue,
                        "/" => left.ResolvedValue / right.ResolvedValue,
                        _ => throw new Exception("unknow operator")
                    };
                }
            }
            return root.ResolvedValue ?? throw new Exception($"could not resolve {rootName}");

        }


        class Monkey {
            public string Name;
            public string Left, Right, Operation;
            public long? ResolvedValue;
            public void Reset() {
                if (Left is not null) ResolvedValue = null;
            }

            public override string ToString() {
                return string.Concat(Name, ": ", ResolvedValue?.ToString() ?? "null");
            }
        }


    }
}
