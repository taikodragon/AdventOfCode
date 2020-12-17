using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day14 : ASolution
    {
        readonly List<(string cmd, string value)> lines;
        public Day14() : base(14, 2020, "Docking Data")
        {
            UseDebugInput = false;

            lines = Input
                .Replace("mem[", string.Empty)
                .Replace("]", string.Empty)
                .SplitByNewline()
                .Select(l => {
                    var pair = l.Split('=');
                    return (pair[0].Trim(), pair[1].Trim());
                }).ToList();
        }

        protected override string SolvePartOne()
        {
            Dictionary<int, ulong> memory = new Dictionary<int, ulong>();
            string mask = string.Empty;
            int maskLength = 0;
            foreach(var inst in lines) {
                if(inst.cmd == "mask") {
                    mask = inst.value;
                    maskLength = mask.Length - 1;
                    continue;
                }
                int memAddr = int.Parse(inst.cmd);
                ulong newValue = ulong.Parse(inst.value);
                
                // do the zeros
                for(int i = maskLength; i >= 0; --i) {
                    int shift = maskLength - i;
                    if( mask[i] == '0') {
                        newValue &= (~(0x1ul << shift));
                    }
                    if( mask[i] == '1' ) {
                        newValue |= (0x1ul << shift);
                    }
                }
                memory[memAddr] = newValue;
            }

            ulong sum = 0;
            foreach( var p in memory ) sum += p.Value;
            return sum.ToString();
        }

        protected override string SolvePartTwo()
        {
            Dictionary<ulong, ulong> memory = new Dictionary<ulong, ulong>();
            List<string> masks = new List<string>(256);
            Queue<string> mp = new Queue<string>(256);
            int maskLength = 0;
            foreach( var inst in lines ) {
                if( inst.cmd == "mask" ) {
                    masks.Clear();
                    maskLength = inst.value.Length - 1;
                    mp.Enqueue(inst.value);
                    int i = inst.value.Length - 1;
                    while(mp.Count > 0) {
                        string mask = mp.Dequeue();
                        bool didQueue = false;
                        for( ; i >= 0; --i ) {
                            if(mask[i] == 'X') {
                                string m1 = mask.Substring(0, i),
                                    m2 = mask.Substring(i+1, (maskLength) - i);
                                mp.Enqueue(string.Concat(m1, "Z", m2));
                                mp.Enqueue(string.Concat(m1, "1", m2));
                                didQueue = true;
                                break;
                            }
                        }
                        if(!didQueue) {
                            masks.Add(mask);
                        }
                    }
                    continue;
                }
                ulong memAddrOri = ulong.Parse(inst.cmd);
                ulong newValue = ulong.Parse(inst.value);

                foreach( string mask in masks ) {
                    ulong memAddr = memAddrOri;
                    for( int i = maskLength; i >= 0; --i ) {
                        int shift = maskLength - i;
                        if( mask[i] == 'Z' ) {
                            memAddr &= (~(0x1ul << shift));
                        }
                        else if( mask[i] == '1' ) {
                            memAddr |= (0x1ul << shift);
                        }
                    }
                    memory[memAddr] = newValue;
                }
            }

            ulong sum = 0;
            foreach( var p in memory ) sum += p.Value;
            return sum.ToString();
        }
    }
}

