using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{

    class Day06 : ASolution
    {
        Dictionary<ulong, ulong> fishes = new(8);
        Dictionary<ulong, ulong> nextFishes = new(8);
        int p1Days = 0;


        public Day06() : base(06, 2021, "Lanternfish", false)
        {
            p1Days = UseDebugInput ? 18 : 80;
        }
        void ParseFish() {
            fishes.Clear();
            foreach (ulong timer in Input.Trim().Split(',').Select(ulong.Parse)) {
                if (fishes.ContainsKey(timer)) {
                    fishes[timer]++;
                }
                else {
                    fishes[timer] = 1;
                }
            }
        }
        void SimDay() {
            nextFishes.Clear();
            foreach(var kv in fishes) {
                if( kv.Key > 0 ) {
                    nextFishes[kv.Key - 1] = nextFishes.GetValueOrDefault(kv.Key - 1, 0ul) + kv.Value; // decrement timer
                } else {
                    nextFishes[6] = nextFishes.GetValueOrDefault(6ul, 0ul) + kv.Value; // reset spawning fish
                    nextFishes[8] = nextFishes.GetValueOrDefault(8ul, 0ul) + kv.Value; // new fish
                }
            }
            var oldFish = fishes;
            fishes = nextFishes;
            nextFishes = oldFish;
            //Trace.WriteLine(string.Join(' ', fishes.OrderBy(kv => kv.Key).Select(kv => kv.ToString())));
        }

        protected override string SolvePartOne()
        {
            ParseFish();
            for (int days = 0; days < p1Days; days++) {
                SimDay();
            }

            ulong sum = 0;
            foreach(var kv in fishes) {  sum += kv.Value; }
            return sum.ToString();
        }

        protected override string SolvePartTwo()
        {
            ParseFish();
            for (int days = 0; days < 256; days++) {
                SimDay();
            }

            ulong sum = 0;
            foreach (var kv in fishes) { sum += kv.Value; }
            return sum.ToString();
        }
    }
}
