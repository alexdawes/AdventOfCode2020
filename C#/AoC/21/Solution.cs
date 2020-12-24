using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._21
{
    public sealed class Solution : ISolution
    {
        public async Task Run()
        {
            var part1 = await Part1();
            Console.WriteLine($"Part 1: {part1}");

            var part2 = await Part2();
            Console.WriteLine($"Part 2: {part2}");
        }

        private async Task<long> Part1()
        {
            var items = await ParseInput();

            var possibilities = items.SelectMany(i => i.Allergens).Distinct()
                                     .ToDictionary(
                                         allergen => allergen,
                                         allergen =>
                                         {
                                             var itemsContainingAllergen =
                                                 items.Where(item => item.Allergens.Contains(allergen)).ToList();
                                             return new HashSet<string>(itemsContainingAllergen.Skip(1)
                                                                           .Aggregate(
                                                                               itemsContainingAllergen[0].Ingredients,
                                                                               (agg, item) =>
                                                                                   agg.Intersect(item.Ingredients)
                                                                                       .ToList()));
                                         });

            var ingredientsWithoutAllergens = items.SelectMany(item => item.Ingredients).Distinct()
                                                   .Where(ingredient =>
                                                              !possibilities.Values.SelectMany(v => v)
                                                                            .Contains(ingredient)).ToList();

            return items.Sum(item => item.Ingredients.Intersect(ingredientsWithoutAllergens).Count());
        }

        private async Task<string> Part2()
        {
            var items = await ParseInput();

            var possibilities = items.SelectMany(i => i.Allergens).Distinct()
                                     .ToDictionary(
                                         allergen => allergen,
                                         allergen =>
                                         {
                                             var itemsContainingAllergen =
                                                 items.Where(item => item.Allergens.Contains(allergen)).ToList();
                                             return new HashSet<string>(itemsContainingAllergen.Skip(1)
                                                                            .Aggregate(
                                                                                itemsContainingAllergen[0].Ingredients,
                                                                                (agg, item) =>
                                                                                    agg.Intersect(item.Ingredients)
                                                                                        .ToList()));
                                         });

            var allergens = new Dictionary<string, string>();
            while (possibilities.Values.Any())
            {
                foreach (var kvp in possibilities.Where(kvp => kvp.Value.Count == 1).ToList())
                {
                    var allergen = kvp.Key;
                    var ingredient = kvp.Value.Single();
                    allergens[allergen] = ingredient;
                    foreach (var v in possibilities.Values)
                    {
                        v.Remove(ingredient);
                    }

                    possibilities.Remove(allergen);
                }
            }

            return string.Join(",",allergens.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

        }

        public static async Task<List<(List<string> Ingredients, List<string> Allergens)>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("21/input");
            var lines = text.Split("\r\n");
            return lines.Select(line =>
            {
                var regex = new Regex("^(?<ingredients>[\\w\\s]+) \\(contains (?<allergens>.*)\\)$");
                var match = regex.Match(line);
                var ingredients = match.Groups["ingredients"].Value.Split(" ").ToList();
                var allergens = match.Groups["allergens"].Value.Split(", ").ToList();
                return (ingredients, allergens);
            }).ToList();
        }
    }

}