using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day23 : ASolution
    {
        class CupLinkedList
        {
            public int label;
            public bool removed = false;
            public CupLinkedList next;
            public CupLinkedList prev;
        }
        static void AddAfter(CupLinkedList at, CupLinkedList next) {
            CupLinkedList nextLink = at.next;
            at.next = next;
            next.prev = at;

            if( nextLink != null ) {
                CupLinkedList pos = next.next;
                while( pos != at && pos.next != null ) {
                    if( pos == at ) throw new Exception("You made bad loops where nextLink cannot be rejoined to the chain");
                    pos = pos.next;
                }
                pos.next = nextLink;
                nextLink.prev = pos;
            }
        }
        static void RemoveBetween(CupLinkedList at, CupLinkedList lastExclusive) {
            CupLinkedList oldNext = at.next;
            oldNext.prev = null;
            if( lastExclusive.prev != null ) {
                lastExclusive.prev.next = null;
            }
            at.next = lastExclusive;
            lastExclusive.prev = at;
        }
        static CupLinkedList NthFrom(CupLinkedList start, int nextN) {
            CupLinkedList at = start;
            for(int i = 0; i < nextN && at != null; i++ ) {
                at = at.next;
            }
            return at;
        }
        static void ForEachIn(CupLinkedList start, Action<CupLinkedList, int> work, int iterationLimit) {
            CupLinkedList at = start.next;
            work(start, 0);
            int i = 1;
            while( at != start && at != null && i < iterationLimit) {
                work(at, i);
                at = at.next;
                i++;
            }
        }



        List<int> startingCups = new List<int>();

        public Day23() : base(23, 2020, "Crab Cups", false)
        {
            

            startingCups = Input.Trim().Select(c => int.Parse(c.ToString())).ToList();

        }

        protected override string SolvePartOne()
        {
            var cups = new List<int>(startingCups);
            List<int> removedCups = new List<int>(3);
            int currentCup = 0;
            for(int move = 1; move <= 100; move++ ) {
                //Trace.WriteLine($"-- move {move} --");
                //Trace.WriteLine($"current cup: {cups[currentCup]}");
                //Trace.WriteLine($"cups: {string.Join(' ', cups)}");

                int currentCupValue = cups[currentCup];
                int destCup = cups[currentCup] - 1;
                removedCups.Clear();
                for(int i = (currentCup + 1) % cups.Count, c = 0; c < 3; c++, i %= cups.Count ) {
                    removedCups.Add(cups[i]);
                    cups.RemoveAt(i);
                }
                //Trace.WriteLine($"pick up: {string.Join(' ', removedCups)}");


                int cupsMin = cups.Min();
                while(!cups.Contains(destCup)) {
                    destCup--;
                    if( destCup < cupsMin ) destCup = cups.Max();
                }
                //Trace.WriteLine($"destination: {destCup}");
                
                int destCupIndex = cups.IndexOf(destCup);
                if( destCupIndex + 1 == cups.Count )
                    cups.AddRange(removedCups);
                else
                    cups.InsertRange(destCupIndex + 1, removedCups);

                currentCup = cups.IndexOf(currentCupValue);

                currentCup = (currentCup + 1) % cups.Count;
            }

            string cupOrder = string.Empty;
            int cupOneIndex = cups.IndexOf(1);
            for(int i = (cupOneIndex + 1) % cups.Count; i != cupOneIndex; i = (i+1) % cups.Count ) {
                cupOrder += cups[i].ToString();
            }
            return cupOrder;
        }

        protected override string SolvePartTwo()
        {
            const int tenMillion = 10000000;
            const int oneMillion = 1000000;

            CupLinkedList cupOne = null;
            Dictionary<int, CupLinkedList> labels = new Dictionary<int, CupLinkedList>(oneMillion);
            CupLinkedList shead = new CupLinkedList { label = startingCups[0] };
            labels.Add(shead.label, shead);
            if( shead.label == 1 ) cupOne = shead;

            CupLinkedList at = shead;
            for(int i = 1 ; i < startingCups.Count; i++ ) {
                var newCup = new CupLinkedList { label = startingCups[i] };
                AddAfter(at, newCup);
                labels.Add(newCup.label, newCup);
                at = newCup;
                if( at.label == 1 ) cupOne = at;
            }
            const bool isTracing = false ;

            for(int ln = startingCups.Max() + 1; ln <= (isTracing ? 9 : oneMillion); ln++) {
                var newCup = new CupLinkedList { label = ln };
                AddAfter(at, newCup);
                labels.Add(newCup.label, newCup);
                at = newCup;
            }
            AddAfter(at, shead); // create a loop

            CupLinkedList currentCup = shead, removedCups = null;
            int cupsMin = 1, cupsMax = (isTracing ? 9 : oneMillion);
            for( int move = 1; move <= (isTracing ? 10 : tenMillion); move++ ) {
                //Trace.WriteLineIf(isTracing, $"-- move {move} --");
                //Trace.WriteLineIf(isTracing, $"current cup: {currentCup.label}");
                //Trace.WriteLineIf(isTracing, $"cups: {string.Join(' ', cups)}");

                ForEachIn(currentCup.next, (c, i) => c.removed = true, 3);

                removedCups = currentCup.next;
                RemoveBetween(currentCup, NthFrom(removedCups, 3));

                //Trace.WriteLineIf(isTracing, $"pick up: {removedCups.label}");

                cupsMin = 1;
                while( labels[cupsMin].removed ) {
                    cupsMin++;
                }
                cupsMax = (isTracing ? 9 : oneMillion);
                while( labels[cupsMax].removed ) {
                    cupsMax--;
                }
                CupLinkedList destCup = labels[currentCup.label == 1 ? cupsMax : currentCup.label - 1];


                while( destCup.removed ) {
                    destCup = labels[destCup.label == 1 ? cupsMax : destCup.label - 1];
                }
                //Trace.WriteLineIf(isTracing, $"destination: {destCup.label}");

                AddAfter(destCup, removedCups);
                ForEachIn(removedCups, (c, i) => c.removed = false, 3);

                currentCup = currentCup.next;
                if( currentCup == null ) Debugger.Break();
            }

            long multResult = 1;
            ForEachIn(cupOne.next, (c, i) => multResult *= c.label, 2);
            return multResult.ToString();
        }
    }
}
