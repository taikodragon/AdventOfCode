using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day22 : ASolution
    {
        const int p1 = 1, p2 = 2;
        Dictionary<int, Queue<int>> playersOriginal = new Dictionary<int, Queue<int>>(2);

        public Day22() : base(22, 2020, "Crab Combat")
        {
            UseDebugInput = false;

            int atPlayer = p1;
            foreach(string playerDesk in Input.Split("\n\n")) {
                playersOriginal[atPlayer] = new Queue<int>(playerDesk.SplitByNewline(false, true).Skip(1).Select(int.Parse));
                atPlayer++;
            }
        }

        protected override string SolvePartOne()
        {
            Dictionary<int, Queue<int>> players = new Dictionary<int, Queue<int>> {
                {p1, new Queue<int>(playersOriginal[p1]) },
                {p2, new Queue<int>(playersOriginal[p2]) }
            };
            while(!players.Values.Any(q => q.Count == 0)) {
                int p1Card = players[p1].Dequeue();
                int p2Card = players[p2].Dequeue();
                if( p1Card > p2Card ) {
                    players[p1].Enqueue(p1Card);
                    players[p1].Enqueue(p2Card);
                } else {
                    players[p2].Enqueue(p2Card);
                    players[p2].Enqueue(p1Card);
                }
            }

            var winnerDeck = players.Values.First(q => q.Count > 0).ToList();
            long score = 0;
            for(int i = 1; i <= winnerDeck.Count; i++ ) {
                score += winnerDeck[winnerDeck.Count - i] * i;
            }
            return score.ToString();
        }


        int RecursiveGame(Queue<int> p1Deck, Queue<int> p2Deck) {
            HashSet<string> plays = new HashSet<string>();
            while( p1Deck.Count > 0 && p2Deck.Count > 0 ) {
                string gameState = string.Join('|', string.Join(',', p1Deck), string.Join(',', p2Deck));
                if( plays.Contains(gameState) )
                    return p1;
                plays.Add(gameState);

                int p1Card = p1Deck.Dequeue();
                int p2Card = p2Deck.Dequeue();
                

                int winner = 0;
                if( p1Deck.Count >= p1Card && p2Deck.Count >= p2Card && (p1Deck.Count > 0 && p2Deck.Count > 0) )
                    winner = RecursiveGame(new Queue<int>(p1Deck.Take(p1Card)), new Queue<int>(p2Deck.Take(p2Card)));
                else if( p1Card > p2Card ) {
                    winner = p1;
                } else {
                    winner = p2;
                }
                if( winner == p1 ) {
                    p1Deck.Enqueue(p1Card);
                    p1Deck.Enqueue(p2Card);
                } else if( winner == p2 ) {
                    p2Deck.Enqueue(p2Card);
                    p2Deck.Enqueue(p1Card);
                }
            }
            return p1Deck.Count > 0 ? p1 : p2;
        }

        protected override string SolvePartTwo()
        {
            Queue<int> p1Deck = new Queue<int>(playersOriginal[p1]),
                p2Deck = new Queue<int>(playersOriginal[p2]);

            int winner = RecursiveGame(p1Deck, p2Deck);

            var winnerDeck = (winner == p1 ? p1Deck : p2Deck).ToList();
            long score = 0;
            for( int i = 1; i <= winnerDeck.Count; i++ ) {
                score += winnerDeck[winnerDeck.Count - i] * i;
            }
            return score.ToString();
        }
    }
}
