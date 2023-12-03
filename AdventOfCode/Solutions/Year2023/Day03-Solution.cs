using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Activation;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 03, "Gear Ratios")]
class Day03 : ASolution
{

    public Day03() : base(false)
    {
        OutputAlways = true;
    }

    List<List<Int2>> reversePoints = [];
    Dictionary<Int2, (int id, int num)> numberPoints = [];
    List<(Int2 at, char sym)> symbols = [];

    protected override void ParseInput()
    {
        var lines = Input.SplitByNewline(false, true);
        List<Int2> numPts = [];
        string num = string.Empty;
        void CompleteNumber() {
            if (numPts.Count == 0) return;
            int number = int.Parse(num);
            int id = reversePoints.Count;
            reversePoints.Add(new(numPts));
            foreach (var pt in numPts) numberPoints.Add(pt, (id, number));
            numPts.Clear(); num = string.Empty;
        }
        for (int y = 0; y < lines.Count; y++) {
            string line = lines[y];
            for(int x = 0; x < line.Length; x++) {
                if (char.IsDigit(line[x])) {
                    num += line[x];
                    numPts.Add(new Int2(x, y));
                }
                else if (line[x] == '.') {
                    CompleteNumber();
                } else {
                    CompleteNumber();
                    symbols.Add((new Int2(x, y), line[x]));
                }
            }
            CompleteNumber();
        }
    }

    protected override object SolvePartOneRaw()
    {
        var myNums = new Dictionary<Int2, (int id, int num)>(numberPoints);
        Dictionary<int, int> foundNumbers = new();

        void TestRemove(Int2 at) {
            if (myNums.TryGetValue(at, out var val)) {
                if (!foundNumbers.ContainsKey(val.id))
                    foundNumbers.Add(val.id, val.num);
            }

        }


        foreach(var (pt, sym) in symbols) {
            TestRemove(pt + Int2.Up);
            TestRemove(pt + Int2.Down);
            TestRemove(pt + Int2.Left);
            TestRemove(pt + Int2.Right);
            TestRemove(pt + Int2.Up + Int2.Left);
            TestRemove(pt + Int2.Down + Int2.Left);
            TestRemove(pt + Int2.Up + Int2.Right);
            TestRemove(pt + Int2.Down + Int2.Right);
        }


        long sum = 0;
        foreach(var n in foundNumbers) {
            sum += n.Value;
        }

        return sum;
    }

    protected override object SolvePartTwoRaw()
    {
        long sum = 0;
        foreach(var (pt, sym) in symbols) {
            if (sym == '*') {
                HashSet<(int id, int num)> parts = new();
                void TestAdd(Int2 at) {
                    if (numberPoints.TryGetValue(at, out var val)) {
                        parts.Add(val);
                    }
                }
                TestAdd(pt + Int2.Up);
                TestAdd(pt + Int2.Down);
                TestAdd(pt + Int2.Left);
                TestAdd(pt + Int2.Right);
                TestAdd(pt + Int2.Up + Int2.Left);
                TestAdd(pt + Int2.Down + Int2.Left);
                TestAdd(pt + Int2.Up + Int2.Right);
                TestAdd(pt + Int2.Down + Int2.Right);

                if (parts.Count == 2) sum += parts.First().num * parts.Last().num;
            }
        }
        return sum;
    }
}
