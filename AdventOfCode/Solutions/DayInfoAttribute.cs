using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class DayInfoAttribute : Attribute
    {
        public int Day { get; }
        public int Year { get; }
        public string Title { get; }

        public DayInfoAttribute(int year, int day, string title) {
            Day = day;
            Year = year;
            Title = title;
        }
    }
}
