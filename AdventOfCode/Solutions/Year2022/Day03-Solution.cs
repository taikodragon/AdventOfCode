using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Appointments;

namespace AdventOfCode.Solutions.Year2022;

[DayInfo(2022, 03, "Rucksack Reorganization")]
class Day03 : ASolution
{

    public Day03() : base(false)
    {
        
    }

    List<string> sacks;
    protected override void ParseInput() {
        sacks = Input.SplitByNewline(false, true);
    }

    int ItemScore(char item) {
        if(item >= 'a' && item <= 'z')
            return item - 'a' + 1;
        else if (item >= 'A' && item <= 'Z')
            return item - 'A' + 27;
        else
            throw new ArgumentOutOfRangeException(nameof(item), "Needs to be a-z A-Z");
    }

    protected override string SolvePartOne()
    {
        var compartments = sacks.Select(s => {
                int compLen = s.Length / 2;
                return new string[] {
                    s[..compLen],
                    s[compLen..],
                };
            });

        long sum = 0;
        HashSet<char> set = new HashSet<char>();
        foreach (string[] comp in compartments) {
            set.Clear();
            foreach(char r in comp[1]) { set.Add(r); }
            foreach(char l in comp[0]) {
                if (set.Contains(l)) {
                    sum += ItemScore(l);
                    break;
                }
            }
        }
        return sum.ToString();
    }

    protected override string SolvePartTwo()
    {
        long sum = 0;
        List<string> group = new(3);
        HashSet<char> set1 = new(), set2 = new();
        foreach (var sack in sacks) {
            group.Add(sack);
            if(group.Count == 3) {
                foreach(var c in group[0]) {
                    set1.Add(c);
                }
                foreach(var c in group[1]) {
                    set2.Add(c);
                }
                foreach(var c in group[2]) {
                    if(set1.Contains(c) && set2.Contains(c)) {
                        sum += ItemScore(c);
                        break;
                    }
                }

                set1.Clear();
                set2.Clear();
                group.Clear();
            }
        }
        return sum.ToString();
    }
}
