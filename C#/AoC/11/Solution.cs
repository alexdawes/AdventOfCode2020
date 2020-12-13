using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC._11
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
            var seatingPlan = await ParseInput();
            var adjacent =
                seatingPlan.Seats.Keys.ToDictionary(
                    s => s, s => seatingPlan.GetAdjacentSeats(s).Select(p => p.Position).ToList());

            var strings = new HashSet<string>(new[] {seatingPlan.ToString()});
            while (true)
            {
                seatingPlan = seatingPlan.Next(adjacent, 4);
                var str = seatingPlan.ToString();
                if (strings.Contains(str))
                {
                    return seatingPlan.Seats.Values.Count(s => s.Occupied);
                }

                strings.Add(str);
            }
        }

        private async Task<int> Part2()
        {
            var seatingPlan = await ParseInput();

            var adjacent =
                seatingPlan.Seats.Keys.ToDictionary(
                    s => s, s => seatingPlan.GetSeatsInLineOfSight(s).Select(p => p.Position).ToList());

            var strings = new HashSet<string>(new[] { seatingPlan.ToString() });
            while (true)
            {
                seatingPlan = seatingPlan.Next(adjacent, 5);
                var str = seatingPlan.ToString();
                if (strings.Contains(str))
                {
                    return seatingPlan.Seats.Values.Count(s => s.Occupied);
                }

                strings.Add(str);
            }
        }


        private async Task<SeatingPlan> ParseInput()
        {
            var text = await File.ReadAllTextAsync("11/input");
            return SeatingPlan.Parse(text);
        }
    }

    public sealed class SeatingPlan
    {
        public IReadOnlyDictionary<(int X, int Y), Seat> Seats { get; }

        public SeatingPlan(IReadOnlyCollection<Seat> seats)
        {
            Seats = seats.ToDictionary(s => (s.Position.X, s.Position.Y));
        }

        public Seat this[(int X, int Y) c] => Seats.ContainsKey((c.X, c.Y)) ? Seats[(c.X, c.Y)] : default;

        public List<Seat> GetAdjacentSeats((int X, int Y) pos)
        {
            var seats = new List<Seat>();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        var s = this[(pos.X + i, pos.Y + j)];
                        if (s != default)
                        {
                            seats.Add(s);
                        }
                    }
                }
            }

            return seats;
        }

        public List<Seat> GetSeatsInLineOfSight((int X, int Y) pos)
        {
            var minX = Seats.Values.Min(s => s.Position.X);
            var maxX = Seats.Values.Max(s => s.Position.X);
            var minY = Seats.Values.Min(s => s.Position.Y);
            var maxY = Seats.Values.Max(s => s.Position.Y);

            var seats = new List<Seat>();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        var x = pos.X;
                        var y = pos.Y;
                        var s = default(Seat);
                        while (minX <= x && x <= maxX && minY <= y && y <= maxY && s == default)
                        {
                            x += i;
                            y += j;
                            s = this[(x, y)];
                        }

                        if (s != default)
                        {
                            seats.Add(s);
                        }
                    }
                }
            }

            return seats;
        }

        public static SeatingPlan Parse(string s)
        {
            var seats = new List<Seat>();
            var lines = s.Split("\r\n").Select(l => l.Trim()).ToList();
            for (var j = 0; j < lines.Count; j++)
            {
                var line = lines[j];
                for (var i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    switch (c)
                    {
                        case 'L':
                            seats.Add(new Seat((i, j), false));
                            break;
                        case '#':
                            seats.Add(new Seat((i, j), false));
                            break;
                        default:
                            break;
                    }
                }
            }

            return new SeatingPlan(seats);
        }

        public SeatingPlan Next(Dictionary<(int X, int Y), List<(int X, int Y)>> adjacentDict, int threshold)
        {
            return new SeatingPlan(Seats.Values.Select(seat =>
            {
                var adjacent = adjacentDict[seat.Position];
                if (!seat.Occupied && !adjacent.Any(s => this[s].Occupied))
                {
                    return new Seat(seat.Position, true);
                }

                if (seat.Occupied && adjacent.Count(s => this[s].Occupied) >= threshold)
                {
                    return new Seat(seat.Position, false);
                }

                return new Seat(seat.Position, seat.Occupied);
            }).ToList());
        }

        public override string ToString()
        {
            var minX = Seats.Values.Min(s => s.Position.X);
            var maxX = Seats.Values.Max(s => s.Position.X);
            var minY = Seats.Values.Min(s => s.Position.Y);
            var maxY = Seats.Values.Max(s => s.Position.Y);

            var stringBuilder = new StringBuilder();
            for (var j = minY; j <= maxY; j++)
            {
                for (var i = minX; i <= maxX; i++)
                {
                    var seat = this[(i, j)];
                    if (seat == default)
                    {
                        stringBuilder.Append(".");
                    }
                    else if (seat.Occupied)
                    {
                        stringBuilder.Append("#");
                    }
                    else
                    {
                        stringBuilder.Append("L");
                    }
                }

                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }

    public sealed class Seat
    {
        public (int X, int Y) Position { get; }

        public bool Occupied { get; }

        public Seat((int X, int Y) position, bool occupied)
        {
            Position = position;
            Occupied = occupied;
        }
    }
}