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
                var copy = Clone();
                copy.CopyUp(Parent);
                copy.CopyDown(Next);
                return copy;
            }
            private Route Clone() {
                return new Route(Ends[0], Ends[1], Dist);
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
                return Parent.Ends[0] == end || Parent.Ends[1] == end || Parent.HasParentWithEnd(end) ;
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
                if( Parent == null && Next == null) {
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

        Dictionary<string, List<Route>> possibleRoutes = new Dictionary<string, List<Route>>();
        List<Route> routes = new List<Route>(20000);

        public Day09() : base(09, 2015, "All in a Single Night", false)
        {
            

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

            foreach( var loc in possibleRoutes.Keys ) {
                //Trace.WriteLine($"Searching routes starting with {loc}:");
                foreach( var prestineOrigin in possibleRoutes[loc] ) {
                    var foundRoutes = MapRoute(prestineOrigin, loc);
                    //foundRoutes.ForEach(r => Trace.WriteLine(r.Print()));
                    routes.AddRange(foundRoutes);
                }
            }


        }

        /// <param name="origin">Route I'm on</param>
        /// <param name="originFrom">Which end I entered this route on</param>
        List<Route> MapRoute(Route origin, string originFrom) {
            string exit = origin.Ends[0] == originFrom ? origin.Ends[1] : origin.Ends[0];
            List<Route> otherRoutes = possibleRoutes.GetValueOrDefault(exit, null);
            if( otherRoutes == null ) {
                return new List<Route> { origin };
            }

            List<Route> results = new List<Route>();
            foreach(var depLoc in otherRoutes) {
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
            return routes.Select(r => r.LinearDistance()).Min().ToString();
        }

        protected override string SolvePartTwo()
        {
            return routes.Select(r => r.LinearDistance()).Max().ToString();
        }
    }
}
