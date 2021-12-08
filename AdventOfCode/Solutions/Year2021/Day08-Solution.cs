using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day08 : ASolution
    {
        public Day08() : base(08, 2021, "Seven Segment Search", false)
        {
        }

        protected override string SolvePartOne()
        {
            // 1 4 7 8
            int counter = 0;
            List<int> countLengths = new() { 2, 3, 4, 7 };
            foreach(string line in Input.SplitByNewline(false, true)) {
                List<string>[] section = line.Split('|').Select(s => s.Split(' ').ToList()).ToArray();

                counter += section[1].Count(s => countLengths.Contains(s.Length));
            }

            return counter.ToString();
        }


        class Display
        {
            readonly Dictionary<string, char> finalMap = new();
            readonly Dictionary<int, List<char>> segments = new(7);
            /*  0000
             * 1    2
             * 1    2
             *  3333
             * 4    5
             * 4    5
             *  6666
             * */

            public Display() {
                for (int i = 0; i < 7; i++) {
                    segments[i] = new List<char>(7) {
                        'a','b','c','d','e','f','g'
                    };
                }
            }
            List<string> fiveSegment = new();
            List<string> sixSegment = new();

            public void PushDigit(string value) {
                switch(value.Length) {
                    case 2:
                        // remove known left segments from others
                        foreach(int seg in new int[] { 0, 1, 3, 4, 6 }) {
                            segments[seg].RemoveAll(c => value.Contains(c));
                        }
                        // remove others from known left segments
                        segments[2].RemoveAll(c => !value.Contains(c));
                        segments[5].RemoveAll(c => !value.Contains(c));
                        AddToFinalMap(value, '1');
                        break;
                    case 3:
                        // remove known segments from others
                        foreach (int seg in new int[] { 1, 3, 4, 6 }) {
                            segments[seg].RemoveAll(c => value.Contains(c));
                        }
                        // remove others from known segments
                        foreach (int seg in new int[] { 0, 2, 5 }) {
                            segments[seg].RemoveAll(c => !value.Contains(c));
                        }
                        AddToFinalMap(value, '7');
                        break;
                    case 4:
                        // remove known segments from others
                        foreach (int seg in new int[] { 0, 4, 6 }) {
                            segments[seg].RemoveAll(c => value.Contains(c));
                        }
                        // remove others from known segments
                        foreach (int seg in new int[] { 1, 2, 3, 5 }) {
                            segments[seg].RemoveAll(c => !value.Contains(c));
                        }
                        AddToFinalMap(value, '4');
                        break;
                    case 5:
                        fiveSegment.Add(value);
                        return;
                    case 6:
                        sixSegment.Add(value);
                        return;
                    case 7:
                        AddToFinalMap(value, '8');
                        return;
                    default:
                        return;
                }
            }

            void AddToFinalMap(string value, char num) {
                finalMap[new string(value.OrderBy(c => c).ToArray())] = num;
            }
            bool CheckPositions(string value, params int[] positions) {
                return positions.All(p => segments[p].Any(c => value.Contains(c)));
            }
            public void Solve() {
                foreach(var op in sixSegment) {
                    // 0 - 0, 1, 2, 4, 5, 6
                    if (CheckPositions(op, 0, 1, 2, 4, 5, 6) && segments[3].Count(c => op.Contains(c)) == 1) {
                        AddToFinalMap(op, '0');
                        sixSegment.Remove(op);
                        segments[3].RemoveAll(c => op.Contains(c));
                        segments[1].RemoveAll(c => !op.Contains(c));
                        break;
                    }
                }

                foreach (var op in fiveSegment) {
                    // 5 - 0, 1, 3, 5, 6
                    if (CheckPositions(op, 0, 1, 3, 5, 6) ) {
                        AddToFinalMap(op, '5');
                        fiveSegment.Remove(op);
                        segments[5].RemoveAll(c => !op.Contains(c));
                        segments[6].RemoveAll(c => !op.Contains(c));
                        segments[2].RemoveAll(c => segments[5].Contains(c));
                        segments[4].RemoveAll(c => segments[6].Contains(c));
                        break;
                    }
                }

                foreach (var op in fiveSegment) {
                    // 3 - 0, 2, 3, 5, 6
                    if (CheckPositions(op, 0, 2, 3, 5, 6)) { AddToFinalMap(op, '3'); }
                    // 2 - 0, 2, 3, 4, 6
                    else if(CheckPositions(op, 0, 2, 3, 4, 6)) { AddToFinalMap(op, '2'); }
                }

                foreach (var op in sixSegment) {
                    // 9 - 0, 1, 2, 3, 5, 6
                    if (CheckPositions(op, 0, 1, 2, 3, 5, 6)) { AddToFinalMap(op, '9'); }
                    // 6 - 0, 1, 3, 4, 5, 6
                    else if (CheckPositions(op, 0, 1, 3, 4, 5, 6)) { AddToFinalMap(op, '6'); }
                }
            }

            public int GetOutput(List<string> digits) {
                return int.Parse(
                    string.Concat(
                        digits.Select(digit => finalMap[new string(digit.OrderBy(c => c).ToArray())])
                    )
                );
            }
        }



        protected override string SolvePartTwo()
        {
            long sum = 0;
            foreach (string line in Input.SplitByNewline(false, true)) {
                List<string>[] section = line.Split('|')
                    .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .ToArray();

                var disp = new Display();
                foreach(var sig in section[0]) {
                    disp.PushDigit(sig);
                }
                disp.Solve();
                sum += disp.GetOutput(section[1]);
            }

            return sum.ToString();
        }
    }
}
