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

        var sensorsInRange = sensors
            .Where(s => s.Position.Y + s.Radius >= yRef && yRef >= s.Position.Y - s.Radius)
            .ToList();
        foreach(var sensor in sensorsInRange) {
            int sensorMin = sensor.Position.X - sensor.Radius;
            if(sensorMin < xMin) {
                xMin = sensorMin;
            }
            int sensorMax = sensor.Position.X + sensor.Radius;
            if(sensorMax > xMax) {
                xMax = sensorMax;
            }
        }

        HashSet<IntCoord> withinRadius = new(xMax - xMin);
        HashSet<IntCoord> foundBeacons = new(sensorsInRange.Count);

        foreach(var sensor in sensorsInRange) {
            if (sensor.NearestBeacon.Y == yRef) foundBeacons.Add(sensor.NearestBeacon);

            int yDelta = Math.Abs(sensor.Position.Y - yRef);
            int rxMin = sensor.Position.X - (sensor.Radius - yDelta);
            int rxMax = sensor.Position.X + (sensor.Radius - yDelta);
            for (int i = rxMin; i <= rxMax; i++) {
                withinRadius.Add((i,yRef));
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

        var result = Parallel.ForEach(sensors, (sensor, state) => {
            IntCoord pos = sensor.Position;
            IntCoord delta = se;
            IntCoord pt = sensor.Position + (0, -(sensor.Radius + 1));
            IntCoord start = pt;
            Sensor[] nearby = sensors
                .Where(s => Utilities.ManhattanDistance(s.Position, sensor.Position) < (sensor.Radius + s.Radius + 2))
                .ToArray();

            do {
                // Only add points in bounds
                if( pt.X >= 0 && pt.X <= max && pt.Y >= 0 && pt.Y <= max) {
                    if (nearby.Any(s => Utilities.ManhattanDistance(s.Position, pt) <= s.Radius)) { }
                    else {
                        maybeBeacon.Add(pt);
                        state.Break();
                        return;
                    }
                }

                if (delta == se && pt.Y == pos.Y) delta = sw;
                else if (delta == sw && pt.X == pos.X) delta = nw;
                else if (delta == nw && pt.Y == pos.Y) delta = ne;

                pt += delta;
            } while (pt != start && !state.ShouldExitCurrentIteration);
        });

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
