using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._07
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
            var rules = await ParseInput();

            var canContainGold =
                new HashSet<string>(rules.Where(r => r.Contents.Any(c => c.Colour == "shiny gold"))
                                         .Select(r => r.Colour));
            while (true)
            {
                var currentCount = canContainGold.Count;
                foreach (var rule in rules.Where(r => r.Contents.Any(c => canContainGold.Contains(c.Colour))))
                {
                    canContainGold.Add(rule.Colour);
                }

                if (canContainGold.Count == currentCount)
                {
                    break;
                }
            }

            return canContainGold.Count;
        }

        private async Task<long> Part2()
        {
            var rules = await ParseInput();

            var toProcess = new Stack<BagContent>(new List<BagContent> { new BagContent(1, "shiny gold") });
            var totalBags = 0;

            while (toProcess.Any())
            {
                var bc = toProcess.Pop();

                var rule = rules.Single(r => r.Colour == bc.Colour);

                foreach (var c in rule.Contents)
                {
                    toProcess.Push(new BagContent(c.Number * bc.Number, c.Colour));
                    totalBags += c.Number * bc.Number;
                }
            }

            return totalBags;
        }

        private async Task<List<BagRule>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("07/input");
            return text
                   .Split("\r\n")
                   .Select(BagRule.Parse)
                   .ToList();
        }

        public sealed class BagRule
        {
            public string Colour { get; }

            public List<BagContent> Contents { get; }

            public BagRule(string colour, List<BagContent> contents)
            {
                Colour = colour;
                Contents = contents;
            }

            public static BagRule Parse(string str)
            {
                var regex = new Regex("^(?<colour>[\\w\\s]+) bags contain (?<contents>.*)\\.$");
                var match = regex.Match(str.Trim());

                var bagColour = match.Groups["colour"].Value;
                var contentsStr = match.Groups["contents"].Value;

                List<BagContent> contents = new List<BagContent>();
                if (contentsStr != "no other bags")
                {
                    var contentRegex = new Regex("^(?<number>\\d+) (?<colour>[\\w\\s]+) bags?$");
                    var contentsStrs = contentsStr.Split(", ");
                    foreach (var cStr in contentsStrs)
                    {
                        var cMatch = contentRegex.Match(cStr.Trim());
                        var cNumber = Convert.ToInt32(cMatch.Groups["number"].Value);
                        var cColour = cMatch.Groups["colour"].Value;
                        contents.Add(new BagContent(cNumber, cColour));
                    }
                }
                return new BagRule(bagColour, contents);
            }
        }

        public sealed class BagContent
        {
            public int Number { get; }

            public string Colour { get; }

            public BagContent(int number, string colour)
            {
                Number = number;
                Colour = colour;
            }
        }
    }
}