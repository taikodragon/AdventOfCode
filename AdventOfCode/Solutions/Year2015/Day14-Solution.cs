using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 14, "Reindeer Olympics")]
    class Day14 : ASolution
    {
        class Flyer
        {
            public string Name { get; set; }
            public int Speed { get; set; }
            public int FlyTime { get; set; }
            public int RestTime { get; set; }

            public int DistTraveled { get; set; }
            public int StartedFlyingAt { get; set; }
            public int StartedRestingAt { get; set; }
            public bool IsFlying { get; set; } = true;
            public int Points { get; set; }
        }

        public Day14() : base(false)
        {
            
        }

        void PartOneUsingAverageSpeed() {
            var winner = Input
                .Replace(" can fly ", " ")
                .Replace(" km/s for ", " ")
                .Replace(" seconds, but then must rest for ", " ")
                .Replace(" seconds.", string.Empty)
                .SplitByNewline(false, true)
                .Select(s => s.Split(' '))
                .Select(p => (p[0], double.Parse(p[1]), double.Parse(p[2]), double.Parse(p[3])))
                .Select(t => (t.Item1, (t.Item2 * t.Item3) / (t.Item3 + t.Item4)))
                .OrderByDescending(t => t.Item2)
                .First();

            Trace.WriteLine($"Winner is {winner.Item1} with distance {Math.Ceiling(winner.Item2 * 2503)}");
        }


        protected override string SolvePartOne()
        {
            var flyers = Input
                .Replace(" can fly ", " ")
                .Replace(" km/s for ", " ")
                .Replace(" seconds, but then must rest for ", " ")
                .Replace(" seconds.", string.Empty)
                .SplitByNewline(false, true)
                .Select(s => s.Split(' '))
                .Select(p => new Flyer { Name = p[0], Speed = int.Parse(p[1]), FlyTime = int.Parse(p[2]), RestTime = int.Parse(p[3]) })
                .ToList();

            for(int s = 0; s < 2503; s++) {
                foreach(var flyer in flyers) {
                    if( flyer.IsFlying ) {
                        int timeFlying = s - flyer.StartedFlyingAt;
                        if( timeFlying < flyer.FlyTime )
                            flyer.DistTraveled += flyer.Speed;
                        else { // switch to resting
                            flyer.IsFlying = false;
                            flyer.StartedRestingAt = s;
                        }
                    }
                    else { // resting phase
                        int timeResting = s - flyer.StartedRestingAt;
                        if( timeResting >= flyer.RestTime ) { // done resting, make sure we "fly" for this second
                            flyer.IsFlying = true;
                            flyer.StartedFlyingAt = s;
                            flyer.DistTraveled += flyer.Speed;
                        }
                    }
                }
            }
            var winner = flyers.OrderByDescending(f => f.DistTraveled).First();
            Trace.WriteLine($"Winner is {winner.Name} with distance {winner.DistTraveled}");

            return winner.DistTraveled.ToString();
        }

        protected override string SolvePartTwo()
        {
            var flyers = Input
                .Replace(" can fly ", " ")
                .Replace(" km/s for ", " ")
                .Replace(" seconds, but then must rest for ", " ")
                .Replace(" seconds.", string.Empty)
                .SplitByNewline(false, true)
                .Select(s => s.Split(' '))
                .Select(p => new Flyer { Name = p[0], Speed = int.Parse(p[1]), FlyTime = int.Parse(p[2]), RestTime = int.Parse(p[3]) })
                .ToList();

            for( int s = 0; s < 2503; s++ ) {
                foreach( var flyer in flyers ) {
                    if( flyer.IsFlying ) {
                        int timeFlying = s - flyer.StartedFlyingAt;
                        if( timeFlying < flyer.FlyTime )
                            flyer.DistTraveled += flyer.Speed;
                        else { // switch to resting
                            flyer.IsFlying = false;
                            flyer.StartedRestingAt = s;
                        }
                    }
                    else { // resting phase
                        int timeResting = s - flyer.StartedRestingAt;
                        if( timeResting >= flyer.RestTime ) { // done resting, make sure we "fly" for this second
                            flyer.IsFlying = true;
                            flyer.StartedFlyingAt = s;
                            flyer.DistTraveled += flyer.Speed;
                        }
                    }
                }

                int leadDist = flyers.Max(f => f.DistTraveled);
                foreach( var flyer in flyers.Where(f => f.DistTraveled == leadDist) )
                    flyer.Points++;
            }
            var winner = flyers.OrderByDescending(f => f.Points).First();
            Trace.WriteLine($"Winner is {winner.Name} with distance {winner.Points}");

            return winner.Points.ToString();
        }
    }
}
