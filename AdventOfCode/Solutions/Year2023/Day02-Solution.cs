using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2023;

[DayInfo(2023, 02, "Cube Conundrum")]
class Day02 : ASolution
{

    public Day02() : base(false)
    {
            
    }

    Dictionary<int, List<(int red, int green, int blue)>> games = new();
    Dictionary<int, (int red, int green, int blue)> gameMaxColors = new();
    protected override void ParseInput()
    {
        foreach(var line in Input.SplitByNewline(false, true)) {
            var idAndParts = line.Split(':', ';');
            int id = int.Parse(idAndParts[0].Split(' ')[^1]);
            List<(int, int, int)> rounds = new();
            int maxRed = 0, maxBlue = 0, maxGreen = 0;
            foreach(string part in idAndParts[1..]) {
                var rgb = part.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                int reds = 0, greens = 0, blues = 0;
                foreach(string cubeCount in rgb) {
                    var numAndColor = cubeCount.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    switch(numAndColor[1]) {
                        case "red": reds = int.Parse(numAndColor[0]); break;
                        case "green": greens = int.Parse(numAndColor[0]); break;
                        case "blue": blues = int.Parse(numAndColor[0]); break;
                        default: throw new Exception("Unknown color");
                    }
                }
                maxRed = Math.Max(maxRed, reds);
                maxGreen = Math.Max(maxGreen, greens);
                maxBlue = Math.Max(maxBlue, blues);
                rounds.Add((reds, greens, blues));
            }
            gameMaxColors.Add(id, (maxRed, maxGreen, maxBlue));

            games.Add(id, rounds);
        }
    }



    protected override object SolvePartOneRaw()
    {
        //only 12 red cubes, 13 green cubes, and 14 blue cubes
        var filter = (red: 12, green: 13, blue: 14);

        return gameMaxColors
            .Where(kv => {
                return kv.Value.red <= filter.red &&
                kv.Value.green <= filter.green &&
                kv.Value.blue <= filter.blue;
            })
            .Select(kv => kv.Key)
            .Sum();
    }

    protected override object SolvePartTwoRaw()
    {
        return gameMaxColors
            .Select(kv => kv.Value.red * kv.Value.green * kv.Value.blue)
            .Sum();
    }
}
