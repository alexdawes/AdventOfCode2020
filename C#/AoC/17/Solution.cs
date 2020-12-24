using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._17
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
            var space = await ParseInput(3);
            Console.WriteLine(space);
            Console.WriteLine();
            for (var i = 0; i < 6; i++)
            {
                space = space.Iterate();
                Console.WriteLine(space);
                Console.WriteLine();
            }

            return space.CountActive();
        }

        private async Task<long> Part2()
        {
            var space = await ParseInput(4);
            for (var i = 0; i < 6; i++)
            {
                space = space.Iterate();
                // Console.WriteLine(space);
            }

            return space.CountActive();
        }

        public static async Task<Space> ParseInput(int dimensions)
        {
            var text = await File.ReadAllTextAsync("17/test_input");
            var lines = text.Split("\r\n").Select(l => l.Trim()).ToArray();
            HashSet<Coordinate> active = new HashSet<Coordinate>();
            for (var j = 0; j < lines.Length; j++)
            {
                var line = lines[j];
                for (var i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    if (c == '#')
                    {
                        var list = new List<int> {i, j};
                        foreach (var k in Enumerable.Range(0, dimensions - 2))
                        {
                            list.Add(0);
                        }
                        active.Add(list.ToArray());
                    }
                }
            }

            return new Space(active);
        }

        public sealed class Space
        {
            private readonly HashSet<Coordinate> _activeStates;

            public Space (HashSet<Coordinate> activeStates)
            {
                _activeStates = activeStates;
            }

            public void Activate(Coordinate coord)
            {
                _activeStates.Add(coord);
            }

            public void Deactivate(Coordinate coord)
            {
                _activeStates.Remove(coord);
            }

            public State Test(Coordinate coord)
            {
                return _activeStates.Contains(coord) ? State.Active : State.Inactive;
            }

            public Space Iterate()
            {
                var toConsider = _activeStates.SelectMany(c => c.GetNeighbours()).Distinct();
                var newStates = toConsider.ToDictionary(c => c, c =>
                {
                    var currentState = Test(c);
                    var neighbours = c.GetNeighbours();
                    var numActiveNeighbours = neighbours.Count(n => Test(n) == State.Active);
                    switch (currentState)
                    {
                        case State.Active:
                            if (numActiveNeighbours == 2 || numActiveNeighbours == 3)
                            {
                                return State.Active;
                            }
                            else
                            {
                                return State.Inactive;
                            }
                        case State.Inactive:
                            if (numActiveNeighbours == 3)
                            {
                                return State.Active;
                            }
                            else
                            {
                                return State.Inactive;
                            }
                        default:
                            throw new Exception($"Unrecognised {nameof(State)}: {currentState}");
                    }
                });
                return new Space(new HashSet<Coordinate>(newStates.Where(kvp => kvp.Value == State.Active).Select(kvp => kvp.Key)));
            }

            public int CountActive()
            {
                return _activeStates.Count;
            }

            public override string ToString()
            {
                return string.Join(", ", _activeStates.Select(a => $"[{string.Join(",", (int[])a)}]"));
            }

        }


        public enum State
        {
            Active,
            Inactive,
        }

        public sealed class Coordinate : IEquatable<Coordinate>
        {
            private readonly int[] _values;

            public Coordinate(int[] values)
            {
                _values = values;
            }
            
            public static implicit operator int[] (Coordinate c)
            {
                return c._values;
            }

            public static implicit operator Coordinate(int[] c)
            {
                return new Coordinate(c);
            }

            public static Coordinate operator +(Coordinate coord, int[] vector)
            {
                var newValues = new int[coord._values.Length];
                for (var i = 0; i < coord._values.Length; i++)
                {
                    newValues[i] = coord._values[i] + vector[i];
                }

                return new Coordinate(newValues);
            }

            public IEnumerable<Coordinate> GetNeighbours()
            {
                var dimensions = _values.Length;

                return _values.Aggregate(new List<List<int>> {new List<int>()},
                                  (agg, v) => agg.SelectMany(i => new[]
                                  {
                                      i.Append(v - 1).ToList(), i.Append(v).ToList(),
                                      i.Append(v + 1).ToList()
                                  }).ToList())
                       .Select(c => new Coordinate(c.ToArray()))
                       .Where(c => !c.Equals(this))
                       .ToList();
            }

            public bool Equals(Coordinate other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return _values.Length == other._values.Length
                       && Enumerable.Range(0, _values.Length).All(i => _values[i] == other._values[i]);
            }

            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || obj is Coordinate other && Equals(other);
            }

            public override int GetHashCode()
            {
                var hashCode = 1;
                foreach (var val in _values)
                {
                    hashCode = HashCode.Combine(hashCode, val);
                }

                return hashCode;
            }
        }
    }
}