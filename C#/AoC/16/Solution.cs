using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._16
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
            var (rules, _, otherTickets) = await ParseInput();
            return otherTickets.SelectMany(t => t)
                               .Where(value => !IsValid(value, rules))
                               .Sum();
        }

        private async Task<long> Part2()
        {
            var (rules, myTicket, otherTickets) = await ParseInput();
            var validOtherTickets = otherTickets.Where(ticket => ticket.All(value => IsValid(value, rules))).ToList();

            var possibleValues = rules.ToDictionary(r => r.Key, r => myTicket.Select((_, idx) => idx).ToList());
            while (possibleValues.Values.Any(v => v.Count > 1))
            {
                foreach (var rule in rules)
                {
                    var field = rule.Key;
                    var ranges = rule.Value;
                    var possible = possibleValues[field];

                    if (possible.Count == 1)
                    {
                        var value = possible.Single();
                        foreach (var key in possibleValues.Keys.Except(new[] {field}))
                        {
                            possibleValues[key].Remove(value);
                        }
                    }
                    else
                    {
                        foreach (var ticket in validOtherTickets)
                        {
                            foreach (var value in possible.ToList())
                            {
                                if (!IsValid(ticket[value], ranges))
                                {
                                    possibleValues[field].Remove(value);
                                }
                            }
                        }
                    }
                }
            }

            return possibleValues.Where(p => p.Key.StartsWith("departure")).Select(p => myTicket[p.Value.Single()])
                                 .Aggregate(1L, (agg, val) => agg * val);
        }

        private bool IsValid(int value, Dictionary<string, List<(int Low, int High)>> rules)
        {
            return rules.Any(rule => rule.Value.Any(range => IsValid(value, range)));
        }

        private bool IsValid(int value, List<(int Low, int High)> ranges)
        {
            return ranges.Any(range => IsValid(value, range));
        }

        private bool IsValid(int value, (int Low, int High) range)
        {
            return range.Low <= value && value <= range.High;
        }

        private async Task<(Dictionary<string, List<(int Low, int High)>> Rules, List<int> MyTicket, List<List<int>> OtherTickets)> ParseInput()
        {
            var text = await File.ReadAllTextAsync("16/input");

            var lines = text.Split("\r\n").ToList();

            var current = 0;

            var rules = new Dictionary<string, List<(int Low, int High)>>();
            var ruleRegex = new Regex("^(?<field>[\\w\\s]+): (?<ranges>.*)$");
            while (current < lines.Count)
            {
                var currentLine = lines[current];

                current++;
                if (string.IsNullOrWhiteSpace(currentLine) || !ruleRegex.IsMatch(currentLine))
                {
                    break;
                }

                var match = ruleRegex.Match(currentLine);
                var field = match.Groups["field"].Value;
                var rangesStr = match.Groups["ranges"].Value;
                var ranges = rangesStr.Split(" or ").Select(r =>
                {
                    var splt = r.Split("-");
                    var low = Convert.ToInt32(splt[0]);
                    var high = Convert.ToInt32(splt[1]);
                    return (Low: low, High: high);
                }).ToList();

                rules[field] = ranges;
            }

            current += 1;

            var myTicket = lines[current].Split(",").Select(i => Convert.ToInt32(i)).ToList();

            current += 3;

            List<List<int>> otherTickets = new List<List<int>>();
            while (current < lines.Count)
            {
                var currentLine = lines[current];
                otherTickets.Add(currentLine.Split(",").Select(i => Convert.ToInt32(i)).ToList());
                current++;
            }

            return (rules, myTicket, otherTickets);
        }


    }
}