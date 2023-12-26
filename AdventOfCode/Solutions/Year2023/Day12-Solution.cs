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
    void Print(List<GroupState> groups, int[] brokens, int indent = 0, bool? isAligned = null) {
        if (!OutputAlways) return;

        StringBuilder sb = new();
        for(int i = 0; i < indent; i++) {
            sb.Append('~');
        }

        foreach(var group in groups) {
            char c = StringFromState(group.state);
            for (int i = group.count; i > 0; i--) {
                sb.Append(c);
            }
        }

        sb.Append("   ");
        sb.Append(string.Join(',', GetBrokens(groups)));

        if ( brokens is not null) {
            sb.Append("   ");
            sb.Append(string.Join(',', brokens));
        }

        if( isAligned != null ) {
            sb.Append("   ");
            sb.Append(isAligned.ToString());
        }

        WriteLine(sb);
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
    class PermGroup {
        public SpringState type;
        public int start, min, max, count;
        public bool anchored;
        public int Delta => max - min;
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
    static int[] GetBrokens(List<GroupState> groups) {
        return groups.Where(g => g.state == SpringState.Broken).Select(g => g.count).ToArray();
    }

    protected override object SolvePartOneRaw()
    {
        return Solve(rows);
    }
    protected override object SolvePartTwoRaw() {
        return Solve(rows2);
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
        public int i;
        public SpringState[] state;
        public int broken;
        public int unknown;
        public List<GroupState> groups;
        public int atGroupIndex;
        public int atGuideIndex;

        public List<SolveIteration> children = new(2);
        public SolveIteration parent = null;
        public ulong rowPermutations;
        public SpringState MyState => i < 1 ? SpringState.OpSlide : state[i - 1];
        public int leftCount;

        public SolveIteration(int i, SpringState[] state, int broken, int unknown, List<GroupState> groups, int atGroupIndex, int atGuideIndex, SolveIteration parent)
        {
            this.i = i;
            this.state = state;
            this.broken = broken;
            this.unknown = unknown;
            this.groups = groups;
            this.atGroupIndex = atGroupIndex;
            this.atGuideIndex = atGuideIndex;
            this.leftCount = CountLeft();
            this.parent = parent;
            if( parent is not null )
                parent.children.Add(this);

        }
        int CountLeft() {
            var myState = MyState;
            int count = 0;
            for (int i = this.i - 1; i > -1 && i < state.Length; i--) {
                if (state[i] == myState) count++;
                else return count; // stop of first mismatch
            }
            return count;
        }

    }

    static bool GroupStateEquals(GroupState lhs, GroupState rhs) {
        return lhs.state == rhs.state && lhs.count == rhs.count && lhs.atIndex == rhs.atIndex;
    }

    string PrintIterTree(SolveIteration iter, int depth = 0) {
        StringBuilder sb = new(2000);

        // Print self
        sb.Append('|', depth)
            .Append('>')
            .Append(iter.state.Select(StringFromState).ToArray())
            .AppendLine($" -- @{iter.i,3} RP{iter.rowPermutations,5}")
            .Append('|', depth)
            .Append(' ', Math.Max(0, iter.i + 1))
            .AppendLine("^");
        
        foreach(var child in iter.children) {
            sb.Append(PrintIterTree(child, depth + 1));
        }
        return sb.ToString();
    }

    ulong SolveRow(Row row, bool enableCaching, bool printTree) {
        Stopwatch sw = new(), rowTotal = new();
        rowTotal.Start();
        long maxIterTicks = 0;
        ulong rowperms = 0, cacheHits = 0;

        var rowInitialStates = row.resolvedStates ?? row.initialStates;
        int ttlBroken = row.brokenGroups.Sum(), maxQueue = 0, length = rowInitialStates.Length;
        long iterations = 0;

        Dictionary<(int atIndex, int guideIndex, SpringState atState, int leftCount, int atGroupIndex), (ulong permutations, SolveIteration fromIter)> permutationCache = new();
        LinkedList<SolveIteration> forks = new();
        forks.AddLast(
            new SolveIteration(
                i: 0,
                state: CloneState(rowInitialStates),
                broken: rowInitialStates.Count(st => st == SpringState.Broken),
                unknown: rowInitialStates.Count(st => st == SpringState.Unknown),
                groups: GetGroups(rowInitialStates, null, 0),
                atGroupIndex: 0,
                atGuideIndex: 0,
                parent: null
            )
        );

        while (forks.Count > 0) {
            sw.Restart();
            iterations++; maxQueue = Math.Max(forks.Count, maxQueue);

            var iterStateNode = forks.First;
            var iter = iterStateNode.Value;
            forks.RemoveFirst();


            if (iter.atGuideIndex == row.brokenGroups.Length) {
                iter.rowPermutations++;

                if(enableCaching) {
                    var key = (iter.i - 1, iter.atGuideIndex, iter.state[iter.i - 1], iter.leftCount, iter.atGroupIndex);
                    if (permutationCache.ContainsKey(key)) {
                        var value = permutationCache[key];
                        if (value.permutations != iter.rowPermutations)
                            WriteLine($"At duplicate leaf to cache entry: {value} for key {key} -- my value {iter.rowPermutations}");
                        permutationCache[key] = (iter.rowPermutations, iter);
                    }
                    else permutationCache.Add(key, (iter.rowPermutations, iter));
                }
            }
            else if (iter.children.Count > 0) {
                foreach(var child in iter.children) {
                    iter.rowPermutations += child.rowPermutations;
                }
                if (!printTree) iter.children.Clear();

                if (iter.parent is null) {
                    rowperms += iter.rowPermutations;
                    // print the tree
                    if( printTree ) WriteLine(PrintIterTree(iter));
                }
                else if (enableCaching) {
                    var key = (iter.i, iter.atGuideIndex, iter.state[iter.i], iter.leftCount, iter.atGroupIndex);
                    if (permutationCache.ContainsKey(key)) {
                        var value = permutationCache[key];
                        if (value.permutations != iter.rowPermutations)
                            WriteLine($"Computed duplicate to cache entry: {value} for key {key} -- my value {iter.rowPermutations}");
                        permutationCache[key] = (iter.rowPermutations, iter);
                    }
                    else {
                        // remove entries below because they aren't relavant to other paths
                        //List<(int, int, SpringState, int, int)> evictions = new(permutationCache.Count);
                        //evictions.AddRange(permutationCache.Keys.Where(k => k.atIndex > iter.i));
                        //foreach (var k in evictions) permutationCache.Remove(k);

                        permutationCache.Add(key, (iter.rowPermutations, iter));
                    }
                }

            }
            else {
                int iterStart = iter.i - 1;
                // skip set state
                for (; iter.i < length && iter.state[iter.i] != SpringState.Unknown; iter.i++) ;

                if (iter.i < length) {// assume we're at a unknown, since we didn't walk off the end
                    bool queueOps = iter.broken + iter.unknown - 1 >= ttlBroken;
                    bool queueBroken = iter.broken + 1 <= ttlBroken;

                    SolveIteration opsIter = null, brokenIter = null;
                    // Queue as operational
                    if (queueOps) { // if selecting operational doesn't allow completion, don't walk that path
                        SpringState[] stateCopy = CloneState(iter.state);
                        stateCopy[iter.i] = SpringState.Operational;
                        var groups = GetGroups(stateCopy, iter.groups, iter.atGroupIndex);

                        bool resetIndex = !GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)]);
                        var (alignment, atSampleIndex, atGuideIndex) = WithinAlignment(groups, resetIndex ? 0 : iter.atGroupIndex, row.brokenGroups, resetIndex ? 0 : iter.atGuideIndex);

                        //if (OutputAlways) Print(groups, row.brokenGroups, 4, alignment);
                        if (alignment) { // only explore if this pathway is a viable match
                            opsIter = new SolveIteration(
                                    i: iter.i + 1,
                                    state: stateCopy,
                                    broken: iter.broken,
                                    unknown: iter.unknown - 1,
                                    groups: groups,
                                    atGroupIndex: atSampleIndex,
                                    atGuideIndex: atGuideIndex,
                                    parent: iter
                                ) {
                            };
                        }
                    }
                    // Queue as broken
                    if (queueBroken) { // if selecting broken here causes too many brokens don't walk that path
                        SpringState[] stateCopy = CloneState(iter.state);
                        stateCopy[iter.i] = SpringState.Broken;
                        var groups = GetGroups(stateCopy, iter.groups, iter.atGroupIndex);
                        bool resetIndex = !GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)]);
                        var (alignment, atSampleIndex, atGuideIndex) = WithinAlignment(groups, resetIndex ? 0 : iter.atGroupIndex, row.brokenGroups, resetIndex ? 0 : iter.atGuideIndex);
                        //if (OutputAlways) Print(groups, row.brokenGroups, 4, alignment);
                        if (alignment) { // only explore if this pathway is a viable match
                            brokenIter = new SolveIteration(
                                    i: iter.i + 1,
                                    state: stateCopy,
                                    broken: iter.broken + 1,
                                    unknown: iter.unknown - 1,
                                    groups: groups,
                                    atGroupIndex: atSampleIndex,
                                    atGuideIndex: atGuideIndex,
                                    parent: iter
                                ) {
                            };
                        }
                    }


                    void QueueFork(SolveIteration siter) {
                        if( siter is not null ) {
                            var key = (siter.i - 1, siter.atGuideIndex, siter.state[siter.i - 1], siter.leftCount, iter.atGroupIndex);
                            if (permutationCache.TryGetValue(key, out var cachedRowPerms)) {
                                siter.rowPermutations = cachedRowPerms.permutations;
                                cacheHits++;
                            } else {
                                forks.AddFirst(siter);
                            }
                        }
                    }
                    if( opsIter is not null || brokenIter is not null ) {
                        iter.i = iterStart; // rewind i to the fork we started at.
                        forks.AddFirst(iter);
                        QueueFork(opsIter);
                        QueueFork(brokenIter);
                    }
                }
            }

            maxIterTicks = Math.Max(sw.ElapsedTicks, maxIterTicks);
        }

        WriteLine($"rowperms: {rowperms} --- iterations {iterations} -- maxQueue {maxQueue} -- cacheHits {cacheHits} -- maxIterTicks {maxIterTicks} --- {rowTotal.Elapsed}");
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
                //_ = SolveRow(row, false, true);
                WriteLine(" ^ BROKEN");
                return null;
            }
            perm += cacheResult;
        }


         return perm;
    }
}
