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
            foreach(string line in Input.SplitByNewline(false, true)) {
                List<string>[] section = line.Split('|').Select(s => s.Split(' ').ToList()).ToArray();
                List<string> seq = section[0];
                List<string> digits = section[1];

                //Dictionary<int,string> map = new Dictionary<int,string>();
                counter += digits.Count(s => s.Length == 2);
                counter += digits.Count(s => s.Length == 4);
                counter += digits.Count(s => s.Length == 3);
                counter += digits.Count(s => s.Length == 7);

                //counter += /*digits.Count(s => map.ContainsValue(s))*/;
            }

            return counter.ToString();
        }


        class Display
        {
            Dictionary<string, char> finalMap = new Dictionary<string, char>();
            Dictionary<int, List<char>> segments = new Dictionary<int, List<char>>(7);
            /*  0000
             * 1    2
             * 1    2
             *  3333
             * 4    5
             * 4    5
             *  6666
             * */
            //char[] finalSegments = new char[7];

            public Display() {
                for (int i = 0; i < 7; i++) {
                    segments[i] = new List<char>(7) {
                        'a',
                        'b',
                        'c',
                        'd',
                        'e',
                        'f',
                        'g'
                    };
                    //finalSegments[i] = ' ';
                }
            }
            List<string> fiveSegment = new List<string>();
            List<string> sixSegment = new List<string>();

            public void PushDigit(string value) {
                switch(value.Length) {
                    case 2:
                        // remove known left segments from others
                        segments[0].RemoveAll(c => value.Contains(c));
                        segments[1].RemoveAll(c => value.Contains(c));
                        segments[3].RemoveAll(c => value.Contains(c));
                        segments[4].RemoveAll(c => value.Contains(c));
                        segments[6].RemoveAll(c => value.Contains(c));
                        // remove others from known left segments
                        segments[2].RemoveAll(c => !value.Contains(c));
                        segments[5].RemoveAll(c => !value.Contains(c));
                        AddToFinalMap(value, '1');
                        break;
                    case 3:
                        // remove known segments from others
                        segments[1].RemoveAll(c => value.Contains(c));
                        segments[3].RemoveAll(c => value.Contains(c));
                        segments[4].RemoveAll(c => value.Contains(c));
                        segments[6].RemoveAll(c => value.Contains(c));

                        // remove others from known segments
                        segments[0].RemoveAll(c => !value.Contains(c));
                        segments[2].RemoveAll(c => !value.Contains(c));
                        segments[5].RemoveAll(c => !value.Contains(c));
                        AddToFinalMap(value, '7');
                        break;
                    case 4:
                        // remove known segments from others
                        segments[0].RemoveAll(c => value.Contains(c));
                        segments[4].RemoveAll(c => value.Contains(c));
                        segments[6].RemoveAll(c => value.Contains(c));
                        // remove others from known segments
                        segments[1].RemoveAll(c => !value.Contains(c));
                        segments[2].RemoveAll(c => !value.Contains(c));
                        segments[3].RemoveAll(c => !value.Contains(c));
                        segments[5].RemoveAll(c => !value.Contains(c));
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
                    default: return;
                }
                //foreach(var kv in segments) {
                //    if (kv.Value.Count == 0) continue;
                //    if (kv.Value.Count == 1) {
                //        finalSegments[kv.Key] = kv.Value[0];
                //        foreach(var kv2 in segments) {
                //            if (kv.Key == kv2.Key) continue;
                //            kv2.Value.Remove(kv.Value[0]);
                //        }
                //    }
                //}
            }

            void AddToFinalMap(string value, char num) {
                finalMap[new string(value.OrderBy(c => c).ToArray())] = num;
            }
            bool CheckPositions(string value, params int[] positions) {
                foreach (var position in positions) {
                    var positionOptions = segments[position];
                    if (!segments[position].Any(c => value.Contains(c))) return false;
                }
                return true;
            }
            public void Solve() {
                string selected = null;
                foreach(var op in sixSegment) {
                    // 0 - 0, 1, 2, 4, 5, 6
                    if (CheckPositions(op, 0, 1, 2, 4, 5, 6) && segments[3].Count(c => op.Contains(c)) == 1) {
                        selected = op;
                        segments[3].RemoveAll(c => op.Contains(c));
                        segments[1].RemoveAll(c => !op.Contains(c));
                        break;
                    }
                }
                if (selected == null) { throw new Exception("boo"); }
                AddToFinalMap(selected, '0');
                sixSegment.Remove(selected);
                selected = null;

                foreach (var op in fiveSegment) {
                    // 5 - 0, 1, 3, 5, 6
                    if (CheckPositions(op, 0, 1, 3, 5, 6) ) {
                        selected = op;
                        segments[5].RemoveAll(c => !op.Contains(c));
                        segments[6].RemoveAll(c => !op.Contains(c));
                        segments[2].RemoveAll(c => segments[5].Contains(c));
                        segments[4].RemoveAll(c => segments[6].Contains(c));
                        break;
                    }
                }
                if (selected == null) { throw new Exception("boo"); }
                AddToFinalMap(selected, '5');
                fiveSegment.Remove(selected);
                selected = null;

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
                string num = string.Empty;
                foreach (var digit in digits) {
                    var compDigit = new string(digit.OrderBy(c => c).ToArray());
                    num += finalMap[compDigit];
                }
                return int.Parse(num);
            }
        }



        protected override string SolvePartTwo()
        {
            long sum = 0;
            foreach (string line in Input.SplitByNewline(false, true)) {
                List<string>[] section = line.Split('|')
                    .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .ToArray();
                List<string> seq = section[0];
                List<string> digits = section[1];

                var disp = new Display();
                foreach(var sig in seq) {
                    disp.PushDigit(sig);
                }
                disp.Solve();
                sum += disp.GetOutput(digits);
            }



            return sum.ToString();
        }
    }
}
