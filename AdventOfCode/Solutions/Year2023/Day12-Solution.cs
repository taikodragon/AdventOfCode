using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Windows.Devices.Input.Preview;
using static AdventOfCode.Solutions.Year2023.Day12;

namespace AdventOfCode.Solutions.Year2023;
using GroupState = (int atIndex, SpringState state, int count);

[DayInfo(2023, 12, "Hot Springs")]
class Day12 : ASolution
{
    public Day12() : base(false)
    {
        //OutputAlways = true;
    }

    public enum SpringState {
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
    void Print(List<GroupState> groups, int[] brokens, int indent = 0) {
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
        WriteLine(sb);
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

    List<Row> rows;

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
        //rows.ForEach(FixInitals);
    }


    static List<GroupState> GetGroups(IEnumerable<SpringState> states) {
        int counter = 0;
        SpringState lastState = SpringState.Operational;
        List<GroupState> groups = new();
        int i = 0;
        foreach(var s in states) {
            if (s != lastState) {
                if (counter > 0) {
                    groups.Add((i - counter, lastState, counter));
                }
                counter = 1;
                lastState = s;
            }
            else counter++;
            i++;
        }
        if (counter > 0) {
            groups.Add((i - counter, lastState, counter));
        }
        return groups;
    }
    static int[] GetBrokens(List<GroupState> groups) {
        return groups.Where(g => g.state == SpringState.Broken).Select(g => g.count).ToArray();
    }

    static int CountMatching(List<SpringState> states, int fromI, SpringState matching, int iter, bool inverse = false) {
        int count = 0;
        for (int i = fromI; i > -1 && i < states.Count; i += iter) {
            if (inverse) {
                if (states[i] != matching) count++;
                else return count; // stop of first mismatch
            }
            else {
                if (states[i] == matching) count++;
                else return count; // stop of first mismatch
            }
        }
        return count;
    }
    static bool IsAnchored(Row row, int groupMin, int groupMax) {
        for (int i = groupMin; i <= groupMax; i++) {
            if (row.initialStates[i] == SpringState.Broken) return true;
        }
        return false;
    }

    void FixInitals(Row row) {
        // .??..??...?##. 1,1,3
        List<SpringState> states = new(row.initialStates);
        List<(int groupMin, int groupMax)> foundGroups = new();
        int brokenCount = 0;
        WriteLine("---");
        Print(GetGroups(row.initialStates), row.brokenGroups);



        bool didChange = true;
        while (didChange) {
            didChange = false;
            brokenCount = 0;
            for (int i = 0; i < states.Count && brokenCount < row.brokenGroups.Length; i++) {
                var thisBrokenCount = row.brokenGroups[brokenCount];
                var thisState = states[i];
                // we only care about broken group to fix
                int backCount = CountMatching(states, i - 1, SpringState.Broken, -1);
                int foreCount = CountMatching(states, i + 1, SpringState.Broken, 1);
                if (thisState == SpringState.Broken) {
                    if ((backCount + foreCount + 1) == thisBrokenCount) {
                        foundGroups.Add((i - backCount, i + foreCount));
                        brokenCount++;
                        i += foreCount; // skip to end of group
                        continue;
                    }
                    // nothing else to do with brokens until we fill the group
                    continue;
                }
                if (thisState == SpringState.Unknown) {
                    if (foreCount > 0 && foreCount == thisBrokenCount) {
                        // we're just before the next group
                        states[i] = IsAnchored(row, i + 1, i + 1 + foreCount) ? SpringState.Operational : SpringState.OpSlide;
                        didChange = true;
                        continue;
                    }
                    if (backCount > 0 && brokenCount > 0
                        && backCount == row.brokenGroups[brokenCount - 1]
                        && foundGroups[brokenCount - 1].groupMax == i - 1) {
                        // we're just after the next group
                        states[i] = IsAnchored(row, i - backCount, i - 1) ? SpringState.Operational : SpringState.OpSlide;
                        didChange = true;
                        continue;
                    }
                    if ((backCount > 0 && foreCount == 0) || (foreCount > 0 && backCount == 0)) {
                        // extend group to fill need
                        states[i] = SpringState.Broken; i--;
                        didChange = true;
                        continue;
                    }
                    if ((foreCount + backCount + 1) <= thisBrokenCount) {
                        int possibleFore = CountMatching(states, i + 1, SpringState.Operational, 1,inverse: true);
                        if( (backCount + 1 + possibleFore) >= thisBrokenCount ) {
                            // bridge group to complete fill
                            states[i] = SpringState.Broken; i--;
                            didChange = true;
                            continue;
                        }
                    }
                }
                if (thisState == SpringState.OpSlide
                    && foreCount > 0 && backCount > 0
                    && (IsAnchored(row, i - backCount, i - 1) || IsAnchored(row, i + 1, i + foreCount))) {
                    // only if either are anchored
                    // I'm sandwiched, switch to just a normal operation
                    states[i] = SpringState.Operational;
                    // this doesn't count as a change because opslides are opertional
                    continue;
                }
            }
            Print(GetGroups(states), row.brokenGroups);

        }
        for(int i = 0; i < states.Count; i++) {
            if (states[i] == SpringState.Unknown) states[i] = SpringState.OpSlide;
        }

        int ops = states.Count(s => s == SpringState.OpSlide);
        WriteLine($"Ops = {ops}");

        row.resolvedStates = states.ToArray();
    }



    int RowPermutations(Row row) {
        // Build up a list of groups that have uncertainty
        List<List<PermGroup>> unknownGroups = new();

        {
            List<SpringState> resolvedStates = new(row.resolvedStates);

            for(int i = 0; i < row.initialStates.Length; i++) {
                if(row.initialStates[i] == SpringState.Unknown) {
                    int groupStart = i;
                    List<PermGroup> groups = new();
                    PermGroup group = null;

                    // Check for empty group before
                    //if( i == 0 || row.resolvedStates[i - 1] != SpringState.Broken) {
                    //    groups.Add(group = new() {
                    //        start = i - 1,
                    //        type = SpringState.OpSlide,
                    //        min = 0
                    //    });
                    //}

                    for(; i < row.initialStates.Length && row.initialStates[i] == SpringState.Unknown; i++) {
                        var resolvedType = row.resolvedStates[i];
                        if (group is null || group.type != resolvedType) {
                            var oldType = group?.type ?? SpringState.Unknown;
                            group = new PermGroup {
                                start = i,
                                type = resolvedType
                            };
                            // Check for anchored broken group, 
                            if (resolvedType == SpringState.Broken) {
                                int backGroup = CountMatching(resolvedStates, i - 1, SpringState.Broken, -1);
                                int foreGroup = CountMatching(resolvedStates, i + 1, SpringState.Broken, 1);

                                if (backGroup + foreGroup > 0) {
                                    group.anchored = IsAnchored(row, i - backGroup, i + foreGroup);
                                }
                            }

                            // group is not anchored so add it to list of slidable groups
                            groups.Add(group);
                        }
                        if ( group.type == resolvedType ) {
                            group.count++;
                            switch(resolvedType) {
                                default: case SpringState.Unknown:
                                    throw new Exception("Resolved state has unknown");
                                case SpringState.Broken:
                                    break;
                                case SpringState.Operational:
                                    group.min++;
                                    group.max++;
                                    break;
                                case SpringState.OpSlide:
                                    group.max++;
                                    break;
                            }
                        }
                    }

                    // Look for groups OpSlide groups that are surrounded by broken
                    var blockPrevType = groupStart > 0 ? row.initialStates[i - 1] : SpringState.Unknown;
                    var blockNextType = i < row.initialStates.Length ? row.initialStates[i] : SpringState.Unknown;
                    for(int j = 0; j < groups.Count; j++) {
                        var thisGroup = groups[j];
                        if( thisGroup.type == SpringState.OpSlide ) {
                            // determine if this has a min of 1
                            var prevType = j > 0 ? groups[j - 1].type : blockPrevType;
                            var nextType = j + 1 < groups.Count ? groups[j + 1].type : blockNextType;

                            if( prevType == SpringState.Broken && nextType == SpringState.Broken ) {
                                thisGroup.min = Math.Max(1, thisGroup.min);
                            }
                        }
                    }

                    // set the max for all expandables to the highest delta
                    int highDelta = groups.Max(pg => pg.Delta);
                    groups.ForEach(pg => {
                        if (pg.type != SpringState.Broken) pg.max = pg.min + highDelta;
                    });

                    unknownGroups.Add(groups);
                    i--; // offset back due to inner loop increment
                }
            }
        }

        int perms = 1;
        foreach(var groupSet in unknownGroups) {
            int setPerms = 1;
            foreach(var group in groupSet) {
                if (group.type == SpringState.Broken) continue;
                setPerms += group.Delta;
            }
            perms *= setPerms;
        }

        WriteLine($"{row.raw}\t\t{string.Join(',', row.brokenGroups)}\t\t{perms}");
        return Math.Max(1, perms);
    }


    protected override object SolvePartOneRaw()
    {
        int perm = 0;


        foreach(var row in rows) {
            int rowperms = 0, ttlBroken = row.brokenGroups.Sum();
            Queue<(int i, List<SpringState> state, int broken, int unknown)> forks = new();
            forks.Enqueue((0, new(row.initialStates), row.initialStates.Count(st => st == SpringState.Broken), row.initialStates.Count(st => st == SpringState.Unknown)));

            while(forks.Count > 0) {
                var (i, state, broken, unknown) = forks.Dequeue();

                // skip set state
                for (; i < state.Count && state[i] != SpringState.Unknown; i++);
                if (i >= state.Count) {
                    var groups = GetGroups(state);
                    Print(groups, row.brokenGroups);
                    var permBrokens = GetBrokens(groups);
                    if (row.brokenGroups.SequenceEqual(permBrokens))
                        rowperms++;
                }
                else {// assume we're at a broken, since we didn't walk off the end
                    List<SpringState> stateCopy = new(state);
                    // Queue as operational
                    if( broken + unknown - 1 >= ttlBroken ) { // if selecting operational doesn't allow completion, don't walk that path
                        state[i] = SpringState.Operational;
                        if( OutputAlways) Print(GetGroups(state), row.brokenGroups, 4);
                        forks.Enqueue((i + 1, state, broken, unknown - 1));
                    }
                    // Queue as broken
                    if( broken + 1 <= ttlBroken ) { // if selecting broken here causes too many brokens don't walk that path
                        stateCopy[i] = SpringState.Broken;
                        if (OutputAlways) Print(GetGroups(stateCopy), row.brokenGroups, 4);
                        forks.Enqueue((i + 1, stateCopy, broken + 1, unknown - 1));
                    }
                }
            }
            
            WriteLine($"rowperms: {rowperms}");
            perm += rowperms;
        }


        return perm;//rows.Sum(RowPermutations);
    }

    protected override object SolvePartTwoRaw()
    {
        return null;
    }
}
