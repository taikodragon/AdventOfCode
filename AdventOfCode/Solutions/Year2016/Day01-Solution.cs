using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2016;

[DayInfo(2016, 01, "No Time for a Taxicab")]
class Day01 : ASolution
{

    public Day01() : base(false)
    {
            
    }

    // Collect the input into a usable structure based so we can iterate against it easily.
    List<(char leftRight, int dist)> instructions = new();

    // Called before either of the Solve methods to handle Input parsing
    protected override void ParseInput()
    {
        // Add the output of our parsing to instructions
        instructions.AddRange(
            Input // This will automatically read in all the input for this days puzzle.
            // the input is all on one line, so split it by ", " as the appears to the a reasonable delimiter.
            .Split(", ") 
            // Perform a map against the results of Split()
            // Returns a tuple of the first character and an integer parsed from the remainer of the string
            .Select(s => (s[0], int.Parse(s[1..])))
        );
    }

    protected override object SolvePartOneRaw()
    {
        // The puzzle indicates we start facing North, we'll want to track that.
        CompassDirection heading = CompassDirection.N;
        // We use integer coordinates as that is an easy way to represent our position within the city.
        // Zero is picked arbitarily here.
        Int2 at = Int2.Zero;

        // Iterate through each instruction
        foreach(var inst in instructions) {
            // On each Left or Right, use a Utility method to "rotate" heading.
            if (inst.leftRight == 'L') heading = Utilities.CompassLeft90(heading);
            else if (inst.leftRight == 'R') heading = Utilities.CompassRight90(heading);

            // Move our position in the direction of heading for the specified distance.
            at = Int2.Offset(at, heading, inst.dist);
        }
        // Per the puzzle instructions, return the ManhattanDistance from our starting point to the current location.
        return Int2.ManhattanDistance(Int2.Zero, at);
    }

    protected override object SolvePartTwoRaw()
    {
        CompassDirection heading = CompassDirection.N;
        Int2 at = Int2.Zero;

        // A hashset is used as it has a fast look-up time compared to a general list/array.
        HashSet<Int2> visited = new() { at };
        foreach (var inst in instructions) {
            if (inst.leftRight == 'L') heading = Utilities.CompassLeft90(heading);
            else if (inst.leftRight == 'R') heading = Utilities.CompassRight90(heading);

            // This time we can't use the distance parameter of Int2.Offset, we might intersect *along* the way while walking an instruction.
            bool dup = false; // senintel to determine if we broke out due to an interection or finished
            for(int remaining = inst.dist; remaining > 0 && !dup; remaining--) {
                at = Int2.Offset(at, heading); // distance defaults to one.

                // Search visited for the new location.
                // If it has been seen then stop, otherwise add this location to the set.
                if (visited.Contains(at)) dup = true;
                else visited.Add(at);
            }
            // If we broke out due to revisiting an intersection, we should also stop processing instructions.
            if (dup) break;
        }
        // Return the distance from the location we're currently at, this will be where we revisited or the end.
        return Int2.ManhattanDistance(Int2.Zero, at);
    }
}
