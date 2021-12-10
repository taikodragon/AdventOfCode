using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day10 : ASolution
    {
        static Dictionary<char, int> scoresp1 = new() {
            { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 }
        };
        static Dictionary<char, int> scoresp2 = new() {
            { ')', 1 }, { ']', 2 }, { '}', 3 }, { '>', 4 }
        };
        static Dictionary<char, char> pairs = new() {
            { '(', ')' }, { '[', ']' }, { '{', '}' }, { '<', '>' }
        };

        List<long> p2LineScores = new();

        public Day10() : base(10, 2021, "Syntax Scoring", false)
        {
        }

        protected override string SolvePartOne() {
            int score = 0;
            Stack<char> openChunks = new Stack<char>();
            foreach(string line in Input.SplitByNewline()) {
                bool skip = false;
                foreach (char c in line) {
                    if (skip) break;
                    if( pairs.ContainsKey(c) ) openChunks.Push(c);
                    else if( scoresp1.ContainsKey(c) ) {
                        skip = pairs[openChunks.Peek()] != c;
                        if (skip) { score += scoresp1[c]; }
                        openChunks.Pop();
                    }
                }
                if (!skip) {
                    long p2score = 0;
                    while (openChunks.Count > 0) {
                        p2score = p2score * 5 + scoresp2[pairs[openChunks.Pop()]];
                    }
                    p2LineScores.Add(p2score);
                }
                openChunks.Clear();
            }
            return score.ToString();
        }

        protected override string SolvePartTwo()
        {
            p2LineScores.Sort();
            return p2LineScores[p2LineScores.Count / 2].ToString();
        }
    }
}
