/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{

    public static class Utilities
    {

        public static int[] ToIntArray(this string str, string delimiter = "")
        {
            if(delimiter == "")
            {
                var result = new List<int>();
                foreach(char c in str) if(int.TryParse(c.ToString(), out int n)) result.Add(n);
                return result.ToArray();
            }
            else
            {
                return str
                    .Split(delimiter)
                    .Where(n => int.TryParse(n, out int v))
                    .Select(n => Convert.ToInt32(n))
                    .ToArray();
            }

        }


        public static int MinOfMany(params int[] items)
        {
            var result = items[0];
            for(int i = 1; i < items.Length; i++)
            {
                result = Math.Min(result, items[i]);
            }
            return result;
        }

        public static int MaxOfMany(params int[] items)
        {
            var result = items[0];
            for(int i = 1; i < items.Length; i++)
            {
                result = Math.Max(result, items[i]);
            }
            return result;
        }

        // https://stackoverflow.com/a/3150821/419956 by @RonWarholic
        public static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for(int row = 0; row < map.GetLength(0); row++)
            {
                for(int col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }

        public static string JoinAsStrings<T>(this IEnumerable<T> items)
        {
            return string.Join("", items);
        }

        public static List<string> SplitByNewline(this string input, bool blankLines = true, bool shouldTrim = false)
        {
            return input
                .Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None)
                .Where(s => blankLines || !string.IsNullOrWhiteSpace(s))
                .Select(s => shouldTrim ? s.Trim() : s)
                .ToList();
        }

        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static int ManhattanDistance((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public static double FindGCD(double a, double b) => (a % b == 0) ? b : FindGCD(b, a % b);

        public static long FindGCD(long a, long b) => (a % b == 0) ? b : FindGCD(b, a % b);

        public static double FindLCM(double a, double b) => a * b / FindGCD(a, b);

        public static void Repeat(this Action action, int count)
        {
            for(int i = 0; i < count; i++) action();
        }

        // https://github.com/tslater2006/AdventOfCode2019
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> values)
        {
            return (values.Count() == 1) ? new[] { values } : values.SelectMany(v => Permutations(values.Where(x => x.Equals(v) == false)), (v, p) => p.Prepend(v));
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for(var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        // https://stackoverflow.com/questions/49190830/is-it-possible-for-string-split-to-return-tuple
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            rest = list.Skip(2).ToList();
        }

        public static (int, int) Add(this (int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);

        /// <summary>
        /// Flips the Array vertically, so the first row becomes the last row.
        /// It is assumed the array is not ragged.
        /// </summary>
        /// <typeparam name="TValue">Data type contained in the array.</typeparam>
        /// <param name="oldValue">2D array to be flipped.</param>
        /// <returns>A NEW 2D array populated with vertically flipped shallow copy.</returns>
        public static TValue[,] FlipVertically<TValue>(this TValue[,] oldValue) {
            int xLength = oldValue.GetLength(0),
                yLength = oldValue.GetLength(1);
            var newValue = new TValue[xLength, yLength];
            for( int ox = 0, nx = xLength - 1; ox < xLength; ox++, nx-- ) {
                for( int y = 0; y < yLength; y++ ) {
                    newValue[nx, y] = oldValue[ox, y];
                }
            }
            return newValue;
        }

        /// <summary>
        /// Flips the Array horizontally, so the first column becomes the last column.
        /// It is assumed the array is not ragged.
        /// </summary>
        /// <typeparam name="TValue">Data type contained in the array.</typeparam>
        /// <param name="oldValue">2D array to be flipped.</param>
        /// <returns>A NEW 2D array populated with horizontally flipped shallow copy.</returns>
        public static TValue[,] FlipHorizontally<TValue>(this TValue[,] oldValue) {
            int xLength = oldValue.GetLength(0),
                yLength = oldValue.GetLength(1);
            var newValue = new TValue[xLength, yLength];
            for( int x = 0; x < xLength; x++ ) {
                for( int oy = 0, ny = yLength - 1; oy < yLength; oy++, ny-- ) {
                    newValue[x, ny] = oldValue[x, oy];
                }
            }
            return newValue;
        }

        /// <summary>
        /// Rotates the Array clockwise by 90 degrees.
        /// It is assumed the array is not ragged.
        /// </summary>
        /// <typeparam name="TValue">Data type contained in the array.</typeparam>
        /// <param name="oldValue">2D array to be rotated.</param>
        /// <returns>A NEW 2D array populated with teh rotated shallow copy.</returns>
        public static TValue[,] RotateClockwise<TValue>(this TValue[,] oldValue) {
            int xLength = oldValue.GetLength(0),
                yLength = oldValue.GetLength(1);
            var newValue = new TValue[xLength, yLength];
            for( int x = 0; x < xLength; x++ ) {
                for( int y = 0; y < yLength; y++ ) {
                    newValue[yLength - 1 - y, x] = oldValue[x, y];
                }
            }
            return newValue;
        }

        /// <summary>
        /// Rotates the Array counter clockwise by 90 degrees.
        /// It is assumed the array is not ragged.
        /// </summary>
        /// <typeparam name="TValue">Data type contained in the array.</typeparam>
        /// <param name="oldValue">2D array to be rotated.</param>
        /// <returns>A NEW 2D array populated with teh rotated shallow copy.</returns>
        public static TValue[,] RotateCounterClockwise<TValue>(this TValue[,] oldValue) {
            int xLength = oldValue.GetLength(0),
                yLength = oldValue.GetLength(1);
            var newValue = new TValue[xLength, yLength];
            for( int x = 0; x < xLength; x++ ) {
                for( int y = 0; y < yLength; y++ ) {
                    newValue[y, xLength - 1 - x] = oldValue[x, y];
                }
            }
            return newValue;
        }

    }
}
