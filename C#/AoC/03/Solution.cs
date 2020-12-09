using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._03
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
            var map = await ParseInput();
            var slope = new Slope(3, 1);

            return map.CountTrees(slope);
        }

        private async Task<long> Part2()
        {
            var map = await ParseInput();
            var slopes = new []
            {
                new Slope(1, 1),
                new Slope(3, 1),
                new Slope(5, 1),
                new Slope(7, 1),
                new Slope(1, 2)
            };

            return slopes.Aggregate(1L, (agg, slope) => agg * map.CountTrees(slope));
        }

        private async Task<Map> ParseInput()
        {
            return Map.Parse(await File.ReadAllTextAsync("03/input"));
        }

        public sealed class Map
        {
            public int Height => TypeMap.GetLength(1);

            public int Width => TypeMap.GetLength(0);

            public LocationType[,] TypeMap { get; }

            public Map(LocationType[,] map)
            {
                TypeMap = map;
            }

            public LocationType GetType(Position position)
            {
                return TypeMap[position.X % Width, position.Y];
            }

            public bool IsTree(Position position)
            {
                return GetType(position) == LocationType.Tree;
            }

            public long CountTrees(Slope slope)
            {
                long count = 0;
                var position = new Position(0, 0);
                while (position.Y < Height)
                {
                    if (IsTree(position))
                    {
                        count++;
                    }
                    position += slope;
                }

                return count;
            }

            public static Map Parse(string s)
            {
                var rows = s.Split('\n').Select(line => line.Trim()).ToList();
                var height = rows.Count;
                var width = rows.First().Length;
                var map = new LocationType[width, height];
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var charValue = rows[y][x];
                        LocationType type;
                        switch (charValue)
                        {
                            case '#':
                                type = LocationType.Tree;
                                break;
                            case '.':
                                type = LocationType.Clear;
                                break;
                            default:
                                throw new Exception($"Unrecognised {nameof(LocationType)} {charValue}");
                        }

                        map[x, y] = type;
                    }
                }

                return new Map(map);
            }
        }

        public sealed class Position
        {
            public int X { get; }

            public int Y { get; }

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Position operator +(Position position, Slope slope)
            {
                return new Position(position.X + slope.Right, position.Y + slope.Down);
            }
        }

        public sealed class Slope
        {
            public int Right { get; }

            public int Down { get; }

            public Slope(int right, int down)
            {
                Right = right;
                Down = down;
            }
        }

        public enum LocationType
        {
            Tree,
            Clear,
        }
    }
}