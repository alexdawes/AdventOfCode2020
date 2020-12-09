using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._01
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
            var report = await ParseInput();
            for (var i = 0; i < report.Count; i++)
            {
                var rI = report[i];
                for (var j = i; j < report.Count; j++)
                {
                    var rJ = report[j];
                    if (rI + rJ == 2020)
                    {
                        return rI * rJ;
                    }
                }
            }

            return 0;
        }

        private async Task<int> Part2()
        {
            var report = await ParseInput();
            for (var i = 0; i < report.Count; i++)
            {
                var rI = report[i];
                for (var j = i; j < report.Count; j++)
                {
                    var rJ = report[j];
                    for (var k = j; k < report.Count; k++)
                    {
                        var rK  = report[k];
                        if (rI + rJ + rK == 2020)
                        {
                            return rI * rJ * rK;
                        }
                    }
                }
            }

            return 0;
        }

        private async Task<List<int>> ParseInput()
        {
            return (await File.ReadAllTextAsync("01/input"))
                   .Split('\n')
                   .Select(int.Parse)
                   .ToList();
        }
    }
}
