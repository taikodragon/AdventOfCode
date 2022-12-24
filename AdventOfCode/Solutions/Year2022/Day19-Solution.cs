using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Networking;

namespace AdventOfCode.Solutions.Year2022
{


    [DayInfo(2022, 19, "")]
    class Day19 : ASolution
    {
        const string Ore = "ore", Clay = "cla", Obsidian = "obs", Geode = "geo";
        const string OreBot = "zb-ore", ClayBot = "zb-cla", ObsidianBot = "zb-obs", GeodeBot = "zb-geo";
        const int MaxMinutes = 24;

        public Day19() : base(true )
        {
            
        }

        List<Blueprint> blueprints = new();
        protected override void ParseInput() {
            var regex = new Regex("([0-9]+)", RegexOptions.Compiled);
            foreach(var line in Input.SplitByNewline(false, true)) {
                var matches = regex.Matches(line);
                var numbers = matches
                    .Where(m => m.Success)
                    .Select(m => int.Parse(m.Groups[1].Value))
                    .ToList();

                var bp = new Blueprint(numbers[0]);
                bp.Costs[OreBot].Add(new(Ore) { Amount = numbers[1] });
                bp.Costs[ClayBot].Add(new(Ore) { Amount = numbers[2] });
                bp.Costs[ObsidianBot].Add(new(Ore) { Amount = numbers[3] });
                bp.Costs[ObsidianBot].Add(new(Clay) { Amount = numbers[4] });
                bp.Costs[GeodeBot].Add(new(Ore) { Amount = numbers[5] });
                bp.Costs[GeodeBot].Add(new(Obsidian) { Amount = numbers[6] });

                bp.maxCost[0] = Math.Max(Math.Max(Math.Max(numbers[1], numbers[2]), numbers[3]), numbers[5]);
                bp.maxCost[1] = numbers[4];
                bp.maxCost[2] = numbers[6];

                blueprints.Add(bp);
            }
        }

        List<string> bots = new() { OreBot, ClayBot, ObsidianBot, GeodeBot };
        protected override object SolvePartOneRaw()
        {
            Dictionary<int, int> scores = new();

            foreach(var bp in blueprints) {
                bp.Reset();

                int maxGeodes = 0;
                Dictionary<int, (int geodes, string construct)> bestGeodesByPath = new();
                Queue<(int minute, string construct, Blueprint bpi)> pathWays = new();
                HashSet<(int minute, string bot, int ore, int clay, int obsidian, int geodes, int oreBots, int clayBots, int obsBots, int geoBots)> visited = new();
                pathWays.Enqueue((1, null, bp.Clone())); // first minute we can never construct a bot

                void EnqueuePath(int minute, string nextConstruct, string nextBot, Blueprint bpClone) {
                    var bpNext = bpClone.Clone();
                    if (nextConstruct is not null && bpNext.CanConstruct(nextConstruct)) {
                        bpNext.BeginConstructBot(nextConstruct);
                    }
                    bpNext.BotTick(minute);
                    bpNext.EndConstructBot(minute);
                    var state = (minute,
                        nextBot,
                        bpNext.Resources[Ore], bpNext.Resources[Clay], bpNext.Resources[Obsidian], bpNext.Resources[Geode],
                        bpNext.Resources[OreBot], bpNext.Resources[ClayBot], bpNext.Resources[Obsidian], bpNext.Resources[GeodeBot]
                        );

                    if (!visited.Contains(state)) {
                        pathWays.Enqueue((minute + 1, nextBot, bpNext));
                        visited.Add(state);
                    }
                }

                while(pathWays.Count > 0) {
                    var (minute, nextConstruct, bpClone) = pathWays.Dequeue();

                    if ( minute > MaxMinutes ) {

                        maxGeodes = Math.Max(maxGeodes, bpClone.CrackedGeodes);
                        continue;
                    }

                    // queue building some bot
                    foreach(var bot in bots) {
                        if( bpClone.CanConstruct(bot) ) {
                            EnqueuePath(minute, nextConstruct, bot, bpClone);
                        }
                    }

                    EnqueuePath(minute, nextConstruct, null, bpClone);
                }

                scores[bp.Id] = bp.Id * maxGeodes;
            }

            return null;
        }

        protected override object SolvePartTwoRaw()
        {
            return null;
        }


        //void SimulateMinute(Blueprint bp, int fromMinute, string attemptConstruct) {
        //    if (fromMinute > MaxMinutes) return;
        //    WriteLine(string.Concat("== Minute ", fromMinute, " =="));

        //    // Choose something to construct, if possible
        //    string constructing = bp.CanConstruct(attemptConstruct) ? attemptConstruct : throw new Exception("asked to construct and cannot");
        //    if( constructing is not null ) {
        //        WriteLine(string.Concat("Starting construct ", constructing, " consuming: ", string.Join('\t', bp.Costs[constructing])));
        //        bp.BeginConstructBot(constructing);
        //    }

        //    // Bot tick
        //    bp.BotTick();
        //    WriteLine(string.Concat(bp.Resources[OreBot], "\tOre bot collects\t", bp.Resources[Ore]));
        //    WriteLine(string.Concat(bp.Resources[ClayBot], "\tCla bot collects\t", bp.Resources[Clay]));
        //    WriteLine(string.Concat(bp.Resources[ObsidianBot], "\tObs bot collects\t", bp.Resources[Obsidian]));
        //    WriteLine(string.Concat(bp.Resources[GeodeBot], "\tGeo bot collects\t", bp.Resources[Geode]));

        //    if(constructing is not null ) {
        //        WriteLine(string.Concat(bp.Constructing, " now available"));
        //        bp.EndConstructBot(); // no-ops if there is not a bot constructing
        //    }
        //}

        class Resource {
            public readonly string Name;
            public int Amount = 0;

            public Resource(string name) {
                this.Name = name;
            }

            public override string ToString() {
                return string.Concat(Amount, " ", Name);
            }
        }
        class Blueprint {
            public readonly int Id;
            public Dictionary<string, List<Resource>> Costs = new() {
                { OreBot, new() },
                { ClayBot, new() },
                { ObsidianBot, new() },
                { GeodeBot, new() },
            };
            public readonly Dictionary<string, int> Resources = new() {
                { Ore, 0 },
                { Clay, 0},
                { Obsidian, 0},
                { Geode, 0},
                { OreBot, 1 },
                { ClayBot, 0 },
                { ObsidianBot, 0 },
                { GeodeBot, 0 },
            };
            public List<(int minute, string construct)> path = new();
            public int[] maxCost = new int[3];
            string constructing;

            public int CrackedGeodes => Resources[Geode];
            public string Constructing => constructing;

            public Blueprint(int id) {
                Id = id;
            }

            public void Reset() {
                foreach(var key in Resources.Keys) {
                    Resources[key] = 0;
                }
                Resources[OreBot] = 1;
                constructing = null;
            }
            public Blueprint Clone() {
                var copy = new Blueprint(Id) {
                    Costs = Costs
                };
                copy.maxCost = maxCost;
                foreach(var key in Resources.Keys) {
                    copy.Resources[key] = Resources[key];
                }
                copy.path.AddRange(path);
                return copy;
            }

            public bool CanConstruct(string botName) {
                return Costs[botName].TrueForAll(r => Resources[r.Name] >= r.Amount);
            }
            public void BeginConstructBot(string botName) {
                foreach(var res in Costs[botName]) {
                    Resources[res.Name] -= res.Amount;
                    if (Resources[res.Name] < 0) throw new Exception($"Over-spent resource for {botName}, bp {Id}, res {res.Name}");
                }
                constructing = botName;
            }
            public void EndConstructBot(int atMinute) {
                path.Add((atMinute, constructing));
                if( constructing is not null ) {
                    Resources[constructing]++;
                    constructing = null;
                }
            }


            public void BotTick(int atMinute) {
                atMinute = MaxMinutes - atMinute; // minutes remaining
                Resources[Ore] = Math.Min(maxCost[0] * atMinute, Resources[Ore] + Resources[OreBot]);
                Resources[Clay] = Math.Min(maxCost[1] * atMinute, Resources[Clay] + Resources[ClayBot]);
                Resources[Obsidian] = Math.Min(maxCost[2] * atMinute, Resources[Obsidian] + Resources[ObsidianBot]);
                Resources[Geode] += Resources[GeodeBot];
            }
        }
    }
}
