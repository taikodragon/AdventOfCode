using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day02 : ASolution
    {
        List<(int l, int w, int h)> presents;

        public Day02() : base(02, 2015, "I Was Told There Would Be No Math")
        {
            UseDebugInput = false;

            presents = Input.SplitByNewline().Select(s => {
                string[] dims = s.Split('x');
                return (int.Parse(dims[0]), int.Parse(dims[1]), int.Parse(dims[2]));
            }).ToList();
        }

        protected override string SolvePartOne()
        {
            return presents
                .Select(ps => new int[] { ps.l * ps.w, ps.w * ps.h, ps.l * ps.h })
                .Select(areas => areas.Select(area => area * 2).Sum() + areas.Min())
                .Sum().ToString();
        }

        protected override string SolvePartTwo()
        {
            return presents
                .Select(ps => 
                    new int[] { ps.l, ps.w, ps.h }
                    .OrderBy(d => d)
                    .Take(2)
                    .Select(d => d * 2)
                    .Sum()
                    + (ps.l * ps.w * ps.h))
                .Sum().ToString();
                
        }
    }
}
