using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{
    [DayInfo(2015, 15, "Science for Hungry People")]
    class Day15 : ASolution
    {
        class Ingr
        {
            public string Name { get; set; }
            public int Capacity { get; set; }
            public int Durability { get; set; }
            public int Flavor { get; set; }
            public int Texture { get; set; }
            public int Calories { get; set; }

        }

        List<Ingr> ingrs = new List<Ingr>();
        public Day15() : base(false)
        {
            

            ingrs = Input.SplitByNewline(false, true)
                .Select(s => s.Split(':').Select(ss => ss.Trim()).ToArray())
                .Select(p => (new Ingr { Name = p[0] }, p[1].Split(',').Select(ss => ss.Trim().Split(' ')).ToArray()))
                .Select(p => {
                    var ingr = p.Item1;
                    foreach( var prm in p.Item2 ) {
                        int v = int.Parse(prm[1]);
                        switch( prm[0] ) {
                            case "capacity": ingr.Capacity = v; break;
                            case "durability": ingr.Durability = v; break;
                            case "flavor": ingr.Flavor = v; break;
                            case "texture": ingr.Texture = v; break;
                            case "calories": ingr.Calories = v; break;
                        }
                    }
                    return ingr;
                })
                .ToList();
        }

        public static byte Sum(byte[] arr) {
            byte sum = 0;
            foreach( byte b in arr ) sum += b;
            return sum;
        }
        static byte SumIn(byte[] arr, int[] idx) {
            byte sum = 0;
            foreach(int i in idx) { sum += arr[i]; }
            return sum;
        }
        static void Spill(byte[] amt, byte startAt, byte max, int[] distIdx) {
            int distIdxLen = distIdx.Length;
            foreach( int i in distIdx ) { amt[i] = 0; };
            for( byte s = startAt, c = 0; s < max; s++, c++ ) {
                amt[distIdx[c % distIdxLen]]++;
            }
        }

        protected override string SolvePartOne()
        {
            const byte recpMax = 100;
            byte[] amounts = new byte[ingrs.Count];
            int amtLen = amounts.Length;
            int ingrMax = recpMax - amounts.Length + 1;
            int i;
            int[] scores = new int[4];

            List<int[]> incColumns = new List<int[]>();


            {
                int[] idxes = Enumerable.Range(0, amtLen).ToArray();
                List<int> perm = new List<int>(amtLen);
                for(i = 1; i < amtLen ; i++) {
                    for(int j = 0; j < amtLen; j++) {
                        perm.Clear();
                        for(int k = 0; k < i; k++) {
                            perm.Add(idxes[(j + k) % amtLen]);
                        }
                        incColumns.Add(perm.ToArray());
                    }
                }
            }

            ulong maxScore = 0, iterations = 0;

            foreach(var frozen in incColumns) {
                int[] nonFrozen = Enumerable.Range(0, amtLen).Where(idx => !frozen.Contains(idx)).ToArray();

                int lastColumn = frozen[frozen.Length - 1];

                Array.Fill<byte>(amounts, 1);
                bool didRollLastColumn = false;
                while(!didRollLastColumn) {
                    iterations++;
                    Spill(amounts, SumIn(amounts, frozen), recpMax, nonFrozen);

                    //AssertFilled();

                    Array.Fill(scores, 0);
                    for(i = 0; i < amtLen; i++) {
                        scores[0] += amounts[i] * ingrs[i].Capacity;
                        scores[1] += amounts[i] * ingrs[i].Durability;
                        scores[2] += amounts[i] * ingrs[i].Flavor;
                        scores[3] += amounts[i] * ingrs[i].Texture;
                    }
                    ulong score = 1;
                    foreach( int prm in scores ) score *= (ulong)Math.Max(prm, 0);
                    if( score > maxScore ) maxScore = score;


                    for(i = 0; i < frozen.Length; i++) {
                        int idx = frozen[i];
                        amounts[idx]++;
                        int max = recpMax - nonFrozen.Length;
                        for( int j = i + 1; j < frozen.Length; j++ )
                            max -= amounts[frozen[j]];
                        
                        if( amounts[idx] < max ) break;
                        else if( idx == lastColumn ) didRollLastColumn = true;
                        else amounts[idx] = 1;
                    }
                }
            }


            Trace.WriteLine($"Total iterations: {iterations}");
            return maxScore.ToString();
        }

        protected override string SolvePartTwo()
        {
            const byte recpMax = 100;
            byte[] amounts = new byte[ingrs.Count];
            int amtLen = amounts.Length;
            int ingrMax = recpMax - amounts.Length + 1;
            int i;
            int[] scores = new int[5];

            List<int[]> incColumns = new List<int[]>();


            {
                int[] idxes = Enumerable.Range(0, amtLen).ToArray();
                List<int> perm = new List<int>(amtLen);
                for( i = 1; i < amtLen; i++ ) {
                    for( int j = 0; j < amtLen; j++ ) {
                        perm.Clear();
                        for( int k = 0; k < i; k++ ) {
                            perm.Add(idxes[(j + k) % amtLen]);
                        }
                        incColumns.Add(perm.ToArray());
                    }
                }
            }

            ulong maxScore = 0;

            foreach( var frozen in incColumns ) {
                int[] nonFrozen = Enumerable.Range(0, amtLen).Where(idx => !frozen.Contains(idx)).ToArray();

                int lastColumn = frozen[frozen.Length - 1];

                Array.Fill<byte>(amounts, 1);
                bool didRollLastColumn = false;
                while( !didRollLastColumn ) {
                    Spill(amounts, SumIn(amounts, frozen), recpMax, nonFrozen);

                    //AssertFilled();

                    Array.Fill(scores, 0);
                    for( i = 0; i < amtLen; i++ ) {
                        scores[0] += amounts[i] * ingrs[i].Capacity;
                        scores[1] += amounts[i] * ingrs[i].Durability;
                        scores[2] += amounts[i] * ingrs[i].Flavor;
                        scores[3] += amounts[i] * ingrs[i].Texture;
                        scores[4] += amounts[i] * ingrs[i].Calories;
                    }

                    if( scores[4] == 500 ) {
                        ulong score = 1;
                        foreach( int prm in scores.Take(4) ) score *= (ulong)Math.Max(prm, 0);
                        if( score > maxScore ) maxScore = score;
                    }


                    for( i = 0; i < frozen.Length; i++ ) {
                        int idx = frozen[i];
                        amounts[idx]++;
                        int max = recpMax - nonFrozen.Length;
                        for( int j = i + 1; j < frozen.Length; j++ )
                            max -= amounts[frozen[j]];

                        if( amounts[idx] < max ) break;
                        else if( idx == lastColumn ) didRollLastColumn = true;
                        else amounts[idx] = 1;
                    }
                }
            }



            return maxScore.ToString();
        }
    }
}
