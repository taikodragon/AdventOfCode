using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 13, "Knights of the Dinner Table")]
    class Day13 : ASolution
    {
        class Route
        {
            public Route(string end1, string end2) {
                Ends = new List<string> { end1, end2 };
                Keys = new HashSet<string> { string.Concat(end1, '|', end2), string.Concat(end2, '|', end1) };
            }
            public HashSet<string> Keys { get; }
            public List<string> Ends { get; }
            public int Dist { get; set; }
            public Route Parent { get; set; }
            public Route Next { get; set; }
            public Route Copy() {
                var copy = Clone();
                copy.CopyUp(Parent);
                copy.CopyDown(Next);
                return copy;
            }
            private Route Clone() {
                return new Route(Ends[0], Ends[1]) { Dist = Dist };
            }
            private void CopyUp(Route parent) {
                if( parent == null ) return;
                Parent = parent.Clone();
                Parent.Next = this;
                Parent.CopyUp(parent.Parent);
            }
            private void CopyDown(Route next) {
                if( next == null ) return;
                Next = next.Clone();
                Next.Parent = this;
                Next.CopyDown(next.Next);
            }
            public bool HasParentOf(Route maybeParent) {
                return Parent?.Keys?.SetEquals(maybeParent.Keys) == true || (Parent?.HasParentOf(maybeParent) ?? false);
            }
            public bool HasParentWithEnd(string end) {
                if( Parent == null ) return false;
                return Parent.Ends[0] == end || Parent.Ends[1] == end || Parent.HasParentWithEnd(end);
            }
            public Route RootNode() {
                Route walker = this;
                while( walker.Parent != null ) walker = walker.Parent;
                return walker;
            }
            public bool Equals(Route rhs) {
                return Keys.SetEquals(rhs?.Keys);
            }
            public int LinearDistance() {
                return Dist + (Next?.LinearDistance() ?? 0);
            }

            public override string ToString() {
                return Keys.First();
            }
            public string Print() {
                const string pointer = " -> ";
                if( Parent == null && Next == null ) {
                    return string.Concat(Ends[0], pointer, Ends[1]);
                }
                string result = string.Empty;
                if( Parent == null ) {
                    result = string.Concat(
                        "{", LinearDistance(), "} ",
                        Next.Ends[0] == Ends[0] || Next.Ends[1] == Ends[0] ? Ends[1] : Ends[0],
                        pointer, Next.Ends[0] == Ends[0] || Next.Ends[1] == Ends[0] ? Ends[0] : Ends[1]);
                }
                else
                    result = string.Concat(pointer, Parent.Ends[0] == Ends[0] || Parent.Ends[1] == Ends[0] ? Ends[1] : Ends[0]);
                result += Next?.Print();
                return result;
            }
        }

        Dictionary<string, List<Route>> possibleRoutes = new Dictionary<string, List<Route>>(20);


        public Day13() : base(false)
        {
            

            string input = Input
                .Replace(" would", string.Empty)
                .Replace(" happiness units by sitting next to", string.Empty)
                .Replace(".", string.Empty);
            List<Route> hapRoutes = new List<Route>();
            foreach( string line in input.SplitByNewline(false, true) ) {
                string[] parts = line.Split(' ').Select(p => p.Trim()).ToArray();
                string key = $"{parts[0]}|{parts[3]}";
                Route hapRoute = hapRoutes.FirstOrDefault(r => r.Keys.Contains(key));
                if( hapRoute == null) {
                    hapRoutes.Add(hapRoute = new Route(parts[0], parts[3]));

                    List<Route> prs;
                    if( !possibleRoutes.TryGetValue(parts[0], out prs) ) {
                        possibleRoutes.Add(parts[0], new List<Route>() { hapRoute });
                    }
                    else {
                        prs.Add(hapRoute);
                    }
                    if( !possibleRoutes.TryGetValue(parts[3], out prs) ) {
                        possibleRoutes.Add(parts[3], new List<Route>() { hapRoute });
                    }
                    else {
                        prs.Add(hapRoute);
                    }
                }
                hapRoute.Dist += int.Parse(parts[2]) * (parts[1] == "gain" ? 1 : -1);
            }
        }

        List<Route> MapRoute(Route origin, string originFrom) {
            string exit = origin.Ends[0] == originFrom ? origin.Ends[1] : origin.Ends[0];
            List<Route> otherRoutes = possibleRoutes.GetValueOrDefault(exit, null);
            if( otherRoutes == null ) {
                return new List<Route> { origin };
            }

            List<Route> results = new List<Route>();
            foreach( var depLoc in otherRoutes ) {
                // don't recurse myself or my parents
                if( origin.Equals(depLoc) || origin.HasParentOf(depLoc) ) continue;
                string nextExit = depLoc.Ends[0] == exit ? depLoc.Ends[1] : depLoc.Ends[0];
                if( origin.HasParentWithEnd(nextExit) ) continue;

                var variant = origin.Copy();
                var next = variant.Next = depLoc.Copy();
                next.Parent = variant;

                results.AddRange(MapRoute(variant.Next, exit).Select(r => r.RootNode()));
            }
            if( results.Count == 0 ) {
                results.Add(origin);
            }

            return results;
        }


        protected override string SolvePartOne()
        {
            List<Route> routes = new List<Route>(20000);
            foreach( var loc in possibleRoutes.Keys ) {
                if( UseDebugInput) Trace.WriteLine($"Searching routes starting with {loc}:");
                foreach( var prestineOrigin in possibleRoutes[loc] ) {
                    var foundRoutes = MapRoute(prestineOrigin, loc);
                    // add in loop link
                    foreach( var route in foundRoutes ) {
                        string rootName = route.Ends[0] == route.Next.Ends[0] || route.Ends[0] == route.Next.Ends[1] ? route.Ends[1] : route.Ends[0];
                        var next = route;
                        while( next.Next != null )
                            next = next.Next;

                        string lastName = 
                            next.Ends[0] == next.Parent.Ends[0] || next.Ends[0] == next.Parent.Ends[1]? next.Ends[1] : next.Ends[0];

                        var last = possibleRoutes[rootName].First(r => r.Ends.Contains(lastName));
                        next.Next = last.Copy();
                        next.Next.Parent = next;
                    }
                    // print route
                    if( UseDebugInput ) foundRoutes.ForEach(r => Trace.WriteLine(r.Print()));
                    routes.AddRange(foundRoutes);
                }
            }

            return routes.Max(r => r.LinearDistance()).ToString();
        }

        protected override string SolvePartTwo()
        {
            List<Route> routes = new List<Route>(20000);

            possibleRoutes["Self"] = new List<Route>();
            foreach( string key in possibleRoutes.Keys ) {
                if( key == "Self" ) continue;
                var routeToMe = new Route(key, "Self");
                possibleRoutes[key].Add(routeToMe);
                possibleRoutes["Self"].Add(routeToMe);
            }

            foreach( var loc in possibleRoutes.Keys ) {
                if( UseDebugInput ) Trace.WriteLine($"Searching routes starting with {loc}:");
                foreach( var prestineOrigin in possibleRoutes[loc] ) {
                    var foundRoutes = MapRoute(prestineOrigin, loc);
                    // add in loop link
                    foreach( var route in foundRoutes ) {
                        string rootName = route.Ends[0] == route.Next.Ends[0] || route.Ends[0] == route.Next.Ends[1] ? route.Ends[1] : route.Ends[0];
                        var next = route;
                        while( next.Next != null )
                            next = next.Next;

                        string lastName =
                            next.Ends[0] == next.Parent.Ends[0] || next.Ends[0] == next.Parent.Ends[1] ? next.Ends[1] : next.Ends[0];

                        var last = possibleRoutes[rootName].First(r => r.Ends.Contains(lastName));
                        next.Next = last.Copy();
                        next.Next.Parent = next;
                    }
                    // print route
                    if( UseDebugInput ) foundRoutes.ForEach(r => Trace.WriteLine(r.Print()));
                    routes.AddRange(foundRoutes);
                }
            }

            return routes.Max(r => r.LinearDistance()).ToString();
        }
    }
}
