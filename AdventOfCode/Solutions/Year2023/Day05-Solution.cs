using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using NumRange = (long start, long length);

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 05, "If You Give A Seed A Fertilizer")]
class Day05 : ASolution
{
    class RangeMap {
        public NumRange Dest;
        public NumRange Src;
        public string From;
        public string To;

        public long DestEnd => Dest.start + Dest.length;
        public long SrcEnd => Src.start + Src.length;
        public long Offset => Src.start - Dest.start;

        public bool WithinSrc(long num) {
            return Within(Src, num);
        }

        public bool WithinDest(long num) {
            return Within(Dest, num);
        }

        public bool OverlapsSrc(NumRange rhs) {
            return Overlap(Src, rhs);
        }
        public long Map(long srcNum) {
            if (!WithinSrc(srcNum)) throw new Exception("Bad source " + srcNum);
            return Dest.start + (srcNum - Src.start);
        }

        public static bool Within(NumRange lhs, long sample) {
            return sample >= lhs.start && sample < (lhs.start + lhs.length);
        }
        public static bool Overlap(NumRange lhs, NumRange rhs) {
            bool any = false;
            any = any || Within(lhs, rhs.start);
            any = any || Within(lhs, rhs.start + rhs.length);
            any = any || Within(rhs, lhs.start);
            any = any || Within(rhs, lhs.start + lhs.length);
            return any;
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
                    Dest = (bits[0], bits[2]),
                    Src = (bits[1], bits[2]),
                    From = fromTo[0], To = fromTo[1]
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


    void SubdivideBy(RangeMap div, List<RangeMap> ranges) {
        for (int i = 0; i < ranges.Count; i++) {
            var at = ranges[i];
            //var next = i + 1 < ranges.Count ? ranges[i + 1] : null;

            if( at.WithinSrc(div.Src.start) ) {
                // Starts overlap, so we'll need to devide this range
                long delta = div.Src.start - at.Src.start;
                var before = new RangeMap() {
                    Src = (at.Src.start, delta),
                    Dest = (at.Dest.start, delta)

                };

                if ( at.WithinSrc(div.SrcEnd - 1) ) {
                    // full contained, split into before, divider, after
                    delta = at.SrcEnd - div.SrcEnd;
                    var after = new RangeMap() {
                        Src = (div.SrcEnd, delta),
                        Dest = (at.DestEnd - delta, delta)
                    };
                    ranges.RemoveAt(i);
                    ranges.InsertRange(i, [before, div, after]);
                    return; // fully consumed divider
                } else {
                    // divider goes on past the end of 'at', split into before, divider bounded by next, mutate divider based on consumed area
                    delta = at.SrcEnd - div.Src.start;
                    var common = new RangeMap() {
                        Src = (div.Src.start, delta),
                        Dest = (div.Dest.start, delta)
                    };

                    delta = div.SrcEnd - common.SrcEnd;
                    div.Src = (common.SrcEnd, delta);
                    div.Dest = (common.DestEnd, delta);
                    ranges.RemoveAt(i);
                    ranges.InsertRange(i, [before, common]);
                    continue;
                }
            }
            else if( at.WithinSrc(div.SrcEnd - 1) ) {
                // End overlaps, so we'll need to divide. We can assume divider.Dest.start doesn't overlap since that was already tested
                // split into before, divider, end
                long delta = div.SrcEnd - at.Src.start;
                var common = new RangeMap() {
                    Src = (at.Src.start, delta),
                    Dest = (div.DestEnd - delta, delta)
                };

                long endDelta = at.SrcEnd - div.SrcEnd;
                var end = new RangeMap() {
                    Src = (div.SrcEnd, endDelta),
                    Dest = (at.DestEnd - endDelta, endDelta)
                };
                
                // shorten divider by the overlap
                div.Src = (div.Src.start, div.Src.length - delta);
                div.Dest = (div.Dest.start, div.Dest.length - delta); 



                ranges.RemoveAt(i);
                ranges.InsertRange(i, [common, end]);
            } else if( div.WithinSrc(at.Src.start) && div.WithinSrc(at.SrcEnd - 1) ) {
                long delta = at.Src.start - div.Src.start;
                at.Dest = (div.Dest.start + delta, at.Dest.length);
            } else {
                // no overlap
                WriteLine("No overlap");
            }
        }
    }

    protected override object SolvePartTwoRaw()
    {
        long locationNumber = long.MaxValue;

        // map down seed to soil
        foreach (var seed in p2seeds) {
            List<RangeMap> proj = [ new RangeMap() {
                Src = seed, Dest = seed,
                From = "seed", To = "soil"
            }];

            string at = "seed";

            while (at != "location") {
                string next = fromToGlobal[at];
                // subdivide proj based on from
                foreach (var div in maps[(at, next)]) {
                    SubdivideBy(new RangeMap() {
                        Src = div.Src, Dest = div.Dest, From = div.From, To = div.To
                    }, proj);
                }
                // push project through to prepare for the next layer
                foreach (var range in proj) {
                    range.Src = range.Dest;
                    range.From = next;
                }
                // remove empties
                proj.RemoveAll(rm => rm.Src.length <= 0);
                proj.Sort((lhs, rhs) => lhs.Src.start.CompareTo(rhs.Src.start));
                at = next;
            }

            locationNumber = Math.Min(locationNumber, proj[0].Src.start);
        }

        return locationNumber;
    }
}
