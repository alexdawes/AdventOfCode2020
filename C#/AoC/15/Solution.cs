using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._15
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
            return await Solve(2020);
        }

        private async Task<long> Part2()
        {
            return await Solve(30000000);
        }

        private async Task<long> Solve(int number)
        {
            var initial = await ParseInput();
            var memory = new Dictionary<int, List<int>>();
            var turnCount = 0;
            var lastSaid = -1;

            while (true)
            {
                turnCount++;

                int nextSaid;
                if (turnCount - 1 < initial.Count)
                {
                    nextSaid = initial[turnCount - 1];
                }
                else
                {
                    var previous = memory[lastSaid];
                    nextSaid = previous.Count >= 2 ? previous[previous.Count - 1] - previous[previous.Count - 2] : 0;
                }

                if (!memory.ContainsKey(nextSaid))
                {
                    memory[nextSaid] = new List<int>();
                }

                memory[nextSaid].Add(turnCount);

                lastSaid = nextSaid;

                if (turnCount == number)
                {
                    return nextSaid;
                }
            }
        }

        private async Task<List<int>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("15/input");
            return text.Split(',').Select(i => Convert.ToInt32(i.Trim())).ToList();
        }
    }
}