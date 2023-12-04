using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 04, "Scratchcards")]
class Day04 : ASolution
{
    class Scratchcard
    {
        public int Id;
        public HashSet<int> winning = new();
        public List<int> yourNumbers = new();
        public int WorthPoints;
        public int WinCount;
    }


    public Day04() : base(false) {

    }

    List<Scratchcard> cards = new();
    protected override void ParseInput() {
        foreach (string line in Input.SplitByNewline()) {
            var numbers = line.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            Scratchcard card = new() {
                Id = int.Parse(numbers[1])
            };
            bool foundSplit = false;
            foreach (var numStr in numbers[2..]) {
                if (numStr == "|") {
                    foundSplit = true;
                    continue;
                }

                if (foundSplit) card.yourNumbers.Add(int.Parse(numStr));
                else card.winning.Add(int.Parse(numStr));
            }
            int winCount = card.WinCount = card.yourNumbers.Intersect(card.winning).Count();
            card.WorthPoints = winCount > 0 ? (int)Math.Pow(2, winCount - 1) : 0;
            cards.Add(card);
        }
    }

    protected override object SolvePartOneRaw() {
        return cards.Sum(c => c.WorthPoints);
    }

    protected override object SolvePartTwoRaw() {
        Dictionary<int, int> copyCount = new();

        cards.ForEach(c => copyCount.Add(c.Id, 1));

        int visited = 0;
        foreach (var card in cards) {
            int myCopies = copyCount[card.Id];
            visited += myCopies;
            // Add copies
            for (int i = 1; i <= card.WinCount; i++) {
                copyCount[card.Id + i] += myCopies;
            }
        }

        return visited;
    }
}
