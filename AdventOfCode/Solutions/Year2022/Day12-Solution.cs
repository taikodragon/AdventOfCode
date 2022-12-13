using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.ApplicationModel.Payments;
using Windows.UI.Composition;
namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 12, "")]
class Day12 : ASolution, IComparer<Day12.Pathway>
{

    public Day12() : base(false) {
        OutputAlways = true;
        queue = new(5000, this);
    }

    const int RepFloor = 75, EndHeight = 'z' - 'a';
    readonly IntCoord[] dirs = new IntCoord[] { IntCoord.Right, IntCoord.Up, IntCoord.Down, IntCoord.Left };

    List<string> grid;
    IntCoord start, end, max;
    List<Region> regions = new();
    Dictionary<IntCoord, Region> pointToRegion = new();

    int iters = 0;
    PriorityQueue<Pathway, Pathway> queue;
    Dictionary<IntCoord, int> gridRep = new(5000);

    protected override void ParseInput() {
        grid = Input.SplitByNewline();
        int y = 0, x = 0;
        foreach(var line in grid) {
            x = 0;
            foreach(var c in line) {
                if (c == 'S') {
                    start = (x, y);

                }
                else if (c == 'E') {
                    end = (x, y);
                }
                x++;
            }
            y++;
        }
        max = (x, y);
        grid[start.Y] = grid[start.Y].Replace('S', 'a');
        grid[end.Y] = grid[end.Y].Replace('E', 'z');


        for(y = 0; y < max.Y; y++) {
            for(x = 0; x < max.X; x++) {
                IntCoord at = (x, y);
                int myHeight = CoordHeight(grid, at);
                var neighbors = dirs
                    .Select(d => at + d)
                    .Where(pt => ValidCoord(pt))
                    .Where(pt => Reachable(grid, at, pt))
                    .Select(pt => (pt, height: CoordHeight(grid, pt)))
                    .ToArray();
                Region myRegion = null;
                // Select all neighbors that could be in the same region
                var nearbyRegions = neighbors
                    .Where(n => n.height == myHeight)
                    .Where(n => pointToRegion.ContainsKey(n.pt))
                    .Select(n => pointToRegion[n.pt])
                    .ToList();

                if( nearbyRegions.Count == 0 ) {
                    myRegion = new Region(myHeight, at);
                    regions.Add(myRegion);
                    pointToRegion[at] = myRegion;
                }
                else if(nearbyRegions.Count == 1 ) {
                    myRegion = nearbyRegions[0];
                    myRegion.area.Add(at); 
                    pointToRegion[at] = myRegion;
                } else { // More than one, so merge
                    myRegion = nearbyRegions[0];
                    myRegion.area.Add(at);
                    pointToRegion[at] = myRegion;

                    foreach(var region in nearbyRegions.Skip(1)) {
                        if (region == myRegion) continue;
                        myRegion.Merge(region);
                        foreach(var pt in region.area) { pointToRegion[pt] = myRegion; }
                        regions.Remove(region);
                    }
                }

                foreach(var neighbor in neighbors.Where(n => n.height != myHeight)) {
                    if (neighbor.height > myHeight)
                        myRegion.exitUp.Add(neighbor.pt);
                    else
                        myRegion.exitDown.Add(neighbor.pt);
                }
            }
        }

        // clear points from the region table
        while(regions.Any(r => !r.Routable)) {
            HashSet<IntCoord> dead = new(pointToRegion.Count);
            foreach (var region in regions.Where(r => !r.Routable)) {
                foreach (var pt in region.area) {
                    dead.Add(pt);
                }
            }
            regions.RemoveAll(r => !r.Routable);
            foreach( var deadAt in dead) {
                var myRegion = pointToRegion[deadAt];
                var neighbors = dirs
                    .Select(d => deadAt + d)
                    .Where(pt => ValidCoord(pt))
                    .Where(pt => pointToRegion.ContainsKey(pt))
                    .Select(pt => pointToRegion[pt])
                    .Where(r => r != myRegion)
                    .Distinct()
                    .ToList();
                foreach(var neighborRegion in neighbors) {
                    neighborRegion.exitUp.Remove(deadAt);
                    neighborRegion.exitDown.Remove(deadAt);
                }
            }
            foreach (var deadAt in dead) {
                pointToRegion.Remove(deadAt);
            }
        }

        if ( false ) {
            StringBuilder sb = new(pointToRegion.Count + (2 * grid.Count));
            for (y = 0; y < max.Y; y++) {
                for (x = 0; x < max.X; x++) {
                    IntCoord at = (x, y);
                    int idx = regions.IndexOf(pointToRegion.GetValueOrDefault(at));
                    if (idx == -1) sb.Append(" . ");
                    else { sb.Append($"{idx,2} "); }
                }
                sb.AppendLine();
            }
            WriteLine(sb);

            sb.Clear();
        }
    }


    void EnqueuePathway(Pathway item) {
        queue.Enqueue(item, item);
    }
    Pathway DequeuePathway() {
        var result = queue.Dequeue();
        return result;
    }
    bool ValidCoord(IntCoord coord) {
        return coord.X >= 0 && coord.Y >= 0 && coord.X < max.X && coord.Y < max.Y;
    }
    static int CoordHeight(List<string> grid, IntCoord coord) {
        return grid[coord.Y][coord.X] - 'a';
    }
    static bool Reachable(List<string> grid, IntCoord self, IntCoord other) {
        if (Utilities.ManhattanDistance(self, other) != 1) return false;
        return CoordHeight(grid, other) - 1 <= CoordHeight(grid, self);
    }
    protected override object SolvePartOneRaw() {
        PriorityQueue<Pathway,int> paths = new();

        {
            var initialPath = new Pathway(grid, gridRep, new(), start, start);
            paths.Enqueue(initialPath, initialPath.Cost);
        }

        List<Pathway> finals = new();
        int endHeight = CoordHeight(grid, end);
        while(paths.Count > 0) {
            var currentPath = paths.Dequeue();
            WriteLine($"Finding paths for {currentPath.Head} / {currentPath.Target} @ {currentPath.Height + 1}");
            var results = FindPathWay(currentPath, pointToRegion[currentPath.Target]);
            WriteLine($"FindPathWay complete with {results.Count} results.");
            foreach(var result in results ) {
                if( result.Head == end ) {
                    finals.Add(result);
                    continue;
                }
                paths.Enqueue(result, result.Cost);

                WriteLine($"Got result reaching {result.Target}, costing {result.Cost}");
            }
            WriteLine($"Current State: finals: {finals.Count} pending: {paths.Count}");
            //Thread.Sleep(3_000);
        }
        
        WriteLine($"Complete in {iters} iterations with {finals.Count}");

        var finalPath = finals.OrderBy(r => r.Cost).FirstOrDefault();
        if( finalPath is not null ) {
            Print(finalPath, false, string.Empty);
            return finalPath.Cost;
        }
        return null;
    }
    protected override object SolvePartTwoRaw() {
        return null;
    }

    void Print(Pathway path, bool withOrdering, string stats) {
        List<StringBuilder> printGrid = new(grid.Select(l => new StringBuilder(l)));
        char at = 'A';
        foreach(var (coord, rep) in gridRep) {
            if( rep >= RepFloor) printGrid[coord.Y][coord.X] = '#';
        }

        foreach (var coord in path.Path) {
            if( withOrdering ) {
                printGrid[coord.Y][coord.X] = at;
                at++;
                if (at == ('Z' + 1)) at = 'A';
            } else {
                if( gridRep.GetValueOrDefault(coord) >= RepFloor ) {
                    printGrid[coord.Y][coord.X] = '+';
                } else {
                    printGrid[coord.Y][coord.X] = (char)(printGrid[coord.Y][coord.X] + ('A' - 'a'));
                }
            }
        }
        printGrid[path.Target.Y][path.Target.X] = '$';
        printGrid[0].Insert(0, $"Head: {path.Head} \tTarget: {path.Target} \tCost: {path.Cost} -- {stats}{Environment.NewLine}");
        printGrid[^1].AppendLine();
        WriteLine(printGrid);
    }

    int IComparer<Pathway>.Compare(Pathway lhs, Pathway rhs) {
        //return (lhs.Cost + lhs.Distance).CompareTo(rhs.Cost + rhs.Distance);
        int comp;
        comp = lhs.Cost.CompareTo(rhs.Cost);
        if (comp == 0) return lhs.Distance.CompareTo(rhs.Distance);
        return comp;
    }


    List<Pathway> FindPathWay(Pathway entry, Region within) {
        WriteLine("=== Region Search ===");
        WriteLine($"Starting from {entry.Target} rooted at {entry.Head} with region {regions.IndexOf(within)}");

        HashSet<IntCoord> targets;
        if (within.exitUp.Count > 0) targets = within.exitUp;
        else targets = within.exitDown;


        List<Pathway> completes = new();
        // queue up targets
        foreach (var targ in targets) {
            Pathway path = entry.Fork();
            if (path.Height == EndHeight)
                path.Target = end;
            else
                path.Target = targ;

            // Prepare state
            Dictionary<IntCoord, Pathway> cheapestTo = new();
            queue.Clear();
            gridRep.Clear();
            EnqueuePathway(path);
            int gridRepMin = 0;
            while(queue.Count > 0) {
                iters++;
                Pathway current = DequeuePathway();
                if (current.Complete) {
                    completes.Add(current);
                    //WriteLine($"Complete Path! === !!! ==={Environment.NewLine}\tHead: {current.Head} \tTarget: {current.Target} \tCost: {current.Cost} -- Exits: {targets.Count} \tComplete Paths: {completes.Count} \tQueue Count: {queue.Count} \tIter: {iters} \tGrid Rep: {gridRep.Count} Min: {gridRepMin}");
                    //Print(current, false, "");
                    continue;
                }
                //Print(current, false, $"Exits: {targets.Count} \tComplete Paths: {completes.Count} \tQueue Count: {queue.Count} \tIter: {iters} \tGrid Rep: {gridRep.Count} Min: {gridRepMin}");

                // we visited this square, avoid over visitation by increasing the reputation
                //gridRep[current.Head] = gridRep.GetValueOrDefault(current.Head) + 1;
                //if (gridRep[current.Head] > gridRepMin) gridRepMin = gridRep[current.Head];

                foreach (var delta in dirs) {
                    var coord = current.Head + delta;
                    // check for within bounds
                    if (coord == current.Target || within.area.Contains(coord)) {
                        var cachedPath = cheapestTo.GetValueOrDefault(coord);
                        if( cachedPath == null) {
                            var alt = current.Fork();
                            if (alt.TryVisit(coord)) {
                                cheapestTo[coord] = alt;
                                EnqueuePathway(alt);
                            }
                        }
                        else if(current.Cost + 1 < cachedPath.Cost) { // replace
                            cheapestTo[coord] = current;
                        }
                        // else this was a worse path
                    }
                }
            }
        }

        return completes;
    }

    internal class Pathway
    {
        readonly List<string> grid;
        readonly Dictionary<IntCoord, int> gridRep;
        readonly HashSet<IntCoord> visited;
        List<IntCoord> path = new();
        IntCoord head;

        public Pathway(List<string> grid, Dictionary<IntCoord, int> gridRep, HashSet<IntCoord> visited, IntCoord head, IntCoord target) {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.gridRep = gridRep ?? throw new ArgumentNullException(nameof(gridRep));
            this.visited = visited ?? throw new ArgumentNullException(nameof(visited));

            this.head = head;
            path.Add(head);
            visited.Add(head);
            this.Target = target;
        }

        public IntCoord Head => head;
        public IntCoord Target { get; set; }
        public IReadOnlyList<IntCoord> Path => path;
        public int Cost => path.Count - 1;
        public int Distance => Utilities.ManhattanDistance(head, Target);
        public bool Complete => head == Target;
        public int Reputation => gridRep.GetValueOrDefault(Head);
        public int Height => grid[head.Y][head.X] - 'a';

        public bool TryVisit(IntCoord coord) {
            if (Utilities.ManhattanDistance(head, coord) != 1) throw new ArgumentException("Not adjacent to head of path");
            if (visited.Contains(coord)) return false;
            if (!Reachable(grid, head, coord)) return false;
            //if (gridRep.GetValueOrDefault(coord) <= RepFloor) return false; // over visited

            visited.Add(coord);
            path.Add(coord);
            head = coord;
            return true;
        }

        public Pathway Fork() {
            return new Pathway(grid, gridRep, new(visited), head, Target) {
                path = new(path)
            };
        }
    }

    class Region {
        public readonly int height;
        public readonly HashSet<IntCoord> area = new();
        public readonly HashSet<IntCoord> exitUp = new();
        public readonly HashSet<IntCoord> exitDown = new();
        public bool Routable => exitUp.Count > 0 || exitDown.Count > 0;

        public Region(int height, IntCoord startPt) {
            this.height = height;
            area.Add(startPt);
        }

        public void Merge(Region other) {
            if (other is null) throw new ArgumentNullException(nameof(other));
            if (other.height != this.height) throw new ArgumentException("Regions must be same height to merge.", nameof(other));
            if (this == other) return;
            foreach (var pt in other.area) { area.Add(pt); }
            foreach (var pt in other.exitUp) { exitUp.Add(pt); }
            foreach (var pt in other.exitDown) { exitDown.Add(pt); }
        }
    }
}
