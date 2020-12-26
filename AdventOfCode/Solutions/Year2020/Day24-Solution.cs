using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day24 : ASolution
    {
        const bool isTracing = false;

        HexDirection[] directions = new HexDirection[] { HexDirection.W, HexDirection.NW, HexDirection.NE, HexDirection.E, HexDirection.SE, HexDirection.SW };

        Dictionary<(int ring, int pos), HexNode<bool>> part1Grid;
        public Day24() : base(24, 2020, "Lobby Layout") {
            UseDebugInput = false;
        }

        protected override string SolvePartOne() {
            Dictionary<(int, int), HexNode<bool>> grid = new Dictionary<(int, int), HexNode<bool>>();
            part1Grid = grid;

            HexNode<bool> origin = new HexNode<bool>(0, 0, grid);

            foreach(string line in Input.SplitByNewline(false, true)) {
                HexNode<bool> atNode = origin;
                for(int at = 0; at < line.Length; at++ ) {
                    HexDirection nextDirection = HexDirection.W;
                    switch(line[at]) {
                        case 'e': nextDirection = HexDirection.E; break;
                        case 'w': nextDirection = HexDirection.W; break;
                        case 'n':
                            at++;
                            if( line[at] == 'e' ) nextDirection = HexDirection.NE;
                            else nextDirection = HexDirection.NW;
                            break;
                        case 's':
                            at++;
                            if( line[at] == 'e' ) nextDirection = HexDirection.SE;
                            else nextDirection = HexDirection.SW;
                            break;
                    }
                    var nextNode = atNode.GetNodeInDirection(nextDirection);
                    if( isTracing ) Trace.WriteLine(string.Concat(atNode, " --", nextDirection.ToString().PadLeft(2), "-> ", nextNode));
                    atNode = nextNode;
                }
                if( isTracing ) Trace.WriteLine($"=== Flipping {atNode} ===");
                atNode.Payload = !atNode.Payload;
            }
            return grid.Values.Count(v => v.Payload).ToString();
        }

        void CopyGrid(Dictionary<(int,int), HexNode<bool>> left, Dictionary<(int, int), HexNode<bool>> right) {
            // reset payloads to initial condition
            foreach(var node in right.Values) { node.Payload = false; }
            // copy payloads from left to right
            foreach(var pair in left) {
                HexNode<bool> leftNode = pair.Value;
                HexNode<bool> rightNode;
                if( right.TryGetValue(pair.Key, out rightNode) )
                    rightNode.Payload = pair.Value.Payload;
                else {
                    right.Add(
                        (leftNode.Ring, leftNode.Position),
                        new HexNode<bool>(leftNode.Ring, leftNode.Position, right) { Payload = leftNode.Payload });
                }
            }
        }
        protected override string SolvePartTwo() {
            Dictionary<(int, int), HexNode<bool>> now = new Dictionary<(int, int), HexNode<bool>>(),
                next = new Dictionary<(int, int), HexNode<bool>>();
            CopyGrid(part1Grid, now); // populate initial state

            int day = 0;
            while( day < 100 ) {
                // Ensure grid is appropriately expanded to handle white-to-black tile case
                foreach(var node in now.Values.ToList()) {
                    if( node.Payload ) {
                        foreach(var dir in directions) { node.GetNodeInDirection(dir); }
                    }
                }
                CopyGrid(now, next); // copy state

                foreach( var pair in now) {
                    var node = pair.Value;
                    int adjacentBlackTiles = 0;
                    foreach(var dir in directions) {
                        if( node.GetNodeInDirection(dir, false)?.Payload == true ) adjacentBlackTiles++;
                    }
                    if( node.Payload && (adjacentBlackTiles == 0 || adjacentBlackTiles > 2) ) {
                        next[pair.Key].Payload = false; // flip to white
                    }
                    else if( !node.Payload && adjacentBlackTiles == 2 ) {
                        next[pair.Key].Payload = true;
                    }
                }

                day++;
                if( isTracing ) Trace.WriteLine($"Day {day}: {next.Values.Count(v => v.Payload)}");
                // flip
                var tmp = now;
                now = next;
                next = tmp;
            }

            return now.Values.Count(v => v.Payload).ToString();
        }

    }

    public enum HexDirection
    {
        W = 0, NW = 1, NE = 2, E = 3, SE = 4, SW = 5
    }
    public class HexNode<T>
    {
        int ring, ringPosition, ringSize;


        Dictionary<(int ring, int pos), HexNode<T>> grid;
        Dictionary<HexDirection, HexNode<T>> neighbors = new Dictionary<HexDirection, HexNode<T>>(6);

        public HexNode(int ring, int ringPosition, Dictionary<(int ring, int pos), HexNode<T>> grid) {
            this.ring = ring;
            ringSize = ring * 6;
            this.ringPosition = ring == 0 ? 0 : ringPosition;
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public int Ring => ring;
        public int Position => ringPosition;
        public int RingSize => ringSize;
        public T Payload { get; set; }
        public HexNode<T> GetNodeInDirection(HexDirection direction, bool allowExpansion = true) {
            HexNode<T> neighbor = null;
            if( neighbors.TryGetValue(direction, out neighbor) ) {
                return neighbor;
            }
            // else look for this one in the global grid
            var coord = GetRingPositionForDirection(direction);
            if( grid.TryGetValue(coord, out neighbor) ) {
                neighbors[direction] = neighbor;
                return neighbor;
            }
            else if( !allowExpansion ) return null; // caller opt-ed out of expansion and should handle nulls

            // else construct it
            neighbor = new HexNode<T>(coord.ring, coord.pos, grid);
            grid.Add(coord, neighbor);
            neighbors[direction] = neighbor;

            return neighbor;
        }

        (int ring, int pos) GetRingPositionForDirection(HexDirection direction) {
            if( ring == 0 ) return (1, (int)direction);
            // Divide the ring into cardinal rays (pos/size), those are the turning points, between the turning points the pattern is the same.
            // Look for each of the 6 quardrants and map the translation based on the direction of travel

            HexDirection rayLine = (HexDirection)(ringPosition / ring); // 3 / 2 = 1
            int upRing = ring + 1, downRing = ring - 1;
            bool isOnRayLine = ringPosition == ring * (int)rayLine;

            switch( rayLine ) {
                case HexDirection.W:
                    switch( direction ) {
                        case HexDirection.W:
                            return (upRing, ringPosition);
                        case HexDirection.NW:
                            return (upRing, ringPosition + 1);
                        case HexDirection.NE:
                            return (ring, ringPosition + 1);
                        case HexDirection.E:
                            return (downRing, ringPosition);
                        case HexDirection.SE:
                            if( isOnRayLine ) return (ring, ringSize - 1);
                            else return (downRing, ringPosition - 1);
                        case HexDirection.SW:
                            if( isOnRayLine ) return (upRing, upRing * 6 - 1);
                            else return (ring, ringPosition - 1);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
                case HexDirection.NW:
                    switch( direction ) {
                        case HexDirection.W:
                            if( isOnRayLine ) return (upRing, ringPosition);
                            else return (ring, ringPosition - 1);
                        case HexDirection.NW:
                            return (upRing, ringPosition + 1);
                        case HexDirection.NE:
                            return (upRing, ringPosition + 2);
                        case HexDirection.E:
                            return (ring, ringPosition + 1);
                        case HexDirection.SE:
                            return (downRing, ringPosition - 1);
                        case HexDirection.SW:
                            if( isOnRayLine ) return (ring, ringPosition - 1);
                            else return (downRing, ringPosition - 2);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
                case HexDirection.NE:
                    switch( direction ) {
                        case HexDirection.W:
                            if( isOnRayLine ) return (ring, ringPosition - 1);
                            else return (downRing, ringPosition - 3);
                        case HexDirection.NW:
                            if( isOnRayLine ) return (upRing, ringPosition + 1);
                            else return (ring, ringPosition - 1);
                        case HexDirection.NE:
                            return (upRing, ringPosition + 2);
                        case HexDirection.E:
                            return (upRing, ringPosition + 3);
                        case HexDirection.SE:
                            return (ring, ringPosition + 1);
                        case HexDirection.SW:
                            return (downRing, ringPosition - 2);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
                case HexDirection.E:
                    switch( direction ) {
                        case HexDirection.W:
                            return (downRing, ringPosition - 3);
                        case HexDirection.NW:
                            if( isOnRayLine ) return (ring, ringPosition - 1);
                            else return (downRing, ringPosition - 4);
                        case HexDirection.NE:
                            if( isOnRayLine ) return (upRing, ringPosition + 2);
                            else return (ring, ringPosition - 1);
                        case HexDirection.E:
                            return (upRing, ringPosition + 3);
                        case HexDirection.SE:
                            return (upRing, ringPosition + 4);
                        case HexDirection.SW:
                            return (ring, ringPosition + 1);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
                case HexDirection.SE:
                    switch( direction ) {
                        case HexDirection.W:
                            return (ring, ringPosition + 1);
                        case HexDirection.NW:
                            return (downRing, ringPosition - 4);
                        case HexDirection.NE:
                            if( isOnRayLine ) return (ring, ringPosition - 1);
                            else return (downRing, ringPosition - 5);
                        case HexDirection.E:
                            if( isOnRayLine ) return (upRing, ringPosition + 3);
                            else return (ring, ringPosition - 1);
                        case HexDirection.SE:
                            return (upRing, ringPosition + 4);
                        case HexDirection.SW:
                            return (upRing, ringPosition + 5);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
                case HexDirection.SW:
                    switch( direction ) {
                        case HexDirection.W:
                            return (upRing, ringPosition + 6);
                        case HexDirection.NW:
                            return (ring, (ringPosition + 1) % ringSize);
                        case HexDirection.NE:
                            if( ringPosition == ringSize - 1 ) return (downRing, 0);
                            else return (downRing, ringPosition - 5);
                        case HexDirection.E:
                            if( isOnRayLine ) return (ring, ringPosition - 1);
                            else return (downRing, ringPosition - 6);
                        case HexDirection.SE:
                            if( isOnRayLine ) return (upRing, ringPosition + 4);
                            else return (ring, ringPosition - 1);
                        case HexDirection.SW:
                            return (upRing, ringPosition + 5);
                    }
                    throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
            }
            throw new Exception($"Unmapped neighbor {rayLine}:{direction}");
        }

        public override string ToString() {
            return string.Concat(ring,",",ringPosition," - ", Payload);
        }
    }
}
