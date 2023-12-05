using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.ExtendedExecution.Foreground;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 05, "If You Give A Seed A Fertilizer")]
class Day05 : ASolution
{
    class RangeMap {
        public long DestStart;
        public long SrcStart;
        public long Length;
        public string From;
        public string To;
        
        public bool WithinSrc(long num) {
            return num >= SrcStart && num < (SrcStart + Length);
        }

        public bool WithinDest(long num) {
            return num >= DestStart && num < (DestStart + Length);
        }

        public long Map(long srcNum) {
            if (!WithinSrc(srcNum)) throw new Exception("Bad source " + srcNum);
            return DestStart + (srcNum - SrcStart);
        }
    }

    List<long> seeds = new();
    List<(long start, long length)> p2seeds = new();
    Dictionary<(string from, string to), List<RangeMap>> maps = new();
    Dictionary<string, string> fromToGlobal = new();
    public Day05() : base(false)
    {
            
    }

    protected override void ParseInput()
    {
        var groups = Input.Split(new string[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var group in groups) {
            var parts = group.Split(new string[] { ":", " map:", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if( parts[0] == "seeds" ) {
                seeds.AddRange(parts[1].Split(' ').Select(long.Parse));
                p2seeds.AddRange(seeds.Chunk(2).Select(arr => (arr[0], arr[1])));
                continue;
            }
            var fromTo = parts[0].Split("-to-");
            List<RangeMap> ranges = new();
            maps.Add((fromTo[0], fromTo[1]), ranges);
            fromToGlobal.Add(fromTo[0], fromTo[1]);
            foreach(var rangenum in parts[1..]) {
                var bits = rangenum.Split(' ').Select(long.Parse).ToArray();
                RangeMap rm = new() {
                    DestStart = bits[0],
                    SrcStart = bits[1],
                    Length = bits[2],
                    From = fromTo[0],
                    To = fromTo[1]
                };
                ranges.Add(rm);
            }
        }
    }

    long WalkTo(string from, long num, string to) {
        while(from != to) {

            // look for self to find next
            string nextTo = fromToGlobal[from];
            var ranges = maps[(from, nextTo)];
            var mapped = ranges.FirstOrDefault(rm => rm.WithinSrc(num));

            from = nextTo;
            if ( mapped is not null ) {
                num = mapped.Map(num);
            }
        }
        return num;
    }

    protected override object SolvePartOneRaw()
    {
        List<long> locationNumbers = new();

        foreach(var seed in seeds) {
            locationNumbers.Add(WalkTo("seed", seed, "location"));
        }

        return locationNumbers.Min();
    }

    protected override object SolvePartTwoRaw()
    {
        long locationNumber = long.MaxValue;

        foreach (var seed in p2seeds) {
            long end = seed.start + seed.length;
            for(long i = seed.start; i < end; i++) {
                locationNumber = Math.Min(locationNumber, WalkTo("seed", i, "location"));
            }
        }

        return locationNumber;
    }
}
