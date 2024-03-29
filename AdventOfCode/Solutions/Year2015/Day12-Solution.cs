using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{

    [DayInfo(2015, 12, "JSAbacusFramework.io")]
    class Day12 : ASolution
    {

        public Day12() : base(false)
        {
            
        }

        protected override string SolvePartOne()
        {
            long sum = 0;
            var jReader = new System.Text.Json.Utf8JsonReader(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(Input)));
            while(jReader.Read()) {
                if( jReader.TokenType == System.Text.Json.JsonTokenType.Number ) {
                    sum += jReader.GetInt64();
                }
            }
            return sum.ToString();
        }

        protected override string SolvePartTwo()
        {

            Dictionary<string, long> pathSums = new Dictionary<string, long>();
            HashSet<string> redPaths = new HashSet<string>();
            using( StringReader reader = new StringReader(Input) ) {
                using( var jReader = new JsonTextReader(reader) ) {
                    Stack<string> objPath = new Stack<string>();
                    string atObjPath = null;
                    while( jReader.Read() ) {
                        switch(jReader.TokenType) {
                            default: break;
                            case JsonToken.StartObject:
                                objPath.Push(atObjPath);
                                atObjPath = jReader.Path;
                                break;
                            case JsonToken.EndObject:
                                atObjPath = objPath.Pop();
                                break;
                            case JsonToken.String:
                                if( (string)jReader.Value == "red" && !jReader.Path.EndsWith(']') )
                                    redPaths.Add(atObjPath);
                                break;
                            case JsonToken.Integer:
                                if( !pathSums.ContainsKey(atObjPath) )
                                    pathSums[atObjPath] = 0;

                                pathSums[atObjPath] += (Int64)jReader.Value;
                                break;
                        }
                    }
                }
            }
            return pathSums.Where(kv => !redPaths.Any(p => kv.Key.StartsWith(p))).Sum(kv => kv.Value).ToString();
        }
    }
}
