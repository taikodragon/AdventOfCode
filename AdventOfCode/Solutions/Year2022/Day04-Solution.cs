using ABI.System.Collections.Generic;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 04, "Camp Cleanup")]
class Day04 : ASolution
{
    class SectionAssignment {
        public SectionAssignment(string range) {
            var numbers = range.Split('-');
            if (numbers.Length != 2) throw new ArgumentException("Unknown range sequence");
            Lower = int.Parse(numbers[0]);
            Upper = int.Parse(numbers[1]);
        }
        public int Lower { get; set; }
        public int Upper { get; set; }

        public bool FullContains(SectionAssignment other) {
            return Lower <= other.Lower && Upper >= other.Upper;
        }
        public bool PartialContains(SectionAssignment other) {
            return FullContains(other) ||
                (Lower <= other.Lower && other.Lower <= Upper) ||
                (Lower <= other.Upper && other.Upper <= Upper);
        }
    }

    public Day04() : base(false)
    {
        
    }

    List<(SectionAssignment Elf1, SectionAssignment Elf2)> data = new();
    protected override void ParseInput() {
        data.AddRange(
            Input.SplitByNewline(false, true)
                .Select(s => s.Split(','))
                .Select(pair => (new SectionAssignment(pair[0]), new SectionAssignment(pair[1])))
        );
    }

    protected override string SolvePartOne()
    {
        return data.Count(pair => pair.Elf1.FullContains(pair.Elf2) || pair.Elf2.FullContains(pair.Elf1))
            .ToString();
    }

    protected override string SolvePartTwo()
    {
        return data.Count(pair => pair.Elf1.PartialContains(pair.Elf2) || pair.Elf2.PartialContains(pair.Elf1))
            .ToString();
    }
}
