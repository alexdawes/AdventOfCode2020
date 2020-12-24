using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._19
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
            var (rules, values) = await ParseInput();

            var regexes = new Dictionary<int, string>();
            while (!regexes.ContainsKey(0))
            {
                var toProcess = rules.Keys.Except(regexes.Keys);
                foreach (var i in toProcess)
                {
                    if (rules[i].TryGetRegex(regexes, out string regex))
                    {
                        regexes[i] = regex;
                    }
                }
            }

            var regex0 = new Regex($"^{regexes[0]}$");

            return values.Count(v => regex0.IsMatch(v));
        }

        private async Task<long> Part2()
        {
            var (rules, values) = await ParseInput();
            rules[8] = new OtherRule(8, new List<List<int>> { new List<int> { 42 }, new List<int> { 42, 8 }});
            rules[11] = new OtherRule(11, new List<List<int>> { new List<int> { 42, 31 }, new List<int> { 42, 11, 31 } });

            var regexes = new Dictionary<int, string>();
            while (!regexes.ContainsKey(31) || !regexes.ContainsKey(42))
            {
                var toProcess = rules.Keys.Except(regexes.Keys);
                foreach (var i in toProcess)
                {
                    if (rules[i].TryGetRegex(regexes, out string regex))
                    {
                        regexes[i] = regex;
                    }
                }
            }

            return values.Count(v =>
            {
                var trimmed = v;
                var regex42 = new Regex($"^{regexes[42]}");
                var regex31 = new Regex($"^{regexes[31]}");
                var matched = string.Empty;
                var count42 = 0;
                var count31 = 0;
                while (regex42.IsMatch(trimmed))
                {
                    var match = regex42.Match(trimmed);
                    count42++;
                    matched += match.Value;
                    trimmed = trimmed.Substring(match.Value.Length);
                }
                while (regex31.IsMatch(trimmed))
                {
                    var match = regex31.Match(trimmed);
                    count31++;
                    matched += match.Value;
                    trimmed = trimmed.Substring(match.Value.Length);
                }

                return matched == v && trimmed == string.Empty && count42 > count31 && count31 > 0;
            });
        }
        
        public static async Task<(Dictionary<int, Rule> Rules, List<string> Values)> ParseInput()
        {
            var text = await File.ReadAllTextAsync("19/input");
            var split = text.Split("\r\n\r\n");

            var rules = split[0].Split("\r\n").Select(Rule.Parse).ToDictionary(r => r.Id);
            var strings = split[1].Split("\r\n").ToList();

            return (rules, strings);
        }

        public abstract class Rule
        {
            public int Id { get; }

            public Rule(int id)
            {
                Id = id;
            }

            public abstract bool TryGetRegex(Dictionary<int, string> regexes, out string regex);

            public static Rule Parse(string s)
            {
                var regex = new Regex("(?<id>\\d+): (?<desc>.+)");
                var match = regex.Match(s);
                if (!match.Success)
                {
                    throw new Exception($"Invalid {nameof(Rule)}: {s}");
                }

                var id = Convert.ToInt32(match.Groups["id"].Value);
                var result = match.Groups["desc"].Value;

                if (result.StartsWith("\"") && result.EndsWith("\""))
                {
                    return new StringRule(id, result.Substring(1, result.Length - 2));
                }
                else
                {
                    return new OtherRule(id, result.Split(" | ").Select(r => r.Split(" ").Select(i => Convert.ToInt32(i)).ToList()).ToList());
                }
            }
        }

        public sealed class StringRule : Rule
        {
            private readonly string _value;

            public StringRule(int id, string value) : base(id)
            {
                _value = value;
            }


            public override bool TryGetRegex(Dictionary<int, string> regexes, out string regex)
            {
                regex = _value;
                return true;
            }
        }

        public sealed class OtherRule : Rule
        {
            private readonly List<List<int>> _others;

            public OtherRule(int id, List<List<int>> others) : base(id)
            {
                _others = others;
            }

            public override bool TryGetRegex(Dictionary<int, string> regexes, out string regex)
            {
                if (_others.SelectMany(o => o).All(regexes.ContainsKey))
                {
                    regex =
                        $"({string.Join("|", _others.Select(o => $"({string.Join("", o.Select(or => regexes[or]))})"))})";
                    return true;
                }

                regex = null;
                return false;
            }
        }
    }
}