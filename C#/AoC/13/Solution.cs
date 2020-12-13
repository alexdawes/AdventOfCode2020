using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._13
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
            var (time, buses) = await ParseInput();
            var waitTimes = buses.Select(b => (Period: b.Period, WaitTime: b.Period - (time % b.Period)));
            var shortest = waitTimes.OrderBy(b => b.WaitTime).First();
            return shortest.Period * shortest.WaitTime;
        }

        private async Task<long> Part2()
        {
            var (_, buses) = await ParseInput();

            var result = 0L;
            long increment = 1L;
            foreach (var (period, offset) in buses.OrderByDescending(b => b.Period))
            {
                while (true)
                {
                    result += increment;
                    if ((result + offset) % period == 0)
                    {
                        increment *= period;
                        break;
                    }
                }
            }

            return result;
        }

        private async Task<(long Time, (long Period, int Index)[] Buses)> ParseInput()
        {
            var text = await File.ReadAllTextAsync("13/input");
            var lines = text.Split('\n');
            var time = Convert.ToInt64(lines[0].Trim());
            var buses = lines[1].Trim().Split(',').Select(s => s == "x" ? (long?)null : Convert.ToInt64(s)).ToArray();
            var inService =
                buses.Select((b, idx) => (Period: b, Index: idx))
                     .Where(b => b.Period != null)
                     .Select(b => (Period: b.Period.Value, b.Index))
                     .ToArray();
            return (time, inService);
        }
    }
}