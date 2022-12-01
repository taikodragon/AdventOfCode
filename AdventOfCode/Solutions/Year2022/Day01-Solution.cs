using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;
class Day01 : ASolution
{
    public Day01() : base(01, 2022, "", false)
    {
        

    }

    protected override string SolvePartOne()
    {
        return Input.Split("\n\n")
            .Select(elf => elf.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            .Select(elf => elf.Select(long.Parse))
            .Select(elf => elf.Sum())
            .Max().ToString();
    }

    protected override string SolvePartTwo()
    {
        return Input.Split("\n\n")
            .Select(elf => elf.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            .Select(elf => elf.Select(long.Parse))
            .Select(elf => elf.Sum())
            .OrderByDescending(elf => elf)
            .Take(3)
            .Sum()
            .ToString();
    }
}
