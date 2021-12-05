using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode.Solutions
{
    class IntCoord : IComparable<IntCoord>, IEquatable<IntCoord>
    {
        public IntCoord(int x, int y) {
            X = x; Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public int North {
            get => Y;
            set => Y = value;
        }

        public int East {
            get => X;
            set => X = value;
        }

        public override string ToString() {
            return string.Concat("X: ", X, " Y: ", Y);
        }

        public int CompareTo([AllowNull] IntCoord other) {
            if( other == null ) return -1;
            if( Y == other.Y ) return X.CompareTo(other.X);
            return Y.CompareTo(other.Y);
        }

        public bool Equals([AllowNull] IntCoord other) {
            return X == other?.X && Y == other?.Y;
        }

        public override bool Equals(object obj) {
            return obj is IntCoord coord ? Equals(coord) : base.Equals(obj);
        }
        public override int GetHashCode() {
            return (X, Y).GetHashCode();
        }

        public static bool operator==(IntCoord lhs, IntCoord rhs) {
            return lhs?.X == rhs?.X && lhs?.Y == rhs?.Y;
        }
        public static bool operator!=(IntCoord lhs, IntCoord rhs) {
            return lhs?.X != rhs?.X || lhs?.Y != rhs?.Y;
        }

        public static IntCoord operator-(IntCoord me) {
            if( me == null ) throw new ArgumentNullException(nameof(me));
            return new IntCoord(-me.X, -me.Y);
        }
        public static IntCoord operator+(IntCoord lhs, IntCoord rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            if( rhs == null ) throw new ArgumentNullException(nameof(rhs));
            return new IntCoord(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
        public static IntCoord operator-(IntCoord lhs, IntCoord rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            if( rhs == null ) throw new ArgumentNullException(nameof(rhs));
            return new IntCoord(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
        public static IntCoord operator*(IntCoord lhs, IntCoord rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            if( rhs == null ) throw new ArgumentNullException(nameof(rhs));
            return new IntCoord(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        public static IntCoord operator+(IntCoord lhs, int rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            return new IntCoord(lhs.X + rhs, lhs.Y + rhs);
        }
        public static IntCoord operator-(IntCoord lhs, int rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            return new IntCoord(lhs.X - rhs, lhs.Y - rhs);
        }

        public static IntCoord operator*(IntCoord lhs, int rhs) {
            if( lhs == null ) throw new ArgumentNullException(nameof(lhs));
            return new IntCoord(lhs.X * rhs, lhs.Y * rhs);
        }
    }
}
