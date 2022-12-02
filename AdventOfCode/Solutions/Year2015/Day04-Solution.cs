using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 04, "The Ideal Stocking Stuffer")]
    class Day04 : ASolution
    {

        public Day04() : base(false)
        {
            
            baseStr = Input;
        }

        int numThreads = 8, finalNumber;
        string baseStr, startsWith;
        
        void ThreadProc(object obj) {
            string hashed = string.Empty;
            MD5 hasher = MD5.Create();
            for( int i = (int)obj; finalNumber == 0 ; i += numThreads ) {
                hashed = string.Concat(
                    hasher.ComputeHash(
                        Encoding.UTF8.GetBytes(string.Concat(baseStr, i))
                    ).Take(3)
                    .Select(b => b.ToString("X2")));
                if( hashed.StartsWith(startsWith) ) finalNumber = i;
            }
        }

        protected override string SolvePartOne()
        {
            startsWith = "00000";
            List<Thread> threads = new List<Thread>();
            for(int i = 0; i < numThreads; ++i) {
                Thread th = new Thread(ThreadProc);
                threads.Add(th);
                th.Start(i);
            }
            foreach( var th in threads ) th.Join();

            return finalNumber.ToString();
        }

        protected override string SolvePartTwo()
        {
            startsWith = "000000";
            finalNumber = 0;
            List<Thread> threads = new List<Thread>();
            for( int i = 0; i < numThreads; ++i ) {
                Thread th = new Thread(ThreadProc);
                threads.Add(th);
                th.Start(i);
            }
            foreach( var th in threads ) th.Join();

            return finalNumber.ToString();
        }
    }
}
