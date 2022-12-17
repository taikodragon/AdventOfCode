using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Graphics.Printing.PrintSupport;
using Windows.Media.Capture;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode.Solutions.Year2022
{


    [DayInfo(2022, 16, "Proboscidea Volcanium")]
    class Day16 : ASolution
    {


        public Day16() : base(false)
        {
            
        }

        Dictionary<string, ValveData> valves = new();
        protected override void ParseInput() {
            foreach (var line in Input.SplitByNewline(false, true)) {
                string[] content = line.Replace("Valve ", string.Empty)
                    .Replace("has flow rate=", string.Empty)
                    .Replace("; tunnels lead to valves", string.Empty)
                    .Replace("; tunnel leads to valve", string.Empty)
                    .Replace(",", string.Empty)
                    .Split(' ');

                ValveData v = new(content[0], int.Parse(content[1]));
                v.exits.AddRange(content.Skip(2));
                valves.Add(v.Id, v);
            }

            // Populate time to other locations
            foreach(var (id,v) in valves) {
                var paths = v.pathToOthers = PathToNodes(v);
                foreach(string key in valves.Keys) {
                    if (id == key) continue;
                    ValveData targ = valves[key];
                    var path = paths[key];
                    v.timeToDestination.Add((path.Count, targ.Rate, key));
                }
            }
        }
        protected override object SolvePartOneRaw()
        {
            Dictionary<string, int> openValves = new();
            string at = "AA";

            Queue<string> followPath = new();
            string openNext = null;
            int remaining = 30;
            long releasedPressure = 0;
            while(remaining > 0) {
                WriteLine($"== Minute {(30 - remaining) + 1} ==");

                // note any open values
                if( openNext == null ) { // select next value to open
                    ValveData loc = valves[at];
                    var poss = loc.timeToDestination
                        .Where(t => !openValves.ContainsKey(t.id) && t.rate > 0)
                        .Select(t => (pres: t.rate * (remaining - t.time), t.time, t.id))
                        .ToList();
                    poss.Sort(ComparePaths_Part1);
                    var next = poss.FirstOrDefault();
                    if( next != default ) {
                        openNext = next.id;
                        if( at != openNext ) {
                            followPath.Clear(); // ensure path is empty
                            loc.pathToOthers[openNext].ForEach( s => {
                                if (s == at) return;
                                followPath.Enqueue(s);
                            });
                            followPath.Enqueue(openNext);
                        }
                    }
                }

                string shouldOpen = null;
                if( openNext != null ) {
                    ValveData target = valves[openNext];
                    if( openNext == at ) { // spend this minute opening the valve
                        shouldOpen = openNext;
                        openNext = null;
                    } else {
                        at = followPath.Dequeue();
                        WriteLine($"Move to {at}");
                    }
                }

                // accumulate pressure
                releasedPressure += openValves.Values.Sum();
                remaining--;

                // open valve at the end of the minute
                if( shouldOpen is not null ) {
                    openValves.Add(shouldOpen, valves[shouldOpen].Rate);
                    WriteLine($"Open valve {shouldOpen}");
                }
            }
            return releasedPressure;
        }

        class Actor {
            public string at = "AA";
            public readonly Queue<string> followPath = new();
            public string openNext = null;
        }
        class P2_State {
            public Dictionary<string, int> openValves = new();
            public Actor you = new();
            public Actor elepant = new();
            public List<string> openNextSet = new();
            public int remaining = 26;

        }
        protected override object SolvePartTwoRaw()
        {
            OutputAlways = true;

            P2_State state = new();
            Dictionary<string, int> openValves = state.openValves;

            List<Actor> actors = new() {
                state.you, state.elepant
            };
            Actor you = state.you, elephant = state.elepant;

            Dictionary<string, int> visits = new();
            foreach(var (id, _) in valves) {
                visits.Add($"you-{id}", id == "AA" ? 1 : 0);
                visits.Add($"ele-{id}", id == "AA" ? 1 : 0);
            }

            List<(string id, Actor actor)> shouldOpen = new();
            List<string> openNextSet = state.openNextSet;
            Dictionary<string, int> releasedPressureEach = new();
            long releasedPressure = 0;
            while (state.remaining > 0) {
                WriteLine($"== Minute {(26 - state.remaining) + 1} ==");

                foreach(var actor in actors) {
                    if (actor.openNext == null) { // select next value to open
                        var otherActor = actor == you ? elephant : you;

                        ValveData loc = valves[actor.at];
                        var (next, _) = PickNext(loc, state);
                        if (next != default) {
                            //ValveData otherLoc = valves[otherActor.openNext ?? otherActor.at];
                            //int othersTimeToNext = otherLoc.timeToDestination.First(t => t.id == next.id).time;
                            //if ( (next.time + otherActor.followPath.Count) > othersTimeToNext ) {
                            //    WriteLine($"{(actor == you ? "You" : "Elephant")} skips as other is closer to {next.id}");
                            //}
                            //else {
                                WriteLine($"{(actor == you ? "You" : "Elephant")} target to {next.Id}");
                                actor.openNext = next.Id;
                                openNextSet.Add(next.Id);
                                if (actor.at != actor.openNext) {
                                    actor.followPath.Clear(); // ensure path is empty
                                    loc.pathToOthers[actor.openNext].ForEach(s => {
                                        if (s == actor.at) return;
                                        actor.followPath.Enqueue(s);
                                    });
                                    actor.followPath.Enqueue(actor.openNext);
                                }
                            //}

                        }
                    }
                }

                shouldOpen.Clear();
                foreach(var actor in actors) {
                    if (actor.openNext != null) {
                        ValveData target = valves[actor.openNext];
                        if (actor.openNext == actor.at) { // spend this minute opening the valve
                            shouldOpen.Add((actor.openNext, actor));
                            openNextSet.Remove(actor.openNext);
                            actor.openNext = null;
                        }
                        else {
                            actor.at = actor.followPath.Dequeue();
                            visits[$"{(actor==you ? "you" : "ele")}-{actor.at}"]++;
                            //WriteLine($"{(actor == you ? "You" : "Elephant")} move to {actor.at}");
                        }
                    }


                }

                // accumulate pressure
                foreach(var (key, rate) in openValves) {
                    releasedPressure += rate;
                    releasedPressureEach[key] = rate + releasedPressureEach.GetValueOrDefault(key);
                }
                state.remaining--;

                // open valve at the end of the minute
                foreach(var (id, actor) in shouldOpen) {
                    openValves.Add(id, valves[id].Rate);
                    WriteLine($"{(actor == you ? "You" : "Elephant")} open valve {id}");
                }
            }

            WriteLine("Released Pressure by Valve");
            WriteLine(releasedPressureEach);
            return releasedPressure;
        }

        int ComparePaths_Part1((int pres, int time, string id) lhs, (int pres, int time, string id) rhs) {
            int lhs_presTime = lhs.pres / lhs.time,
                rhs_presTime = rhs.pres / rhs.time;
            int delta = Math.Abs(lhs_presTime - rhs_presTime);
            if( delta < 10 ) {
                int comp = lhs.time.CompareTo(rhs.time);
                if( comp == 0 ) {
                    return lhs_presTime.CompareTo(rhs_presTime);
                } else {
                    return comp;
                }
            } else {
                return rhs_presTime.CompareTo(lhs_presTime);
            }
        }

        int ComparePaths_Part2((int pres, int ppt, int time, string id, int rate, Actor self, Actor other) lhs, (int pres, int ppt, int time, string id, int rate, Actor self, Actor other) rhs) {
            Actor self = lhs.self, other = lhs.other;


            int lhsScore = lhs.time, rhsScore = rhs.time;

            int otherDelay = other.followPath.Count;
            if( otherDelay > 0 ) { otherDelay += 1; }

            ValveData otherLoc = valves[other.openNext ?? other.at];

            var otherLhs = otherLoc.timeToDestination.First(t => t.id == lhs.id);
            if (lhs.time < otherLhs.time + otherDelay) {
                lhsScore += lhsScore * (otherLhs.time - lhs.time);
            }
            
            var otherRhs = otherLoc.timeToDestination.First(t => t.id == rhs.id);
            if (rhs.time < otherRhs.time + otherDelay) {
                rhsScore += rhsScore * (otherRhs.time - rhs.time);
            }


            return lhsScore.CompareTo(rhsScore);
        }

        (ValveData next, int estPressure) PickNext(ValveData from, P2_State state, int? remainingOverride = null, HashSet<ValveData> path = null, int depth = 0) {
            int remaining = remainingOverride ?? state.remaining;
            path ??= new();
            path.Add(from);
            var possTotal = from.timeToDestination
                .Where(t => t.rate > 0)
                .Where(t => !state.openValves.ContainsKey(t.id))
                .Where(t => !state.openNextSet.Contains(t.id))
                .Select(t => (pres: t.rate * (remaining - t.time), v: valves[t.id], t.time))
                .Where(t => !path.Contains(t.v))
                .Where(t => t.pres > 0)
                .ToList();

            var poss = possTotal
                .Where(t => t.time < 4)
                .Select(t => (t.pres,
                    t.v,
                    t.time,
                    presPoss: depth < 2 ? PickNext(t.v, state, remaining - (t.time + 1), new(path), depth + 1) : (null, 0)))
                .OrderByDescending(t => t.pres + t.presPoss.estPressure)
                .ThenBy(t => t.time)
                .ToList();

            if (poss.Count == 0) {
                poss = possTotal
                    .Select(t => (t.pres,
                        t.v,
                        t.time,
                        presPoss: depth < 2 ? PickNext(t.v, state, remaining - (t.time + 1), new(path), depth + 1) : (null, 0)))
                    .OrderByDescending(t => t.pres + t.presPoss.estPressure)
                    .ThenBy(t => t.time)
                    .ToList();
            }
            var next = poss.FirstOrDefault();
            if (next == default)
                return default;
            else
                return (next.v, next.pres + next.presPoss.estPressure);
        }
        Dictionary<string, List<string>> PathToNodes(ValveData from) {
            Dictionary<string, List<string>> bestPaths = new Dictionary<string, List<string>>();

            Queue<(string id, List<string> path)> consider = new();
            consider.Enqueue((from.Id, new()));
            while(consider.Count > 0) {
                var (nextId, path) = consider.Dequeue();
                ValveData data = valves[nextId];
                var cachedPath = bestPaths.GetValueOrDefault(nextId);
                if( cachedPath is null || cachedPath.Count > path.Count ) {
                    // replace bestPath
                    bestPaths[nextId] = new(path);
                }
                foreach(var exit in data.exits) {
                    if (path.Contains(exit)) continue; // don't double back
                    consider.Enqueue((exit, new List<string>(path) { nextId }));
                }
            }
            return bestPaths;
        }
        class ValveData {
            public readonly string Id;
            public readonly int Rate;
            public readonly List<string> exits = new();
            public readonly List<(int time, int rate, string id)> timeToDestination = new();
            public Dictionary<string, List<string>> pathToOthers;

            public ValveData(string id, int rate) {
                Id = id;
                Rate = rate;
            }

            public override string ToString() {
                return string.Concat("ValveData -- Id: ", Id, " Rate: ", Rate);
            }
        }
    }
}
