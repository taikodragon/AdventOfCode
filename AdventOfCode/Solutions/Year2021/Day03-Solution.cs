using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day03 : ASolution
    {
        List<string> input;
        int halfSize;
        int numberSize;

        public Day03() : base(03, 2021, "Binary Diagnostic", false)
        {
            input = Input.SplitByNewline(false, true)
                .ToList();
            halfSize = (int)Math.Floor(input.Count / 2.0);
            numberSize = UseDebugInput ? 5 : 12;


        }

        List<int> OneCounts(List<string> currentSet) {
            List<int> position = new List<int>();
            for (int i = 0; i < numberSize; i++) { position.Add(0); }

            foreach (string input in currentSet) {
                for (int i = 0; i < numberSize; i++) {
                    if (input[i] == '1') position[i]++;
                }
            }
            return position;
        }

        protected override string SolvePartOne()
        {
            List<int> position = OneCounts(input);

            halfSize = (int)Math.Floor(input.Count / 2.0);
            string gammaString = string.Empty, epString = string.Empty;
            for (int i = 0; i < numberSize; i++) {
                if (position[i] > halfSize) {
                    gammaString += '1';
                    epString += '0';
                }
                else {
                    gammaString += '0';
                    epString += '1';
                }
            }
            int gamma = Convert.ToInt32(gammaString, 2);
            int ep = Convert.ToInt32(epString, 2);

            return (gamma*ep).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Oxy
            string oxyRatingStr;
            List<string> currentSet = new List<string>(input);
            int atPosition = 0;
            while (currentSet.Count > 1 && atPosition < numberSize) {
                double halfSize = currentSet.Count / 2.0;
                var positions = OneCounts(currentSet);
                char keepMe = ' ';
                if (positions[atPosition] > halfSize || positions[atPosition] == halfSize) {
                    keepMe = '1';
                }
                else {
                    keepMe = '0';
                }
                currentSet.RemoveAll(s => s[atPosition] != keepMe);
                atPosition++;
            }
            oxyRatingStr = currentSet[0];

            string co2Scrub;
            currentSet = new List<string>(input);
            atPosition = 0;
            while (currentSet.Count > 1 && atPosition < numberSize) {
                double halfSize = currentSet.Count / 2.0;
                var positions = OneCounts(currentSet);
                char keepMe = ' ';
                if (positions[atPosition] > halfSize || positions[atPosition] == halfSize) {
                    keepMe = '0';
                }
                else {
                    keepMe = '1';
                }
                currentSet.RemoveAll(s => s[atPosition] != keepMe);
                atPosition++;
            }
            co2Scrub = currentSet[0];

            return (Convert.ToInt32(oxyRatingStr, 2) * Convert.ToInt32(co2Scrub, 2)).ToString();
        }
    }
}
