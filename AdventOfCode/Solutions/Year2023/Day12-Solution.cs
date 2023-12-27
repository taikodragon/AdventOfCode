using Newtonsoft.Json.Bson;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 12, "Hot Springs")]
class Day12 : ASolution
{
    public Day12() : base(false)
    {
        OutputAlways = true;
    }

    public enum SpringState : byte {
        Unknown,
        Operational,
        Broken,
        OpSlide
    };
    static SpringState StateFromChar(char c) {
        return c switch {
            '.' => SpringState.Operational,
            '#' => SpringState.Broken,
            '?' => SpringState.Unknown,
            _ => throw new Exception("Unknown sprint state: " + c)
        };
    }
    static char StringFromState(SpringState state) {
        return state switch {
            SpringState.Operational => '.',
            SpringState.Broken => '#',
            SpringState.Unknown => '?',
            SpringState.OpSlide => '-',
            _ => throw new Exception("Unknown state value: " + state)
        };
    }

    class GroupState {
        public int atIndex;
        public SpringState state;
        public int count;

        public GroupState(int atIndex, SpringState state, int count)
        {
            this.atIndex = atIndex;
            this.state = state;
            this.count = count;
        }

        public override string ToString() {
            return $"{nameof(GroupState)}:{state} @{atIndex}--{count}";
        }
    }

    class Row {
        public string raw;
        public SpringState[] initialStates;
        public int[] brokenGroups;
        public SpringState[] resolvedStates;
    }

    List<Row> rows, rows2;

    protected override void ParseInput()
    {
        rows = Input.SplitByNewline(false, true)
            .Select(l => l.Split(' ', ','))
            .Select(p => new Row {
                raw = p[0],
                initialStates = p[0].Select(StateFromChar).ToArray(),
                brokenGroups = p[1..].Select(int.Parse).ToArray(),
            })
            .ToList();

        rows2 = Input.SplitByNewline(false, true)
            .Select(l => l.Split(' '))
            .Select(p => new Row {
                raw = p[0],
                initialStates = string.Join('?', p[0], p[0], p[0], p[0], p[0]).Select(StateFromChar).ToArray(),
                brokenGroups = string.Join(',', p[1], p[1], p[1], p[1], p[1]).Split(',').Select(int.Parse).ToArray(),
            })
            .ToList();

    }


    static List<GroupState> GetGroups(SpringState[] states, List<GroupState> fromGroups, int startFromGroup) {
        List<GroupState> groups;
        int counter;
        SpringState lastState;
        int i;
        
        if(fromGroups is null || startFromGroup <= 0) {
            groups = new();
            counter = 0;
            lastState = SpringState.Operational;
            i = 0;
        } else {
            groups = fromGroups[..(startFromGroup - 1)];
            counter = fromGroups[startFromGroup - 1].count;
            lastState = fromGroups[startFromGroup - 1].state;
            i = fromGroups[startFromGroup].atIndex;
        }

        for(; i < states.Length; i++) {
            var s = states[i];
            if (s != lastState) {
                if (counter > 0) {
                    groups.Add(new(i - counter, lastState, counter));
                }
                counter = 1;
                lastState = s;
            }
            else counter++;
        }
        if (counter > 0) {
            groups.Add(new(i - counter, lastState, counter));
        }
        return groups;
    }

    protected override object SolvePartOneRaw()
    {
        return Solve(rows);
    }
    protected override object SolvePartTwoRaw() {
        //return Solve(rows2);
        return null;
    }

    static (bool result, int atSampleIndex, int atGuideIndex) WithinAlignment(List<GroupState> sample, int fromSampleIndex, int[] guide, int guideIndex) {
        guideIndex = Math.Max(0, guideIndex);
        for (int i = Math.Max(0, fromSampleIndex); i < sample.Count && guideIndex < guide.Length; i++) {
            if (sample[i].state == SpringState.Broken) {
                if (sample[i].count < guide[guideIndex]) {
                    return (i + 1 < sample.Count && sample[i + 1].state == SpringState.Unknown, i, guideIndex);
                }
                else if (sample[i].count == guide[guideIndex])
                    guideIndex++;
                else return (false, i, guideIndex); // exceeded guide count
            }
            // don't limit based on groups at or after unknowns
            else if (sample[i].state == SpringState.Unknown) return (true, i, guideIndex);
        }
        return (true, sample.Count - 1, guideIndex);
    }
    static SpringState[] CloneState(SpringState[] fromState) {
        SpringState[] newState = new SpringState[fromState.Length];
        Array.Copy(fromState, newState, fromState.Length);
        return newState;
    }

    class SolveIteration {
        public readonly int forkedAt, broken, unknown, atGuideIndex, atGroupIndex;
        public SpringState[] state;
        public List<GroupState> groups;

        public readonly SolveIteration parent = null;

        private List<SolveIteration> children = new(2);
        private readonly int myGroupIndex;
        private ulong _rowPermutations;
        public ulong RowPermutations => _rowPermutations;
        public SpringState MyType => forkedAt >= 0 ? state[forkedAt] : SpringState.Unknown;
        public bool cacheEnabled = true;
        public List<SolveIteration> printChildren;
        public bool IsSolved { get; private set; }

        public SolveIteration(int forkedAt, SpringState[] state, int broken, int unknown, List<GroupState> groups, int atGroupIndex, int atGuideIndex, SolveIteration parent)
        {
            this.forkedAt = forkedAt;
            this.state = state;
            this.broken = broken;
            this.unknown = unknown;
            this.groups = groups;
            this.atGroupIndex = atGroupIndex;
            this.atGuideIndex = atGuideIndex;
            this.parent = parent;
            if (parent is not null) {
                parent.children.Add(this);
                cacheEnabled = parent.cacheEnabled;
                if( parent.printChildren is not null ) {
                    parent.printChildren.Add(this);
                    printChildren = new(2);
                }
            }

            myGroupIndex = groups.FindIndex(Math.Max(0, parent?.myGroupIndex ?? 0), gs => gs.atIndex <= forkedAt && (gs.atIndex + gs.count) > forkedAt);
            
        }

        public (int forkedAt, int nthGroup, SpringState groupType, int nthGroupSize, int atGuideIndex) GetCacheKey() {
            if( forkedAt < 0 ) {
                return (forkedAt, myGroupIndex, SpringState.Unknown, 0, 0);
            }
            return (forkedAt, myGroupIndex, groups[myGroupIndex].state, groups[myGroupIndex].count, atGuideIndex);
        }

        public void Solved(ulong rowPermutations, bool fromCache, Dictionary<(int,int,SpringState,int,int), (ulong,SolveIteration)> cache) {
            if (IsSolved) throw new InvalidOperationException("Row already has been solved.");

            _rowPermutations += rowPermutations;
            IsSolved = true;

            if( cacheEnabled && !fromCache ) {
                var key = GetCacheKey();
                if (cache.TryGetValue(key, out var value)) {
                    if (value.Item2 != this) {
                        Debugger.Break();
                        throw new Exception("Duplucate cache entry!");
                    }
                }
                else cache.Add(key, (_rowPermutations, this));
            }

            parent?.SolvedChild(this, fromCache, cache);
        }
        private void SolvedChild(SolveIteration child, bool fromCache, Dictionary<(int, int, SpringState, int, int), (ulong, SolveIteration)> cache) {
            if (!children.Remove(child)) throw new Exception("Unknown caller to SolvedChild"); // remove reference to prune memory usage
            // accumulate child arrangements
            _rowPermutations += child._rowPermutations;

            if( children.Count == 0 ) {
                if (cacheEnabled) {
                    var key = GetCacheKey();
                    if (cache.TryGetValue(key, out var value)) {
                        if (value.Item2 != this) {
                            Debugger.Break();
                            throw new Exception("Duplucate cache entry!");
                        }
                    }
                    else cache.Add(key, (_rowPermutations, this));
                }

                IsSolved = true;
                parent?.SolvedChild(this, fromCache, cache);
            }

        }
    }

    static bool GroupStateEquals(GroupState lhs, GroupState rhs) {
        return lhs.state == rhs.state && lhs.count == rhs.count && lhs.atIndex == rhs.atIndex;
    }

    string PrintIterTree(SolveIteration iter, int depth = 0, StringBuilder builder = null) {
        StringBuilder sb = builder ?? new(16 * 1024);

        // Print self
        sb.Append('|', depth)
            .Append('>')
            .Append(iter.state.Select(StringFromState).ToArray())
            .AppendLine($" -- @{iter.forkedAt,3} RP{iter.RowPermutations,5} Solved {iter.IsSolved}")
            .Append('|', depth + 1)
            .Append(' ', Math.Max(0, iter.forkedAt))
            .AppendLine("^");

        foreach (var child in iter.printChildren) {
            _ = PrintIterTree(child, depth + 1, sb);
        }
        return builder is null ? sb.ToString() : string.Empty;
    }

    ulong SolveRow(Row row, bool enableCaching, bool printTree) {
        Stopwatch sw = new(), rowTotal = new();
        rowTotal.Start();

        TimeSpan maxIterTime = TimeSpan.Zero;
        ulong rowperms = 0, cacheHits = 0;

        var rowInitialStates = row.resolvedStates ?? row.initialStates;
        int ttlBroken = row.brokenGroups.Sum(), maxQueue = 0, length = rowInitialStates.Length;
        long iterations = 0;

        Dictionary<(int forkedAt, int nthGroup, SpringState groupType, int nthGroupSize, int atGuideIndex), (ulong permutations, SolveIteration fromIter)> permutationCache = new();
        LinkedList<SolveIteration> forks = new();
        SolveIteration root;
        forks.AddLast(
            root = new SolveIteration(
                forkedAt: -1,
                state: CloneState(rowInitialStates),
                broken: rowInitialStates.Count(st => st == SpringState.Broken),
                unknown: rowInitialStates.Count(st => st == SpringState.Unknown),
                groups: GetGroups(rowInitialStates, null, 0),
                atGroupIndex: 0,
                atGuideIndex: 0,
                parent: null
            ) {
                cacheEnabled = enableCaching,
                printChildren = printTree ? new(2) : null
            }
        );

        void QueueFork(SolveIteration siter) {
            if (siter is not null) {
                var key = siter.GetCacheKey();
                if (enableCaching && permutationCache.TryGetValue(key, out var cachedRowPerms)) {
                    siter.Solved(cachedRowPerms.permutations, true, permutationCache);
                    cacheHits++;
                }
                else if (siter.atGuideIndex == row.brokenGroups.Length) { // this row is solved
                    siter.Solved(1, false, permutationCache);
                }
                else {
                    forks.AddFirst(siter);
                }
            }
        }

        while (forks.Count > 0) {
            sw.Restart();
            iterations++; maxQueue = Math.Max(forks.Count, maxQueue);

            var iterStateNode = forks.First;
            var iter = iterStateNode.Value;
            forks.RemoveFirst();

            if (iter.atGuideIndex == row.brokenGroups.Length) {
                iter.Solved(1, false, permutationCache);

            }
            else {
                int iterAt = iter.forkedAt + 1;
                // skip set state
                for (; iterAt < length && iter.state[iterAt] != SpringState.Unknown; iterAt++) ;

                if (iterAt < length) {// assume we're at a unknown, since we didn't walk off the end
                    SolveIteration opsIter = null, brokenIter = null;
                    // Queue as operational
                    SpringState[] stateCopy = CloneState(iter.state);
                    stateCopy[iterAt] = SpringState.Operational;
                    var groups = GetGroups(stateCopy, iter.groups, iter.atGroupIndex);

                    bool resetIndex = !GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)]);
                    var opsAlignment = WithinAlignment(groups, resetIndex ? 0 : iter.atGroupIndex, row.brokenGroups, resetIndex ? 0 : iter.atGuideIndex);

                    opsIter = new SolveIteration(
                            forkedAt: iterAt,
                            state: stateCopy,
                            broken: iter.broken,
                            unknown: iter.unknown - 1,
                            groups: groups,
                            atGroupIndex: opsAlignment.atSampleIndex,
                            atGuideIndex: opsAlignment.atGuideIndex,
                            parent: iter
                        ) {
                    };


                    // Queue as broken
                    stateCopy = CloneState(iter.state);
                    stateCopy[iterAt] = SpringState.Broken;
                    groups = GetGroups(stateCopy, iter.groups, iter.atGroupIndex);
                    resetIndex = !GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)]);
                    var brokenAlignment = WithinAlignment(groups, resetIndex ? 0 : iter.atGroupIndex, row.brokenGroups, resetIndex ? 0 : iter.atGuideIndex);
                    //if (OutputAlways) Print(groups, row.brokenGroups, 4, alignment);
                    brokenIter = new SolveIteration(
                            forkedAt: iterAt,
                            state: stateCopy,
                            broken: iter.broken + 1,
                            unknown: iter.unknown - 1,
                            groups: groups,
                            atGroupIndex: brokenAlignment.atSampleIndex,
                            atGuideIndex: brokenAlignment.atGuideIndex,
                            parent: iter
                        ) {
                    };

                    // IMPORTANT! All children must be added before calling solved on those children
                    if (opsAlignment.result) { // only explore if this pathway is a viable match
                        QueueFork(opsIter);
                    }
                    else
                        opsIter.Solved(0, permutationCache.ContainsKey(opsIter.GetCacheKey()), permutationCache);

                    if (brokenAlignment.result) { // only explore if this pathway is a viable match
                        QueueFork(brokenIter);
                    }
                    else
                        brokenIter.Solved(0, permutationCache.ContainsKey(brokenIter.GetCacheKey()), permutationCache);
                }
                else {
                    bool aligned = row.brokenGroups.SequenceEqual(iter.groups.Where(gs => gs.state == SpringState.Broken).Select(gs => gs.count));
                    if (aligned)
                        iter.Solved(1, false, permutationCache);
                    else
                        iter.Solved(0, false, permutationCache);
                }
            }

            var iterTime = sw.Elapsed;
            if (maxIterTime < iterTime) maxIterTime = iterTime;
        }

        if (printTree) WriteLine(PrintIterTree(root));
        rowperms = root.RowPermutations;
        WriteLine($"rowperms: {rowperms} --- iterations {iterations} -- maxQueue {maxQueue} -- cacheHits {cacheHits} -- maxIter {maxIterTime} --- {rowTotal.Elapsed}");
        return rowperms;
    }
    object Solve(List<Row> myRows)
    {
        WriteLine("Start Solve...");
       ulong perm = 0;


        foreach (var row in myRows) {
            WriteLine(row.raw);
            var cacheResult = SolveRow(row, true, false);
            var nocacheResult = SolveRow(row, false, false);
            if (cacheResult != nocacheResult) {
                WriteLine(" ^ BROKEN");
                _ = SolveRow(row, true, true);
                _ = SolveRow(row, false, true);
                WriteLine(" ^ BROKEN");
                return null;
            }
            perm += cacheResult;
        }


         return perm;
    }
}
