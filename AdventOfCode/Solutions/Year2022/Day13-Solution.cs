using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 13, "Distress Signal")]
class Day13 : ASolution
{
    public Day13() : base(false) { }

    List<Packet[]> input = new List<Packet[]>();
    protected override void ParseInput() {
        input = Input.SplitByNewline(false, true)
            .Select(l => new Packet(l))
            .Chunk(2)
            .ToList();
    }
    protected override object SolvePartOneRaw() {
        List<int> rightOrderPairs = new();
        int i = 1;
        foreach(var pair in input) {
            Packet left = pair[0], right = pair[1];
            if( left.InOrder(right) == true ) {
                rightOrderPairs.Add(i);
            }
            i++;
        }
        return rightOrderPairs.Sum();
    }
    protected override object SolvePartTwoRaw() {
        Packet div1 = new Packet("[[2]]");
        Packet div2 = new Packet("[[6]]");

        List<Packet> ordered = new(input.SelectMany(l => l)) {
            div1,
            div2
        };

        ordered.Sort((lhs, rhs) => {
            return lhs.InOrder(rhs) switch {
                true => -1,
                false => 1,
                null => 0
            };
        });

        int idx1 = ordered.IndexOf(div1) + 1;
        int idx2 = ordered.IndexOf(div2) + 1;

        return idx1 * idx2;
    }

    internal class Packet
    {
        public int? Num { get; set; }
        public List<Packet> Sub { get; set; } = new();
        public bool IsList => Num is null;

        public Packet(int num, bool asList = false) {
            if( asList ) {
                Sub.Add(new Packet(num));
            }
            else {
                Num = num;
            }
        }
        public Packet(string txt) {
            int firstBracket = txt.IndexOf('[');
            int lastBracket = txt.LastIndexOf(']');
            string content = txt.Substring(firstBracket +1, lastBracket - firstBracket - 1);
            while (content.Length > 0) {
                if (content[0] == '[') {
                    int open = 1, i = 1;
                    for (; i < content.Length && open > 0; i++) {
                        if (content[i] == '[') {
                            open++;
                        }
                        else if (content[i] == ']') {
                            open--;
                        }
                    }
                    if (open == 0) {
                        Sub.Add(new Packet(content.Substring(0, i)));
                        content = content.Substring(i).TrimStart(',');
                    }
                    else throw new Exception("Malformed input");
                }
                else {
                    int idx = content.IndexOf(',');
                    
                    string digit;
                    if (idx == -1) digit = content;
                    else digit = content.Substring(0, idx);

                    Sub.Add(new Packet(int.Parse(digit)));

                    if (idx == -1) content = string.Empty;
                    else content = content.Substring(idx + 1);
                }
            }
        }

        public bool? InOrder(Packet right) {
            if( IsList ) {
                if( !right.IsList) {
                    return this.InOrder(new Packet(right.Num.Value, true));
                }
                int i = 0;
                for(; i < Sub.Count && i < right.Sub.Count; i++) {
                    bool? result = Sub[i].InOrder(right.Sub[i]);
                    if (result is not null) return result;
                }
                if( Sub.Count == right.Sub.Count ) { // Same length, continue
                    return null;
                }
                else if( i == Sub.Count ) { // Left ran out first
                    return true;
                }
                else { // Right ran out first
                    return false;
                }
            }
            else { // I'm not list, check if right is
                if( right.IsList ) {
                    return (new Packet(Num.Value, true)).InOrder(right);
                }
                // Both are not lists
                if (Num == right.Num) return null;
                return Num < right.Num;
            }
        }

    }
}
