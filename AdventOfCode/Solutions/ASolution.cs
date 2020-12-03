using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;

namespace AdventOfCode.Solutions
{

    abstract class ASolution
    {

        Lazy<string> _input, _part1, _part2;

        public int Day { get; }
        public int Year { get; }
        public string Title { get; }
        public string DebugInput { get; set; }
        public string Input => string.IsNullOrEmpty(_input.Value) ? null : _input.Value;
        public string Part1 => string.IsNullOrEmpty(_part1.Value) ? "" : _part1.Value;
        public string Part2 => string.IsNullOrEmpty(_part2.Value) ? "" : _part2.Value;
        protected bool UseDebugInput { get; set; }

        private protected ASolution(int day, int year, string title)
        {
            Day = day;
            Year = year;
            Title = title;
            _input = new Lazy<string>(LoadInput);
            _part1 = new Lazy<string>(() => SafelySolve(SolvePartOne));
            _part2 = new Lazy<string>(() => SafelySolve(SolvePartTwo));
        }

        public void Solve(int part = 0)
        {
            if(Input == null) return;

            bool doOutput = false;
            string output = $"--- Day {Day}: {Title} --- \n";
            if(DebugInput != null)
            {
                output += $"!!! DebugInput used: {DebugInput}\n";
            }

            if(part != 2)
            {
                if(Part1 != "")
                {
                    output += $"Part 1: {Part1}\n";
                    doOutput = true;
                }
                else
                {
                    output += "Part 1: Unsolved\n";
                    if(part == 1) doOutput = true;
                }
            }
            if(part != 1)
            {
                if(Part2 != "")
                {
                    output += $"Part 2: {Part2}\n";
                    doOutput = true;
                }
                else
                {
                    output += "Part 2: Unsolved\n";
                    if(part == 2) doOutput = true;
                }
            }

            if( doOutput ) {
                System.Diagnostics.Trace.WriteLine(output);
                Console.WriteLine(output);
            }
        }

        string LoadInput()
        {
            string DEBUGINPUT_FILEPATH = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"../../../Solutions/Year{Year}/Day{Day:D2}/debugInput"));
            string INPUT_FILEPATH = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"../../../Solutions/Year{Year}/Day{Day:D2}/input"));
            string INPUT_URL = $"https://adventofcode.com/{Year}/day/{Day}/input";
            string input = "";

            if( UseDebugInput && File.Exists(DEBUGINPUT_FILEPATH) && new FileInfo(DEBUGINPUT_FILEPATH).Length > 0) {
                input = DebugInput = File.ReadAllText(DEBUGINPUT_FILEPATH);
            }
            else if(File.Exists(INPUT_FILEPATH) && new FileInfo(INPUT_FILEPATH).Length > 0)
            {
                input = File.ReadAllText(INPUT_FILEPATH);
            }
            else
            {
                try
                {
                    DateTime CURRENT_EST = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc).AddHours(-5);
                    if(CURRENT_EST < new DateTime(Year, 12, Day)) throw new InvalidOperationException();

                    using(var client = new WebClient())
                    {
                        client.Headers.Add(HttpRequestHeader.Cookie, Program.Config.Cookie);
                        input = client.DownloadString(INPUT_URL).Trim();
                        File.WriteAllText(INPUT_FILEPATH, input);
                    }
                }
                catch(WebException e)
                {
                    var statusCode = ((HttpWebResponse)e.Response).StatusCode;
                    if(statusCode == HttpStatusCode.BadRequest)
                    {
                        Console.WriteLine($"Day {Day}: Error code 400 when attempting to retrieve puzzle input through the web client. Your session cookie is probably not recognized.");
                    }
                    else if(statusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Day {Day}: Error code 404 when attempting to retrieve puzzle input through the web client. The puzzle is probably not available yet.");
                    }
                    else
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                catch(InvalidOperationException)
                {
                    Console.WriteLine($"Day {Day}: Cannot fetch puzzle input before given date (Eastern Standard Time).");
                }
            }
            return input;
        }

        protected abstract string SolvePartOne();
        protected abstract string SolvePartTwo();

        private string SafelySolve(Func<string> partSolver)
        {
            try {
                return partSolver();
            }
            catch(Exception ex) {
                Trace.TraceError($"Caught Exception:\r\n{ex}");
                Debugger.Break();
                return string.Empty;
            }
        }
    }
}
