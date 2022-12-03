using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Appointments;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 02, "Rock Paper Scissors")]
class Day02 : ASolution
{
    const string rock = "rock", paper = "paper", scissors = "scissors";
    static readonly Dictionary<char, string> map = new() {
        { 'A', rock},
        { 'B', paper},
        { 'C', scissors},
        { 'X', rock},
        { 'Y', paper},
        { 'Z', scissors},
    };
    static readonly Dictionary<string, int> handScoreMap = new() {
        { rock, 1 }, { paper, 2 }, { scissors, 3 }
    };
    static readonly Dictionary<string, string> winsAgainst = new() {
        { paper, rock }, { scissors, paper }, { rock, scissors }
    };
    enum OutcomeType {
        Lose = 0,
        Draw = 3,
        Win = 6
    }
    class Round {
        public Round(string other, string you) {
            Other = other;
            You = you;
            
            if (Other == You) Outcome = OutcomeType.Draw;
            else if (winsAgainst[You] == Other) Outcome = OutcomeType.Win;
            else Outcome = OutcomeType.Lose;

            Score = handScoreMap[You] + (int)Outcome;
        }
        public string Other { get; set; }
        public string You { get; set; }
        public OutcomeType Outcome { get; set; }
        public int Score { get; set; }
    }

    public Day02() : base(false)
    {
    }

    string[][] parsed;
    protected override void ParseInput() {
        parsed = Input.SplitByNewline(false, true)
            .Select(s => s.Split(' '))
            .ToArray();
    }
    protected override string SolvePartOne()
    {
        List<Round> rounds;
        rounds = parsed
            .Select(s => s.Select(c => map[c[0]]).ToArray())
            .Select(p => new Round(p[0], p[1]))
            .ToList();
        return rounds.Sum(r => r.Score).ToString();
    }

    protected override string SolvePartTwo()
    {
    
        List<Round> rounds = new();
        foreach(var pair in parsed) {
            string other = map[pair[0][0]];
            Round round;
            switch(pair[1]) {
                case "X": // Lose
                    round = new Round(other, winsAgainst[other]);
                    break;
                case "Y": // Draw
                    round = new Round(other, other);
                    break;
                case "Z": // Win
                    string me = winsAgainst.FirstOrDefault(kv => kv.Value == other).Key;
                    round = new Round(other, me);
                    break;
                default: throw new Exception("Unknown outcome");
            }
            rounds.Add(round);
        }

        return rounds.Sum(r => r.Score).ToString();

    }
}
