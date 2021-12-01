using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day18 : ASolution
    {
        enum Op
        {
            Sum, Mult
        }
        public Day18() : base(18, 2020, "Operation Order", false)
        {
            
        }

        long MultSum(string line, ref int at) {
            long pval;
            long? lhs = null, rhs = null;
            Op? operation = null;
            for(; at < line.Length; ++at) {
                if( line[at] == ')' ) return lhs.Value;
                if( line[at] == '(' ) {
                    at++;
                    pval = MultSum(line, ref at);
                    if( lhs == null ) lhs = pval;
                    else rhs = pval;
                }
                if( line[at] == '*' ) operation = Op.Mult;
                if( line[at] == '+' ) operation = Op.Sum;
                if( long.TryParse(line[at].ToString(), out pval) ) {
                    if( lhs == null ) lhs = pval;
                    else rhs = pval;
                }
                if( lhs.HasValue && rhs.HasValue && operation.HasValue ) {
                    switch( operation.Value ) {
                        case Op.Sum:
                            lhs = lhs.Value + rhs.Value;
                            break;
                        case Op.Mult:
                            lhs = lhs.Value * rhs.Value;
                            break;
                        default:
                            break;
                    }
                    rhs = null;
                    operation = null;
                }
            }
            return lhs.Value;
        }

        protected override string SolvePartOne()
        {
            List<long> values = new List<long>();
            long sum = 0;
            foreach(string line in Input.SplitByNewline(false) ) {
                int at = 0;
                long result = MultSum(line.Replace(" ", string.Empty), ref at);
                values.Add(result);
                sum = sum + result;
            }

            return sum.ToString();
        }


        long MultSum2(string line, ref int at, bool breakAtMult) {
            long pval;
            long? lhs = null, rhs = null;
            Op? operation = null;
            for( ; at < line.Length; ++at ) {
                if( line[at] == ')' ) return lhs.Value;
                else if( line[at] == '(' ) {
                    at++;
                    pval = MultSum2(line, ref at, false);
                    if( lhs == null ) lhs = pval;
                    else rhs = pval;
                }
                else if( long.TryParse(line[at].ToString(), out pval) ) {
                    if( lhs == null ) lhs = pval;
                    else rhs = pval;
                }
                else if( line[at] == '*' ) {
                    if( breakAtMult ) {
                        return lhs ?? 0;
                    }
                    at++;
                    operation = Op.Mult;
                    rhs = MultSum2(line, ref at, true);
                    at--;
                }
                else if( line[at] == '+' ) operation = Op.Sum;
                if( lhs.HasValue && rhs.HasValue && operation.HasValue ) {

                    switch( operation.Value ) {
                        case Op.Sum:
                            lhs = lhs.Value + rhs.Value;
                            break;
                        case Op.Mult:
                            lhs = lhs.Value * rhs.Value;
                            break;
                        default:
                            break;
                    }
                    rhs = null;
                    operation = null;
                }
            }
            return lhs.Value;
        }

        protected override string SolvePartTwo()
        {
            List<long> values = new List<long>();
            long sum = 0;
            foreach( string line in Input.SplitByNewline(false) ) {
                string mutLine = line.Replace(" ", string.Empty);
                int at = 0;
                long result = MultSum2(mutLine, ref at, false);
                values.Add(result);
                sum = sum + result;
            }

            return sum.ToString();
        }
    }
}
