﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode.Solutions
{
    class IntCoord : IComparable<IntCoord>, IEquatable<IntCoord>
    {
        public static readonly IntCoord Up = new (0, -1),
            Down = new (0, 1),
            Left = new (-1, 0),
            Right = new (1, 0);


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
            return HashCode.Combine(X, Y);
        }

        public static bool operator==(IntCoord lhs, IntCoord rhs) {
            return lhs?.X == rhs?.X && lhs?.Y == rhs?.Y;
        }
        public static bool operator!=(IntCoord lhs, IntCoord rhs) {
            return lhs?.X != rhs?.X || lhs?.Y != rhs?.Y;
        }

        public static implicit operator (int x, int y)(IntCoord pt) => (pt.X, pt.Y);
        public static implicit operator IntCoord((int x, int y) pt) => new(pt.x, pt.y);
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

        public double Dist(IntCoord other) {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }
    }
}
