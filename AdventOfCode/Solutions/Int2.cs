using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode.Solutions
{
    struct Int2 : IComparable<Int2>, IEquatable<Int2>
    {
        public static readonly Int2 Up = new (0, -1),
            Down = new (0, 1),
            Left = new (-1, 0),
            Right = new (1, 0),
            Zero = new (0, 0),
            One = new(1,1),
            North = Up,
            NorthEast = Up + Right,
            East = Right,
            SouthEast = Down + Right,
            South = Down,
            SouthWest = Down + Left,
            West = Left,
            NorthWest = Up + Left;


        public Int2(int x, int y) {
            X = x; Y = y;
        }

        /// <summary>
        /// Also known as East.
        /// </summary>
        public int X;
        /// <summary>
        /// Also known as North.
        /// </summary>
        public int Y;

        public override string ToString() {
            return string.Concat("X: ", X, " Y: ", Y);
        }

        public int CompareTo([AllowNull] Int2 other) {
            if( Y == other.Y ) return X.CompareTo(other.X);
            return Y.CompareTo(other.Y);
        }

        public bool Equals(Int2 other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj) {
            return obj is Int2 coord ? Equals(coord) : base.Equals(obj);
        }
        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public static bool operator==(Int2 lhs, Int2 rhs) {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }
        public static bool operator!=(Int2 lhs, Int2 rhs) {
            return lhs.X != rhs.X || lhs.Y != rhs.Y;
        }

        public static implicit operator (int x, int y)(in Int2 pt) => (pt.X, pt.Y);
        public static implicit operator Int2((int x, int y) pt) => new(pt.x, pt.y);
        public static Int2 operator-(in Int2 me) {
            return new Int2(-me.X, -me.Y);
        }
        public static Int2 operator+(in Int2 lhs, in Int2 rhs) {
            return new Int2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
        public static Int2 operator-(in Int2 lhs, in Int2 rhs) {
            return new Int2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
        public static Int2 operator*(in Int2 lhs, in Int2 rhs) {
            return new Int2(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        public static Int2 operator+(in Int2 lhs, int rhs) {
            return new Int2(lhs.X + rhs, lhs.Y + rhs);
        }
        public static Int2 operator-(in Int2 lhs, int rhs) {
            return new Int2(lhs.X - rhs, lhs.Y - rhs);
        }

        public static Int2 operator*(in Int2 lhs, int rhs) {
            return new Int2(lhs.X * rhs, lhs.Y * rhs);
        }

        public double DistSquared(in Int2 other) {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }
        public static Int2 Offset(in Int2 position, in CompassDirection dir, int magnitude = 1) {
            return position + (dir switch {
                CompassDirection.N => North,
                CompassDirection.S => South,
                CompassDirection.W => West,
                CompassDirection.E => East,
                CompassDirection.NW => NorthWest,
                CompassDirection.NE => NorthEast,
                CompassDirection.SW => SouthWest,
                CompassDirection.SE => SouthEast,
                _ => throw new Exception("Unsupport direction")
            } * magnitude);
        }
        public static int ManhattanDistance(in Int2 a, in Int2 b) {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

    }
}
