using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{
    class Food
    {
        public HashSet<string> Ingredients = new HashSet<string>();
        public HashSet<string> Allergens = new HashSet<string>();
    }
    [DayInfo(2020, 21, "Allergen Assessment")]
    class Day21 : ASolution
    {
        List<Food> foods = new List<Food>();
        Dictionary<string, List<Food>> allerCache = new Dictionary<string, List<Food>>();
        Dictionary<string, List<Food>> ingreCache = new Dictionary<string, List<Food>>();
        Dictionary<string, string> allergenToIngredient = new Dictionary<string, string>();
        HashSet<string> allIngredients = new HashSet<string>();

        public Day21() : base(false)
        {
            

            foreach(string line in Input.SplitByNewline()) {
                var ingrAller = line.Split("(contains ").Select(p => p.Trim(')').Trim()).ToList();
                if( ingrAller.Count != 2 )
                    Debugger.Break();

                Food food = new Food();
                foods.Add(food);
                foreach( string ingr in ingrAller[0].Split(' ', StringSplitOptions.RemoveEmptyEntries) ) {
                    food.Ingredients.Add(ingr);
                    allIngredients.Add(ingr);
                    if( ingreCache.TryGetValue(ingr, out var foodList) ) {
                        foodList.Add(food);
                    } else {
                        ingreCache[ingr] = new List<Food> { food };
                    }
                }
                foreach( string aller in ingrAller[1].Split(',').Select(s => s.Trim()) ) {
                    food.Allergens.Add(aller);
                    if( allerCache.TryGetValue(aller, out var foodList) ) {
                        foodList.Add(food);
                    }
                    else {
                        allerCache[aller] = new List<Food> { food };
                    }
                }

            }
        }

        protected override string SolvePartOne()
        {
            Queue<string> allergens = new Queue<string>();
            foreach( string aller in allerCache.Keys ) allergens.Enqueue(aller);

            while(allergens.Count > 0) {
                string allergen = allergens.Dequeue();
                if( allergenToIngredient.ContainsKey(allergen) )
                    continue;
                var foodList = allerCache[allergen];

                List<string> susIngre = new List<string>(foodList.First().Ingredients.Where(i => !allergenToIngredient.ContainsValue(i)));
                foreach(Food food in foodList.Skip(1)) {
                    var otherIngredients = food.Ingredients.Where(i => !allergenToIngredient.ContainsValue(i)).ToHashSet();
                    //allergenToIngredient.Values.Select(v => otherIngredients.Add(v));
                    susIngre = susIngre.Where(ingre => otherIngredients.Contains(ingre)).ToList();
                }
                if( susIngre.Count > 1) {
                    // try again later
                    allergens.Enqueue(allergen);
                } else if( susIngre.Count == 1) {
                    allergenToIngredient[allergen] = susIngre[0]; // keep track of this mapping
                    allIngredients.Remove(susIngre[0]); // remove from the universe
                } else {
                    Debugger.Break();
                }
            }

            Dictionary<string, int> counters = new Dictionary<string, int>();
            foreach( string ingre in allIngredients) {
                counters.Add(ingre, 0);
            }

            foreach(var food in foods) {
                foreach( string ingre in food.Ingredients ) {
                    if( allIngredients.Contains(ingre) ) counters[ingre]++;
                }
            }
            return counters.Values.Sum().ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Join(',', allergenToIngredient.OrderBy(p => p.Key).Select(p => p.Value));
        }
    }
}
