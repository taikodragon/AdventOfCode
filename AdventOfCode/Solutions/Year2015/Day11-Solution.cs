using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 11, "Corporate Policy")]
    class Day11 : ASolution
    {
        static readonly char[] letters = "abcdefghjkmnpqrstuvwxyz".ToCharArray();
        static readonly int lettersCount = letters.Length;

        string input;
        public Day11() : base(false)
        {
            DebugInput = "abcdefgh";
            UseDebugInput = true;
            input = Input;
        }

        static void Inc(byte[] pw) {
            for(int i = pw.Length - 1; i > -1; --i) {
                pw[i]++;
                if( pw[i] < lettersCount ) break;
                else pw[i] = 0; // roll over
            }
        }
        static string AsString(byte[] pw) {
            return string.Concat(pw.Select(b => letters[b]));
        }
        protected override string SolvePartOne()
        {
            const int pwLen = 8, notFound = -2;
            byte[] pw = new byte[pwLen];
            int i, dblA, dblB;
            // encode Input into our password storage
            for( i = 0; i < pw.Length; i++ ) {
                int idx = Array.IndexOf(letters, input[i]);
                if(idx == -1) {
                    idx = Array.IndexOf(letters, input[i] + 1);
                    pw[i] = (byte)idx;
                    break;
                }
                pw[i] = (byte)idx;
            }

            bool cont = true;

            while(cont) {
                Inc(pw);
                //if( UseDebugInput ) Trace.WriteLine(AsString(pw));
                dblA = notFound;
                dblB = notFound;
                for( i = 0; i < pwLen - 1; i++ ) {
                    if( pw[i] == pw[i + 1] ) {
                        if( dblA == notFound ) {
                            dblA = i;
                        }
                        else if( dblB == notFound && dblA != i - 1 ) {
                            dblB = i;
                            break;
                        }
                    }
                }
                if( dblA == notFound || dblB == notFound )
                    continue;

                for( i = 0; cont && i < pwLen - 2; i++ ) {
                    if( pw[i + 1] - pw[i] == 1 && pw[i + 2] - pw[i + 1] == 1 )
                        cont = false; // found!
                }
            }
            return AsString(pw);
        }

        protected override string SolvePartTwo()
        {
            input = Part1;
            return SolvePartOne();
        }
    }
}
