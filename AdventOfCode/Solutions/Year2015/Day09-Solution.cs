using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day09 : ASolution
    {
        class Route
        {
            public Route(string end1, string end2, int dist) {
                Ends = new List<string> { end1, end2 };
                Dist = dist;
                Keys = new HashSet<string> { string.Concat(end1, '|', end2), string.Concat(end2, '|', end1) };
            }
            public HashSet<string> Keys { get; }
            public List<string> Ends { get; }
            public int Dist { get; }
            public Route Parent { get; set; }
            public Route Next { get; set; }
            public Route Copy() {
                return new Route(Ends[0], Ends[1], Dist) {
                    Parent = Parent?.Copy(),
                    Next = Next?.Copy()
                };
            }
            public bool HasParentOf(Route maybeParent) {
                return Parent?.Keys?.SetEquals(maybeParent.Keys) == true || (Parent?.HasParentOf(maybeParent) ?? false);
            }
            public Route RootNode() {
                Route walker = this;
                while( walker.Parent != null ) walker = walker.Parent;
                return walker;
            }
            public int LinearDistance() {
                return Dist + (Next?.LinearDistance() ?? 0);
            }
        }

        Dictionary<string, List<Route>> possibleRoutes = new Dictionary<string, List<Route>>();

        public Day09() : base(09, 2015, "All in a Single Night")
        {
            UseDebugInput = true;

            foreach(string line in Input.SplitByNewline(false, true)) {
                string[] parts = line.Split(new string[] { "to", "=" }, StringSplitOptions.None).Select(p => p.Trim()).ToArray();
                Route route = new Route(parts[0], parts[1], int.Parse(parts[2]));
                List<Route> routes;
                if( !possibleRoutes.TryGetValue(parts[0], out routes) ) {
                    possibleRoutes.Add(parts[0], new List<Route>() { route });
                } else {
                    routes.Add(route);
                }
                if( !possibleRoutes.TryGetValue(parts[1], out routes) ) {
                    possibleRoutes.Add(parts[1], new List<Route>() { route });
                } else {
                    routes.Add(route);
                }
            }
        }

        List<Route> MapRoute(Route origin, string originFrom) {
            List<Route> otherRoutes = possibleRoutes.GetValueOrDefault(originFrom, null);
            if( otherRoutes == null ) {
                return new List<Route> { origin };
            }

            List<Route> results = new List<Route>();
            foreach(var depLoc in otherRoutes) {
                if( origin.HasParentOf(depLoc) ) continue;
                
                var variant = origin.Copy();
                var next = variant.Next = depLoc.Copy();
                next.Parent = variant;
                string nextFrom = next.Ends.First(e => e != originFrom);

                results.AddRange(MapRoute(variant.Next, nextFrom).Select(r => r.RootNode()));
            }
            if( results.Count == 0 ) {
                results.Add(origin);
            }

            return results;
        }

        protected override string SolvePartOne()
        {
            List<Route> routes = new List<Route>(20000);
            foreach(var loc in possibleRoutes.Keys) {
                foreach(var prestineOrigin in possibleRoutes[loc]) {
                    routes.AddRange(MapRoute(prestineOrigin, loc));
                }
            }

            foreach(Route root in routes) {
                List<string> nodes = new List<string>();
                string rootLhs, rootRhs;
                if( root.Next != null ) {
                    rootRhs = root.Next.Ends.First(e => root.Ends.Contains(e));
                    rootLhs = root.Ends.First(e => e != rootRhs);
                    nodes.Add(rootLhs);
                    nodes.Add(rootRhs);
                    Route at = root.Next;
                    while( at != null ) {
                        rootLhs = rootRhs;
                        rootRhs = at.Ends.First(e => e != rootLhs);
                        nodes.Add(rootRhs);
                        at = at.Next;
                    }
                } else {
                    nodes.AddRange(root.Ends);
                }
                Trace.WriteLine(string.Join(" -> ", nodes));
            }

            return routes.Select(r => r.LinearDistance()).Min().ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
