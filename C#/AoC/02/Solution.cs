using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._02
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


        private async Task<int> Part1()
        {
            var records = await ParseInput();
            return records.Count(r => r.IsValid(PolicyType.OldPolicy));
        }

        private async Task<int> Part2()
        {
            var records = await ParseInput();
            return records.Count(r => r.IsValid(PolicyType.NewPolicy));
        }

        private async Task<List<Record>> ParseInput()
        {
            return (await File.ReadAllTextAsync("02/input"))
                   .Split('\n')
                   .Select(Record.Parse)
                   .ToList();
        }

        public sealed class Record
        {
            public string Password { get; }

            public Policy Policy { get; }

            public Record(string password, Policy policy)
            {
                Password = password;
                Policy = policy;
            }

            public bool IsValid(PolicyType policyType) => Policy.IsValid(Password, policyType);

            public static Record Parse(string line)
            {
                var regex = new Regex("^(?<low>\\d+)\\-(?<high>\\d+) (?<char>[a-z]): (?<password>.*)$");
                var match = regex.Match(line);
                if (!match.Success)
                {
                    throw new InvalidOperationException($"{line} is not a valid {nameof(Record)}");
                }

                var low = Convert.ToInt32(match.Groups["low"].Value);
                var high = Convert.ToInt32(match.Groups["high"].Value);
                var character = Convert.ToChar(match.Groups["char"].Value);
                var password = match.Groups["password"].Value;

                return new Record(password, new Policy(low, high, character));
            }
        }

        public sealed class Policy
        {
            public int Low { get; }

            public int High { get; }

            public char Character { get; }

            public Policy(int low, int high, char character)
            {
                Low = low;
                High = high;
                Character = character;
            }

            public bool IsValid(string password, PolicyType policyType)
            {
                switch (policyType)
                {
                    case PolicyType.OldPolicy:
                        var count = password.Count(c => c == Character);
                        return Low <= count && count <= High;
                    case PolicyType.NewPolicy:
                        var first = password.Length < Low ? (char?)null : password[Low - 1];
                        var second = password.Length < High ? (char?)null : password[High - 1];
                        return first == Character ^ second == Character;
                    default:
                        throw new ArgumentException($"{nameof(PolicyType)} {policyType} is not recognised.");
                }
            }
        }

        public enum PolicyType
        {
            OldPolicy,
            NewPolicy,
        }
    }
}