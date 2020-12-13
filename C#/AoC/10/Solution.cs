using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._10
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
            var voltages = await ParseInput();
            var sorted = voltages.Concat(new [] { 0, voltages.Max() + 3 }).OrderBy(v => v).ToList();
            var steps = Enumerable.Range(0, sorted.Count - 1).Select(i => sorted[i + 1] - sorted[i]).ToList();
            var counts = steps.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());
            return counts[1] * counts[3];
        }

        private async Task<long> Part2()
        {
            var voltages = await ParseInput();
            var sorted = voltages.Concat(new[] { voltages.Max() + 3 }).OrderBy(v => v).ToList();
            var snippets = new List<List<long>>();
            var current = new List<long>{ 0 };
            foreach (var v in sorted)
            {
                var last = current.Last();
                if (v - last == 3)
                {
                    snippets.Add(current);
                    current = new List<long>();
                }

                current.Add(v);
            }

            snippets.Add(current);

            return snippets.Aggregate(1L, (agg, snip) => agg * CountPossiblePaths(snip));
        }

        private long CountPossiblePaths(List<long> snippet)
        {
            Stack<List<long>> toProcess = new Stack<List<long>>();
            var start = snippet.Min();
            var end = snippet.Max();

            toProcess.Push(new List<long> { start });
            var count = 0;
            while (toProcess.Any())
            {
                var next = toProcess.Pop();
                var last = next.Max();
                if (last == end)
                {
                    count++;
                    continue;
                }

                var possible = snippet.Where(i => last < i && i <= last + 3);
                foreach (var p in possible)
                {
                    toProcess.Push(next.Append(p).ToList());
                }
            }

            return count;
        }

        private async Task<List<long>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("10/input");
            return text
                   .Split("\r\n")
                   .Select(s => Convert.ToInt64(s.Trim()))
                   .ToList();
        }
    }
}