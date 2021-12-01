using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{

    class Day07 : ASolution
    {
        class BagRule
        {
            public string Color { get; set; }
            public int RequiredCount { get; set; }
        }

        Dictionary<string, List<string>> containedBy = new Dictionary<string, List<string>>();
        Dictionary<string, List<BagRule>> rules = new Dictionary<string, List<BagRule>>();
        List<string> lines;
        public Day07() : base(07, 2020, "Handy Haversacks", false)
        {
            

            lines = Input.SplitByNewline().Select(s => s.Trim('.')).ToList();

            foreach( string line in lines ) {
                if( string.IsNullOrEmpty(line) ) continue;
                string[] keyRule = line.Split("contain");
                string key = ParseBagName(keyRule[0]).Color;
                keyRule[1].Split(',').ToList().ForEach(s => AddRule(key, ParseBagName(s)));
            }

        }

        void AddRule(string keyBag, BagRule containsBag)
        {
            if( !rules.ContainsKey(keyBag) ) {
                rules.Add(keyBag, new List<BagRule>());
            }
            if( !containedBy.ContainsKey(containsBag.Color) ) {
                containedBy.Add(containsBag.Color, new List<string>());
            }
            if( containsBag.Color != "no other" ) {
                rules[keyBag].Add(containsBag);
                containedBy[containsBag.Color].Add(keyBag);
            }
        }

        BagRule ParseBagName(string name)
        {
            var res = new BagRule {
                Color = Regex.Replace(name, "[0-9]+ | bag[s]?", string.Empty).Trim(),
            };
            if( int.TryParse(name.Trim()[0].ToString(), out int cnt) )
                res.RequiredCount = cnt;
            return res;
        }

        protected override string SolvePartOne()
        {
            List<string> canContain = new List<string>();
            Queue<string> search = new Queue<string>();
            search.Enqueue("shiny gold");
            while(search.Count != 0) {
                string me = search.Dequeue();
                if( containedBy.TryGetValue(me, out var containers)) {
                    containers.ForEach(s => search.Enqueue(s));
                }
                Trace.WriteLine(me);
                if( me != "shiny gold" && !canContain.Contains(me)) {
                    canContain.Add(me);
                }
            }

            return canContain.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            int bags = -1;
            Queue<List<BagRule>> searchRules = new Queue<List<BagRule>>();
            searchRules.Enqueue(rules["shiny gold"]);

            while(searchRules.Count != 0) {
                bags++;
                var bagRules = searchRules.Dequeue();
                foreach(var rule in bagRules) {
                    var childRules = rules[rule.Color];
                    for(int i = 0; i < rule.RequiredCount; i++) {
                        searchRules.Enqueue(childRules);
                    }
                }
            }

            return bags.ToString();
        }
    }
}
