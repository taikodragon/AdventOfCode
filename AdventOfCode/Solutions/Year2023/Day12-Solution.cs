using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public SolveIteration(int i, SpringState[] state, int broken, int unknown, List<GroupState> groups, int atGroupIndex, int atGuideIndex)
        {
            this.i = i;
            this.state = state;
            this.broken = broken;
            this.unknown = unknown;
            this.groups = groups;
            this.atGroupIndex = atGroupIndex;
            this.atGuideIndex = atGuideIndex;
        }
    }

    static bool GroupStateEquals(GroupState lhs, GroupState rhs) {
        return lhs.state == rhs.state && lhs.count == rhs.count && lhs.atIndex == rhs.atIndex;
    }
    object Solve(List<Row> myRows)
    {
        WriteLine("Start Solve...");
       ulong perm = 0;


        foreach (var row in myRows) {
            Stopwatch sw = new(), rowTotal = new();
            rowTotal.Start();
            long maxIterTicks = 0;
            ulong rowperms = 0;
            int ttlBroken = row.brokenGroups.Sum(), maxQueue = 0, length = row.initialStates.Length;
            long iterations = 0;
            LinkedList<SolveIteration> forks = new();
            forks.AddLast(
                new SolveIteration(
                    i: 0,
                    state: CloneState(row.initialStates),
                    broken: row.initialStates.Count(st => st == SpringState.Broken),
                    unknown: row.initialStates.Count(st => st == SpringState.Unknown),
                    groups: GetGroups(row.initialStates, null, 0),
                    atGroupIndex: 0,
                    atGuideIndex: 0
                )
            );

            while (forks.Count > 0) {
                sw.Restart();
                iterations++; maxQueue = Math.Max(forks.Count, maxQueue);

                var iterStateNode = forks.First;
                var iter = iterStateNode.Value;
                forks.RemoveFirst();
                //if (OutputAlways && printed != forks.Count) {
                //    WriteLine($"{i,3}/{length,3} -- {forks.Count,3}");
                //    printed = forks.Count;
                //}

                if (iter.atGuideIndex == row.brokenGroups.Length) {
                    rowperms++;
                }
                else {
                    // skip set state
                    for (; iter.i < length && iter.state[iter.i] != SpringState.Unknown; iter.i++) ;
                    if (iter.i < length) {// assume we're at a broken, since we didn't walk off the end
                        bool queueOps = iter.broken + iter.unknown - 1 >= ttlBroken;
                        bool queueBroken = iter.broken + 1 <= ttlBroken;
                        // Queue as operational
                        if (queueOps) { // if selecting operational doesn't allow completion, don't walk that path
                            SpringState[] stateCopy = queueBroken ? CloneState(iter.state) : iter.state; // don't copy when not double queuing
                            stateCopy[iter.i] = SpringState.Operational;
                            var groups = GetGroups(stateCopy, iter.groups, iter.atGroupIndex);
                            if (!GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)])) {
                                iter.atGroupIndex = 0; iter.atGuideIndex = 0;
                            }
                            var (alignment, atSampleIndex, atGuideIndex) = WithinAlignment(groups, iter.atGroupIndex, row.brokenGroups, iter.atGuideIndex);
                            //if (OutputAlways) Print(groups, row.brokenGroups, 4, alignment);
                            if (alignment) { // only explore if this pathway is a viable match
                                forks.AddFirst(
                                    new SolveIteration(
                                        i: iter.i + 1,
                                        state: stateCopy,
                                        broken: iter.broken,
                                        unknown: iter.unknown - 1,
                                        groups: groups,
                                        atGroupIndex: atSampleIndex,
                                        atGuideIndex: atGuideIndex
                                    )
                                );
                            }

                        }
                        // Queue as broken
                        if (queueBroken) { // if selecting broken here causes too many brokens don't walk that path
                            iter.state[iter.i] = SpringState.Broken;
                            var groups = GetGroups(iter.state, iter.groups, iter.atGroupIndex);
                            if (!GroupStateEquals(groups[Math.Max(0, iter.atGroupIndex - 1)], iter.groups[Math.Max(0, iter.atGroupIndex - 1)])) {
                                iter.atGroupIndex = 0; iter.atGuideIndex = 0;
                            }
                            var (alignment, atSampleIndex, atGuideIndex) = WithinAlignment(groups, iter.atGroupIndex, row.brokenGroups, iter.atGuideIndex);
                            //if (OutputAlways) Print(groups, row.brokenGroups, 4, alignment);
                            if (alignment) { // only explore if this pathway is a viable match
                                forks.AddFirst(new SolveIteration(
                                        i: iter.i + 1,
                                        state: iter.state,
                                        broken: iter.broken + 1,
                                        unknown: iter.unknown - 1,
                                        groups: groups,
                                        atGroupIndex: atSampleIndex,
                                        atGuideIndex: atGuideIndex
                                    )
                                );
                            }
                        }
                    }
                }

                maxIterTicks = Math.Max(sw.ElapsedTicks, maxIterTicks);
            }

            WriteLine($"rowperms: {rowperms} --- iterations {iterations} -- maxQueue {maxQueue} -- maxIterTicks {maxIterTicks} --- {rowTotal.Elapsed}");
            perm += rowperms;
        }


        return perm;//rows.Sum(RowPermutations);
    }
}
