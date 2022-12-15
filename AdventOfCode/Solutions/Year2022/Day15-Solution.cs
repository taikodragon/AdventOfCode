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
        return null;
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

    class Iter {
        public int y, xMin, xMax;
        public (int x, int rad, int yDist)[] sensors;
        public HashSet<IntCoord> maybeBeacon;
    }
    bool stop = false;
    ConcurrentQueue<Iter> yLines = new();
    protected override object SolvePartTwoRaw() {
        OutputAlways = true;
        int max = UseDebugInput ? 20 : 4_000_000;
        int yMax = max;
        int xMin = int.MaxValue, xMax = int.MinValue;
        foreach (var sensor in sensors) {
            int sensorMin = sensor.Position.X - sensor.Radius;
            if (sensorMin < xMin) {
                xMin = sensorMin;
            }
            int sensorMax = sensor.Position.X + sensor.Radius;
            if (sensorMax > xMax) {
                xMax = sensorMax;
            }
        }
        xMin = Math.Max(0, xMin);
        xMax = Math.Min(max, xMax);

        List<Task> threads = new();
        for (int i = 0; i < 6; i++) {
            threads.Add(new Task(P2_Thread, TaskCreationOptions.LongRunning));
            threads[^1].Start();
        }

        Stopwatch sw = new();
        for (int y = 0; y < yMax; y++) {
            if (y % 10 == 0) {
                sw.Stop();
                WriteLine($"{y} -- {sw.ElapsedMilliseconds / 10.0}");
            }
            sw.Restart();
            (int x, int rad, int yDist)[] filtered = sensors
                .Where(s => s.Position.Y + s.Radius >= y && y >= s.Position.Y - s.Radius)
                .Select(s => (s.Position.X, s.Radius, Math.Abs(s.Position.Y - y)))
                .ToArray();

            IntCoord pt = (0, y);

            while (yLines.Count >= 20_000)
                Thread.Sleep(1);

            yLines.Enqueue(new Iter {
                y = y,
                xMin = xMin,
                xMax = xMax,
                sensors = filtered
            });
        }

        stop = true;
        Task.WaitAll(threads.ToArray());

        var beacon = maybeBeacon.First();
        return beacon.X * 4_000_000L + beacon.Y;
    }

    void P2_Thread() {
        while(!stop || yLines.Count > 0) {

            if (!yLines.TryDequeue(out var line)) {
                Thread.Sleep(1);
                continue;
            }
            IntCoord pt = (0, line.y);
            for (int xBase = line.xMin, xStride = 1000; (xBase) < line.xMax; xBase += xStride) {
                int xChunkMax = xBase + xStride;
                (int x, int rad, int yDist)[] moreFiltered = line.sensors
                    .Where(s => {
                        int sMax = s.x + s.rad, sMin = s.x - s.rad;
                        return (sMax <= xChunkMax && sMin >= xBase) ||
                            (sMax >= xBase && xBase >= sMin) ||
                            (sMax >= xChunkMax && xChunkMax >= sMin);
                    })
                    .ToArray();
                int filteredCount = moreFiltered.Length - 1;
                for (int x = xBase; x < xChunkMax; x++) {
                    pt.X = x;
                    bool withinReach = false;
                    for (int i = filteredCount; i >= 0; i--) {
                        var s = moreFiltered[i];
                        withinReach = (Math.Abs(s.x - x) + s.yDist) <= s.rad;
                        if (withinReach) break;

                    }
                    if (!withinReach)
                        maybeBeacon.Add(pt);
                }

            }
        }
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
