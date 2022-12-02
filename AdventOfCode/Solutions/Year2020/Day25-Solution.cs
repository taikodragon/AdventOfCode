using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 25, "")]
    class Day25 : ASolution
    {
        const long encmod = 20201227;
        long cardPubKey, doorPubKey;

        bool continueGuessing = true;

        public Day25() : base(false)
        {
            

            if( UseDebugInput ) {
                cardPubKey = 5764801;
                doorPubKey = 17807724;
            } else {
                var lines = Input.SplitByNewline();
                cardPubKey = long.Parse(lines[0]);
                doorPubKey = long.Parse(lines[1]);
            }
        }

        long TransformValue(long subjectNumber, long loopSize) {
            long value = 1;
            for(long i = 0; i < loopSize; ++i ) {
                value = (value * subjectNumber) % encmod;
            }
            return value;
        }

        void GuessLoopSize(object args) {
            (long pubKey, Action<long> resultSet) = ((long,Action<long>))args;

            long value = 1;
            for( long i = 1; i <= 10000000 && continueGuessing; ++i ) {
                value = (value * 7) % encmod;
                if( pubKey == value ) {
                    continueGuessing = false;
                    resultSet(i);
                }
            }
        }

        protected override string SolvePartOne()
        {
            long cardLoopSize = 0, doorLoopSize = 0;

            Thread cardGuesser = new Thread(GuessLoopSize);
            cardGuesser.Start((cardPubKey, new Action<long>(i => cardLoopSize = i)));
            Thread doorGuesser = new Thread(GuessLoopSize);
            doorGuesser.Start((doorPubKey, new Action<long>(i => doorLoopSize = i)));

            cardGuesser.Join();
            doorGuesser.Join();

            if( cardLoopSize > 0 ) {
                return TransformValue(doorPubKey, cardLoopSize).ToString();
            }
            else if( doorLoopSize > 0) {
                return TransformValue(cardPubKey, doorLoopSize).ToString();
            } else {
                return "FAIL";
            }
        }

        protected override string SolvePartTwo()
        {
            return "ALL DONE";
        }
    }
}
