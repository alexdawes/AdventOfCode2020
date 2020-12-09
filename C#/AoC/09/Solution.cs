using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AoC.Utils.Computation;

namespace AoC._09
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

        private bool IsValid(List<long> values, int pointer, int preambleLength)
        {
            var next = values[pointer + preambleLength];

            for (var i = 0; i < preambleLength - 1; i++)
            {
                var left = values[i + pointer];
                for (var j = i + 1; j < preambleLength; j++)
                {
                    var right = values[j + pointer];
                    var result = left + right;
                    if (result == next)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<long> Part1()
        {
            var values = await ParseInput();
            var pointer = 0;
            var preambleLength = 25;

            while (true)
            {
                if (!IsValid(values, pointer, preambleLength))
                {
                    return values[pointer + preambleLength];
                }

                pointer++;
            }
        }

        private async Task<long> Part2()
        {
            var values = await ParseInput();
            var pointer = 0;
            var key = await Part1();

            while (true)
            {
                var pointer2 = pointer;
                long sum = 0;
                var summed = new List<long>();
                while (sum < key)
                {
                    sum += values[pointer2];
                    summed.Add(values[pointer2]);
                    if (sum == key)
                    {
                        return summed.Min() + summed.Max();
                    }

                    pointer2++;
                }

                pointer++;
            }

        }

        private async Task<List<long>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("09/input");
            return text
                   .Split("\r\n")
                   .Select(s => Convert.ToInt64(s.Trim()))
                   .ToList();
        }
    }
}