using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{

    class Day19 : ASolution
    {
        Dictionary<int, char> roots = new Dictionary<int, char>();
        Dictionary<int, List<int[]>> refRules = new Dictionary<int, List<int[]>>();
        List<string> lines;
        public Day19() : base(19, 2020, "Monster Messages")
        {
            UseDebugInput = false;
        }

        void ParseRulesV1(string[] groups) {

            foreach( string line in groups[0].SplitByNewline(false) ) {
                string[] parts = line.Split(':').Select(s => s.Trim()).ToArray();
                int ruleId = int.Parse(parts[0]);
                if( parts[1][0] == '"' ) {
                    roots.Add(ruleId, parts[1].Trim('"')[0]);
                    continue;
                }
                // else
                List<int[]> ruleSubRules;
                if( !refRules.TryGetValue(ruleId, out ruleSubRules) ) {
                    refRules.Add(ruleId, ruleSubRules = new List<int[]>());
                }
                foreach( string subRule in parts[1].Split('|') ) {
                    ruleSubRules.Add(
                        subRule
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray()
                            );
                }
            }

            lines = groups[1].SplitByNewline(false);

        }

        List<int> EvaluateRule(int ruleId, string line, int at, int depth = 0, bool debugging = false) {
            if( at >= line.Length ) {
                if( debugging ) Trace.WriteLine($"Broken rule {ruleId} -- {line}");
                return new List<int> ();
            }
            if( roots.TryGetValue(ruleId, out char matchAt) ) {
                List<int> result = new List<int>();
                if( line[at] == matchAt )
                    result.Add(at + 1);
                return result;
            }
            List<int[]> subRules = refRules[ruleId];
            List<int> searchResult = new List<int>();
            foreach(int[] ruleSet in subRules) {
                List<int> runRulesAt = new List<int> { at };
                foreach(int rule in ruleSet) {
                    List<int> newRunRulesAt = new List<int>();
                    foreach(int nowAt in runRulesAt) {
                        var newAts = EvaluateRule(rule, line, nowAt, depth + 1, debugging);
                        newRunRulesAt.AddRange(newAts);
                    }
                    if( newRunRulesAt.Count == 0 ) {
                        runRulesAt.Clear();
                        break;
                    }
                    runRulesAt = newRunRulesAt;
                }
                if( runRulesAt.Count > 0 ) {
                    searchResult.AddRange(runRulesAt);
                }
            }
            if( debugging && searchResult.Count > 0 ) {
                Trace.IndentLevel = depth;
                Trace.WriteLine($"{ruleId} -- {searchResult.Count} -- " + string.Join(" | ", searchResult));
            }

            if( depth == 0 && searchResult.Count > 0) {
                if( searchResult.Any(rat => rat == line.Length) )
                    return searchResult;
                else
                    return new List<int>();
            }
            return searchResult;
        }

        protected override string SolvePartOne() {
            string[] groups = Input.Split("\n\n");
            ParseRulesV1(groups);

            return lines.Count(s => {
                return EvaluateRule(0, s, 0).Count > 0;
            }).ToString();
        }

        protected override string SolvePartTwo()
        {
            roots = new Dictionary<int, char>();
            refRules = new Dictionary<int, List<int[]>>();

            string[] groups = Input.Split("\n\n");
            groups[0] = groups[0]
                .Replace("\n8: 42\n", "\n8: 42 8 | 42\n")
                .Replace("\n11: 42 31\n", "\n11: 42 31 | 42 11 31\n");
            ParseRulesV1(groups);

            string debugMe = "disabled";
            int sum = 0;
            foreach(string line in lines) {
                bool result = EvaluateRule(0, line, 0, 0, line == debugMe).Count > 0;
                if( result ) sum++;
            }
            return sum.ToString();
        }
    }
}
