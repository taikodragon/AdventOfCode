using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Input;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 15, "Beacon Exclusion Zone")]
class Day15 : ASolution
{

    public Day15() : base(false) { }

    List<Sensor> sensors = new();
    protected override void ParseInput() {
        foreach(var line in Input.SplitByNewline(false, true)) {
            var pure = line.Replace("Sensor at x=", string.Empty)
                .Replace(" y=", string.Empty)
                .Replace(" closest beacon is at x=", string.Empty);
            var pts = pure.Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => int.Parse(n))
                .ToArray();
            sensors.Add(new Sensor((pts[0], pts[1]), (pts[2], pts[3])));
        }
    }
    protected override object SolvePartOneRaw() {
        int yRef = UseDebugInput ? 10 : 2000000;
        int xMin = int.MaxValue, xMax = int.MinValue;
        foreach(var sensor in sensors) {
            int sensorMin = sensor.Position.X - sensor.Radius;
            if(sensorMin < xMin) {
                xMin = sensorMin;
            }
            int sensorMax = sensor.Position.X + sensor.Radius;
            if(sensorMax > xMax) {
                xMax = sensorMax;
            }
        }

        HashSet<IntCoord> foundBeacons = new(xMax - xMin);
        HashSet<IntCoord> withinRadius = new(xMax - xMin);
        for(int i = xMin; i < xMax; i++) {
            IntCoord pt = (i, yRef);
            foreach(var s in sensors) {
                if (s.NearestBeacon == pt) {
                    foundBeacons.Add(pt);
                    break;
                }
                if (Utilities.ManhattanDistance(s.Position, pt) <= s.Radius) {
                    withinRadius.Add(pt);
                }
            }
        }

        return withinRadius.Count - foundBeacons.Count;
    }

    HashSet<IntCoord> maybeBeacon = new();
    protected override object SolvePartTwoRaw() {
        OutputAlways = true;

        int max = UseDebugInput ? 20 : 4_000_000;

        IntCoord se = IntCoord.Down + IntCoord.Right,
            sw = IntCoord.Down + IntCoord.Left,
            nw = IntCoord.Up + IntCoord.Left,
            ne = IntCoord.Up + IntCoord.Right;

        var result = Parallel.ForEach(sensors, (sensor) => {
            IntCoord pos = sensor.Position;
            IntCoord delta = se;
            IntCoord pt = sensor.Position + (0, -(sensor.Radius + 1));
            IntCoord start = pt;

            do {
                // Only add points in bounds
                if( pt.X >= 0 && pt.X <= max && pt.Y >= 0 && pt.Y <= max) {
                    if (sensors.Any(s => Utilities.ManhattanDistance(s.Position, pt) <= s.Radius)) { }
                    else { maybeBeacon.Add(pt); }
                }

                if (delta == se && pt.Y == pos.Y) delta = sw;
                else if (delta == sw && pt.X == pos.X) delta = nw;
                else if (delta == nw && pt.Y == pos.Y) delta = ne;

                pt += delta;
            } while (pt != start);
        });
        if (!result.IsCompleted) throw new Exception("loop broke");

        var beacon = maybeBeacon.First();
        return beacon.X * 4_000_000L + beacon.Y;
    }

    class Sensor {
        public readonly IntCoord Position;
        public readonly IntCoord NearestBeacon;
        public readonly int Radius;

        public Sensor(IntCoord position, IntCoord nearestBeacon) {
            Position = position;
            NearestBeacon = nearestBeacon;
            Radius = Utilities.ManhattanDistance(position, nearestBeacon);
        }
    }
}
