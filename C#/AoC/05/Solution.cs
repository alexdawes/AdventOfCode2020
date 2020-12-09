using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._05
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
            var seatIds = (await ParseInput()).Select(s => s.SeatId).ToList();
            return seatIds.Max();
        }

        private async Task<long> Part2()
        {
            var seatIds = (await ParseInput()).Select(s => s.SeatId).ToList();
            return Enumerable.Range(seatIds.Min(), seatIds.Max() - seatIds.Min()).Except(seatIds).First();
        }

        private async Task<List<Seat>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("05/input");
            return text
                   .Split("\r\n")
                   .Select(Seat.Parse)
                   .ToList();
        }

        public sealed class Seat
        {
            public int Row { get; }

            public int Column { get; }

            public Seat(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int SeatId => Row * 8 + Column;

            public static Seat Parse(string s)
            {
                var rowStr = s.Substring(0, 7);
                var columnStr = s.Substring(7, 3);
                var row = rowStr.Aggregate(0, (agg, c) => agg * 2 + (c == 'B' ? 1 : 0));
                var column = columnStr.Aggregate(0, (agg, c) => agg * 2 + (c == 'R' ? 1 : 0));
                return new Seat(row, column);
            }
        }
    }
}