using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC._23
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

        private async Task<string> Part1()
        {
            var cups = new LinkedList<int>(await ParseInput());
            Play(cups, 100);
            var cupOne = cups.Find(1);
            var next = cupOne.NextOrFirst();

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < cups.Count - 1; i++)
            {
                stringBuilder.Append(next.Value);
                next = next.NextOrFirst();
            }

            return stringBuilder.ToString();
        }

        private async Task<long> Part2()
        {
            var input = await ParseInput();
            var cups = new LinkedList<int>(input.Concat(Enumerable.Range(input.Max() + 1, 1000000 - input.Count)));

            Play(cups, 10000000);

            var one = cups.Find(1);
            return (long)one.NextOrFirst().Value * (long)one.NextOrFirst().NextOrFirst().Value;
        }

        private void Play(LinkedList<int> cups, int iterations)
        {
            var index = new Dictionary<int, LinkedListNode<int>>();
            var indexer = cups.First;
            while (indexer != null)
            {
                index.Add(indexer.Value, indexer);
                indexer = indexer.Next;
            }

            var maxCup = index.Keys.Max();

            var currentCup = cups.First;
            for (var i = 0; i < iterations; i++)
            {
                var pickUp = new List<LinkedListNode<int>>
                {
                    currentCup.NextOrFirst(),
                    currentCup.NextOrFirst().NextOrFirst(),
                    currentCup.NextOrFirst().NextOrFirst().NextOrFirst(),
                };
                foreach (var pickedUp in pickUp)
                {
                    cups.Remove(pickedUp);
                }

                var destinationCupValue = currentCup.Value - 1;
                if (destinationCupValue < 1)
                {
                    destinationCupValue = maxCup;
                }
                while (pickUp.Any(p => p.Value == destinationCupValue) ||
                       destinationCupValue == currentCup.Value)
                {
                    destinationCupValue -= 1;
                    if (destinationCupValue < 1)
                    {
                        destinationCupValue = maxCup;
                    }
                }

                currentCup = currentCup.NextOrFirst();
                var destination = index[destinationCupValue];
                cups.AddAfter(destination, pickUp[2]);
                cups.AddAfter(destination, pickUp[1]);
                cups.AddAfter(destination, pickUp[0]);
            }
        }

        public static async Task<List<int>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("23/input");
            return text.Select(c => Convert.ToInt32(c.ToString())).ToList();
        }
    }

    public static class Extensions
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }
    }
}