using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day10 : ASolution
    {
        Dictionary<char, int> scoresp1 = new() {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 }
        };
        List<string> incompleteLines = new();
        Dictionary<char, int> scoresp2 = new() {
            { ')', 1 },
            { ']', 2 },
            { '}', 3 },
            { '>', 4 }
        };
        Dictionary<char, char> pairs = new() {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };

        public Day10() : base(10, 2021, "Syntax Scoring", false)
        {
            
        }



        protected override string SolvePartOne() {
            int score = 0;
            Stack<char> openChunks = new Stack<char>();
            bool IsMatchAndScore(char c, char matching) {
                if (openChunks.Peek() != matching) {
                    score += scoresp1[c];
                    return false;
                }
                return true;
            }
            foreach(string line in Input.SplitByNewline()) {
                bool skip = false;
                foreach (char c in line) {
                    if (skip) break;
                    switch (c) {
                        case '(':
                        case '[':
                        case '{':
                        case '<':
                            openChunks.Push(c);
                            break;
                        case ')':
                            skip = !IsMatchAndScore(c, '(');
                            openChunks.Pop();
                            break;
                        case ']':
                            skip = !IsMatchAndScore(c, '[');
                            openChunks.Pop();
                            break;
                        case '}':
                            skip = !IsMatchAndScore(c, '{');
                            openChunks.Pop();
                            break;
                        case '>':
                            skip = !IsMatchAndScore(c, '<');
                            openChunks.Pop();
                            break;
                    }
                }
                if (!skip) incompleteLines.Add(line);
                openChunks.Clear();
            }
            return score.ToString();
        }

        protected override string SolvePartTwo()
        {
            List<long> scores = new();
            Stack<char> openChunks = new Stack<char>();
            foreach (string line in incompleteLines) {
                foreach (char c in line) {
                    switch (c) {
                        case '(':
                        case '[':
                        case '{':
                        case '<':
                            openChunks.Push(c);
                            break;
                        case ')':
                            openChunks.Pop();
                            break;
                        case ']':
                            openChunks.Pop();
                            break;
                        case '}':
                            openChunks.Pop();
                            break;
                        case '>':
                            openChunks.Pop();
                            break;
                    }
                }
                long score = 0;
                while (openChunks.Count > 0) {
                    score = score * 5 + scoresp2[pairs[openChunks.Pop()]];
                }
                scores.Add(score);
                openChunks.Clear();
            }
            scores.Sort();
            return scores[scores.Count / 2].ToString();
        }
    }
}
