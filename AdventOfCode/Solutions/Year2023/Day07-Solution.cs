using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 07, "Camel Cards")]
class Day07 : ASolution {
    const int FiveKind = 1,
        FourKind = 2,
        FullHouse = 3,
        ThreeKind = 4,
        TwoPair = 5,
        OnePair = 6,
        HighCard = 7;

    class Hand {
        static Dictionary<char, int> cardsToScore = new() {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9',  9 },
            { '8',  8 },
            { '7',  7 },
            { '6',  6 },
            { '5',  5 },
            { '4',  4 },
            { '3',  3 },
            { '2',  2 }
        };
        public string Text;
        public List<int> Values = new();
        public int Bid;
        public int WinCat;

        public Hand(string text, int bid) {
            Values.AddRange(text.Select(c => cardsToScore[c]));
            Text = text;
            Bid = bid;

            Dictionary<int, int> typeCounts = new();
            foreach (int val in Values) {
                typeCounts[val] = typeCounts.GetValueOrDefault(val, 0) + 1;
            }

            List<int> counts = new(typeCounts.Values);
            // Five of a kind
            if (counts.Contains(5)) WinCat = FiveKind;
            // Four of a kind
            else if (counts.Contains(4)) WinCat = FourKind;
            else if (counts.Contains(3)) {
                // Full House
                if (counts.Contains(2)) WinCat = FullHouse;
                // Three of a kind
                else WinCat = ThreeKind;
            }
            else {
                var pairCount = counts.Count(c => c == 2);
                if (pairCount == 2) WinCat = TwoPair; // Two Pair
                else if (pairCount == 1) WinCat = OnePair; // One Pair
                else WinCat = HighCard; // High Card
            }
        }
    }


    public Day07() : base(false)
    {
            
    }

    List<Hand> hands;
    List<Hand2> p2hands;
    protected override void ParseInput()
    {
        hands = Input.SplitByNewline(false, true)
            .Select(l => l.Split(' '))
            .Select(p => new Hand(p[0], int.Parse(p[1])))
            .ToList();
        p2hands = Input.SplitByNewline(false, true)
            .Select(l => l.Split(' '))
            .Select(p => new Hand2(p[0], int.Parse(p[1])))
            .ToList();

    }

    int CompareHands(Hand lhs, Hand rhs) {
        int catResult = -1 * lhs.WinCat.CompareTo(rhs.WinCat); // reserve sort so low hands are first
        if( catResult == 0 ) {
            int cardResult = 0;
            for(int i = 0; i < lhs.Values.Count && cardResult == 0; i++) {
                cardResult = lhs.Values[i].CompareTo(rhs.Values[i]);
            }
            return cardResult;
        }
        return catResult;
    }

    protected override object SolvePartOneRaw()
    {
        List<Hand> myHands = new(hands);
        myHands.Sort(CompareHands);

        long score = 0;
        for(int i = 0; i < myHands.Count; i++) {
            score += myHands[i].Bid * (i + 1);
        }

        return score;
    }


    class Hand2
    {
        static Dictionary<char, int> cardsToScore = new() {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'T', 10 },
            { '9',  9 },
            { '8',  8 },
            { '7',  7 },
            { '6',  6 },
            { '5',  5 },
            { '4',  4 },
            { '3',  3 },
            { '2',  2 },
            { 'J',  1 }
        };
        public string Text;
        public List<int> Values = new();
        public int Bid;
        public int WinCat;

        public Hand2(string text, int bid) {
            Values.AddRange(text.Select(c => cardsToScore[c]));
            Text = text;
            Bid = bid;

            Dictionary<char, int> typeCounts = new();
            foreach (char val in text) {
                typeCounts[val] = typeCounts.GetValueOrDefault(val, 0) + 1;
            }

            // Five of a kind
            int jokerCount = typeCounts.GetValueOrDefault('J', 0);
            if (jokerCount > 0) typeCounts.Remove('J'); // Remove jokers from counts to avoid double count

            List<int> counts = new(typeCounts.Values);
            int countMax = counts.Count > 0 ? counts.Max() : 0;
            if (countMax + jokerCount == 5) WinCat = FiveKind;
            // Four of a kind
            else if (countMax + jokerCount == 4) WinCat = FourKind;
            else {
                var maxType = typeCounts.First(kv => kv.Value == countMax);
                typeCounts.Remove(maxType.Key);
                counts.Clear(); counts.AddRange(typeCounts.Values);

                switch (maxType.Value) {
                    case 3:
                        if (counts.Contains(2)) // natural full house
                            WinCat = FullHouse;
                        else {
                            switch (jokerCount) {
                                case 0: WinCat = ThreeKind; break;
                                case 1: throw new Exception("FourKind");
                                case 2: throw new Exception("FiveKind");
                                default: throw new Exception("WTF");
                            }
                        }
                        break;
                    case 2:
                        switch (jokerCount) {
                            case 0: // No Jokers
                                if (counts.Contains(2)) WinCat = TwoPair;
                                else WinCat = OnePair;
                                break;
                            case 1:
                                if (counts.Contains(2)) WinCat = FullHouse;
                                else WinCat = ThreeKind;
                                break;
                            case 2: throw new Exception("FourKind");
                            case 3: throw new Exception("FiveKind");
                            default: throw new Exception("WTF");
                        }
                        break;
                    case 1:
                        switch (jokerCount) {
                            case 0: WinCat = HighCard; break;
                            case 1: WinCat = OnePair; break;
                            case 2: WinCat = ThreeKind; break;
                            case 3: throw new Exception("FourKind");
                            case 5: throw new Exception("FiveKind");
                            default: throw new Exception("WTF");
                        }
                        break;
                }
            }
        }
    }

    int CompareHands(Hand2 lhs, Hand2 rhs) {
        int catResult = -1 * lhs.WinCat.CompareTo(rhs.WinCat); // reserve sort so low hands are first
        if (catResult == 0) {
            int cardResult = 0;
            for (int i = 0; i < lhs.Values.Count && cardResult == 0; i++) {
                cardResult = lhs.Values[i].CompareTo(rhs.Values[i]);
            }
            return cardResult;
        }
        return catResult;
    }

    protected override object SolvePartTwoRaw()
    {
        List<Hand2> myHands = new(p2hands);
        myHands.Sort(CompareHands);

        long score = 0;
        for (int i = 0; i < myHands.Count; i++) {
            score += myHands[i].Bid * (i + 1);
        }

        return score;
    }
}
