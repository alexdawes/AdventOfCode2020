using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._24
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
            var paths = await ParseInput();
            var flipped = new HashSet<(int X, int Y)>();

            foreach (var path in paths)
            {
                var coord = (0, 0);
                foreach (var d in path)
                {
                    coord = coord.Move(d);
                }

                if (flipped.Contains(coord))
                {
                    flipped.Remove(coord);
                }
                else
                {
                    flipped.Add(coord);
                }
            }
            return flipped.Count;
        }

        private async Task<long> Part2()
        {
            var paths = await ParseInput();
            var blackTiles = new HashSet<(int X, int Y)>();

            foreach (var path in paths)
            {
                var coord = (0, 0);
                foreach (var d in path)
                {
                    coord = coord.Move(d);
                }

                if (blackTiles.Contains(coord))
                {
                    blackTiles.Remove(coord);
                }
                else
                {
                    blackTiles.Add(coord);
                }
            }

            for (var i = 0; i < 100; i++)
            {
                var toFlip = new HashSet<(int X, int Y)>();
                var toConsider = blackTiles.SelectMany(f => f.GetNeighbours().Append(f)).ToList();

                foreach (var tile in toConsider)
                {
                    var neighbours = tile.GetNeighbours();
                    var blackNeighbours = neighbours.Where(blackTiles.Contains);
                    var numBlackNeighbours = blackNeighbours.Count();
                    if (blackTiles.Contains(tile))
                    {
                        if (numBlackNeighbours == 0 || numBlackNeighbours > 2)
                        {
                            toFlip.Add(tile);
                        }
                    }
                    else
                    {
                        if (numBlackNeighbours == 2)
                        {
                            toFlip.Add(tile);
                        }
                    }
                }

                foreach (var tile in toFlip)
                {
                    if (blackTiles.Contains(tile))
                    {
                        blackTiles.Remove(tile);
                    }
                    else
                    {
                        blackTiles.Add(tile);
                    }
                }
            }

            return blackTiles.Count;
        }

        public static async Task<List<List<Direction>>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("24/input");
            return text.Split("\r\n").Select(line =>
            {
                var regex = new Regex("(ne|nw|se|sw|e|w)");
                var matches = regex.Matches(line);
                return matches.Select(m => m.Value)
                              .Select(d =>
                              {
                                  switch (d)
                                  {
                                      case "ne":
                                          return Direction.NorthEast;
                                      case "e":
                                          return Direction.East;
                                      case "se":
                                          return Direction.SouthEast;
                                      case "sw":
                                          return Direction.SouthWest;
                                      case "w":
                                          return Direction.West;
                                      case "nw":
                                          return Direction.NorthWest;
                                      default:
                                          throw new Exception($"Unrecognised {nameof(Direction)}: {d}");
                                  }
                              })
                              .ToList();
            }).ToList();
        }
    }

    public static class Extensions
    {
        public static (int X, int Y) Move(this (int X, int Y) c, Direction direction)
        {
            var (x, y) = c;
            switch (direction)
            {
                case Direction.NorthEast:
                    return (x + 1, y - 1);
                case Direction.East:
                    return (x + 2, y);
                case Direction.SouthEast:
                    return (x + 1, y + 1);
                case Direction.SouthWest:
                    return (x - 1, y + 1);
                case Direction.West:
                    return (x - 2, y);
                case Direction.NorthWest:
                    return (x - 1, y - 1);
                default:
                    throw new Exception($"Unrecognised {nameof(Direction)}: {direction}");
            }
        }

        public static List<(int X, int Y)> GetNeighbours(this (int X, int Y) c)
        {
            return Enum.GetValues(typeof(Direction)).Cast<Direction>().Select(d => c.Move(d)).ToList();
        }
    }

    public enum Direction
    {
        NorthEast,
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest
    }
}