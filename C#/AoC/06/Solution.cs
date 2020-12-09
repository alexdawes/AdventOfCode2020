using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace AoC._06
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
            var groups = await ParseInput();
            return groups.Sum(g => g.AnyTotal);
        }

        private async Task<long> Part2()
        {
            var groups = await ParseInput();
            return groups.Sum(g => g.AllTotal);
        }

        private async Task<List<GroupAnswer>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("06/input");
            return text
                   .Split("\r\n\r\n")
                   .Select(GroupAnswer.Parse)
                   .ToList();
        }

        public sealed class GroupAnswer
        {
            public IEnumerable<Answer> Answers { get; }

            public GroupAnswer(IEnumerable<Answer> answers)
            {
                Answers = answers;
            }

            public int AnyTotal =>
                Answers.SelectMany(a => a.Answers).Distinct().Count();

            public int AllTotal =>
                Answers.Aggregate(
                    (IEnumerable<char>)"abcdefghijklmnopqrstuvwxyz", 
                    (agg, ans) => agg.Intersect(ans.Answers)).Count();

            public static GroupAnswer Parse(string s)
            {
                return new GroupAnswer(s.Trim().Split("\r\n").Select(Answer.Parse));
            }
        }

        public sealed class Answer
        {
            public IEnumerable<char> Answers { get; }

            public Answer(IEnumerable<char> answers)
            {
                Answers = answers;
            }

            public static Answer Parse(string s) => new Answer(s.Trim());
        }
    }
}