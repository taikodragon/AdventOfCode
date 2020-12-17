using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    class Day09 : ASolution
    {
        class Route
        {
            public Route(string from, string to, int dist) {
                From = from;
                To = to;
                Dist = dist;
                Key = string.Concat(From, '|', To);
            }
            public string Key { get; }
            public string From { get; }
            public string To { get; }
            public int Dist { get; }
            public Route Parent { get; set; }
            public Route Next { get; set; }
            public Route Copy() {
                return new Route(From, To, Dist) {
                    Parent = Parent?.Copy(),
                    Next = Next?.Copy()
                };
            }
            public bool HasParentOf(Route maybeParent) {
                return Parent?.Key == maybeParent.Key || (Parent?.HasParentOf(maybeParent) ?? false);
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
                List<Route> routes;
                if( !possibleRoutes.TryGetValue(parts[0], out routes) ) {
                    possibleRoutes.Add(parts[0], routes = new List<Route>());
                }
                routes.Add(new Route(parts[0], parts[1], int.Parse(parts[2])));
            }
        }

        List<Route> MapRoute(Route origin) {
            List<Route> otherRoutes = possibleRoutes.GetValueOrDefault(origin.To, null);
            if( otherRoutes == null ) {
                return new List<Route> { origin };
            }

            List<Route> results = new List<Route>();
            foreach(var depLoc in otherRoutes) {
                if( origin.HasParentOf(depLoc) ) continue;
                
                var variant = origin.Copy();
                variant.Next = depLoc.Copy();
                variant.Next.Parent = variant;

                results.AddRange(MapRoute(variant.Next).Select(r => r.RootNode()));
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
                    routes.AddRange(MapRoute(prestineOrigin));
                }
            }
            return routes.Select(r => r.LinearDistance()).Min().ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
