using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day05 : ASolution
    {
        string vowels = "aeiou";
        List<string> forbidden = new List<string>() {
            "ab", "cd", "pq", "xy"
        };
        public Day05() : base(05, 2015, "Doesn't He Have Intern-Elves For This?")
        {
            UseDebugInput = false;


        }

        bool ValidString(string val) {
            if( /*num vowels*/ val.Count(c => vowels.Contains(c)) < 3 || 
                /* any forbidden */ forbidden.Any(f => val.Contains(f)) ) {
                return false;
            }
            bool hasRepeating = false;
            for( int i = 0; i < val.Length - 1; ++i ) {
                hasRepeating = hasRepeating || val[i] == val[i + 1];
            }
            return hasRepeating;
        }

        protected override string SolvePartOne()
        {
            List<string> lines = Input.SplitByNewline();
            return lines.Count(s => ValidString(s)).ToString();
        }

        bool BetterValidString(string val) {

            bool hasSpacedRepeating = false;
            for(int i = 2; !hasSpacedRepeating && i < val.Length; ++i ) {
                hasSpacedRepeating = val[i - 2] == val[i];
            }
            if( !hasSpacedRepeating ) return false;

            Dictionary<char, List<int>> charPositions = new Dictionary<char, List<int>>();
            for(int i = val.Length - 1; i >= 0; --i ) {
                char ch = val[i];
                List<int> posList;
                if( !charPositions.TryGetValue(ch, out posList) ) {
                    posList = charPositions[ch] = new List<int>();
                }
                posList.Add(i);
            }
            for(int i = 0; i < val.Length - 1; ++i) {
                char ich = val[i];
                foreach(int otherPos in charPositions[ich] ) {
                    if( otherPos == i || otherPos + 1 == val.Length ) continue; // skip me or where a pair cannnot exist
                    if( val[i + 1] == val[otherPos + 1] ) {
                        return !(i + 1 == otherPos || i - 1 == otherPos); // is pair at i adjacent to pair at otherPos
                    }
                }
            }
            return false;
        }

        protected override string SolvePartTwo()
        {
            List<string> lines = Input.SplitByNewline();
            return lines.Count(s => BetterValidString(s)).ToString();
        }
    }
}
