using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2021
{
    class BingoSquare
    {
        public int Num { get; set; }
        public bool Selected { get; set; }
    }
    class Day04 : ASolution
    {
        const int BOARD_SIZE = 5;

        List<int> draws;
        List<BingoSquare[,]> boards = new();

        public Day04() : base(04, 2021, "", false)
        {
            var parts = Input.Replace("\r","").Split("\n\n");
            draws = parts[0].Split(',').Select(int.Parse).ToList();
            
            for(int i = 1; i < parts.Length; i++) {
                var newBoard = new BingoSquare[BOARD_SIZE, BOARD_SIZE];
                var lines = parts[i].Split('\n');
                for (int j = 0; j < lines.Length; j++) {
                    var cols = lines[j].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < cols.Length; k++) {
                        newBoard[j, k] = new BingoSquare { Num = int.Parse(cols[k]) };
                    }
                }
                boards.Add(newBoard);
            }
        }

        bool CheckBoardForWin(BingoSquare[,] board) {
            int[] rows = new int[5];
            int[] cols = new int[5];

            for (int i = 0; i < BOARD_SIZE; i++) {
                for (int j = 0; j < BOARD_SIZE; j++) {
                    if( board[i,j].Selected ) {
                        rows[i]++;
                        cols[j]++;
                    }
                }
            }
            return rows.Any(n => n == BOARD_SIZE) || cols.Any(n => n == BOARD_SIZE);
        }

        void MakePlay(BingoSquare[,] board, int number) {
            foreach(var sq in board) {
                if (sq.Num == number)
                    sq.Selected = true;
            }
        }

        void Reset() {
            foreach (var board in boards) {
                foreach (var sq in board) {
                    sq.Selected = false;
                }
            }
        }
        protected override string SolvePartOne()
        {
            Reset();

            int winDraw = 0;
            BingoSquare[,] winBoard = null;
            foreach(int draw in draws) {
                foreach(var board in boards) {
                    MakePlay(board, draw);
                    if (CheckBoardForWin(board)) {
                        winDraw = draw;
                        winBoard = board;
                        break;
                    }
                }
                if (winBoard != null) break;
            }

            int unmarkedSum = 0;
            foreach(var sq in winBoard) {
                if( !sq.Selected )
                    unmarkedSum += sq.Num;
            }
            
            return (unmarkedSum * winDraw).ToString();
        }

        protected override string SolvePartTwo()
        {
            Reset();
            var localBoards = new List<BingoSquare[,]>(boards);
            var removals = new List<BingoSquare[,]>();
            int winDraw = 0;
            BingoSquare[,] winBoard = null;
            foreach (int draw in draws) {
                foreach (var board in localBoards) {
                    MakePlay(board, draw);
                    if (CheckBoardForWin(board)) {
                        if( localBoards.Count - removals.Count > 1) {
                            removals.Add(board);
                        } else {
                            winDraw = draw;
                            winBoard = board;
                            break;
                        }
                    }
                }
                foreach(var board in removals) { localBoards.Remove(board);  }
                removals.Clear();
                if (winBoard != null) break;
            }

            int unmarkedSum = 0;
            foreach (var sq in winBoard) {
                if (!sq.Selected)
                    unmarkedSum += sq.Num;
            }

            return (unmarkedSum * winDraw).ToString();
        }
    }
}
