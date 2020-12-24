using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AoC._22
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
            var (deck1, deck2) = await ParseInput();

            var winner = PlayCombat(deck1, deck2);

            var winnerDeck = winner == 1 ? deck1 : deck2;

            return winnerDeck.Select((card, idx) => card * (winnerDeck.Count - idx)).Sum();
        }

        private async Task<long> Part2()
        {
            var (deck1, deck2) = await ParseInput();

            var winner = PlayRecursiveCombat(deck1, deck2);

            var winnerDeck = winner == 1 ? deck1 : deck2;

            return winnerDeck.Select((card, idx) => card * (winnerDeck.Count - idx)).Sum();
        }

        public static async Task<(List<int> Deck1, List<int> Deck2)> ParseInput()
        {
            var text = await File.ReadAllTextAsync("22/input");

            var split = text.Split("\r\n\r\n");
            var player1 = split[0].Split("\r\n").Skip(1).Select(l => Convert.ToInt32(l.Trim())).ToList();
            var player2 = split[1].Split("\r\n").Skip(1).Select(l => Convert.ToInt32(l.Trim())).ToList();

            return (player1, player2);
        }

        private int PlayRecursiveCombat(List<int> deck1, List<int> deck2)
        {
            var seen = new HashSet<string>();
            while (deck1.Any() && deck2.Any())
            {
                PlayRecursiveCombatRound(deck1, deck2, seen);
            }

            return deck1.Any() ? 1 : 2;
        }

        private void PlayRecursiveCombatRound(List<int> deck1, List<int> deck2, HashSet<string> seen)
        {
            var key = GetKey(deck1, deck2);
            if (seen.Contains(key))
            {
                deck1.AddRange(deck2);
                deck2.RemoveAll(i => true);
                return;
            }

            seen.Add(key);

            var deck1Top = deck1[0];
            deck1.RemoveAt(0);
            var deck2Top = deck2[0];
            deck2.RemoveAt(0);

            if (deck1.Count >= deck1Top && deck2.Count >= deck2Top)
            {
                var subDeck1 = deck1.Take(deck1Top).ToList();
                var subDeck2 = deck2.Take(deck2Top).ToList();
                var result = PlayRecursiveCombat(subDeck1, subDeck2);

                var winningDeck = result == 1 ? deck1 : deck2;
                var winningCard = result == 1 ? deck1Top : deck2Top;
                var losingCard = result == 1 ? deck2Top : deck1Top;
                winningDeck.AddRange(new[] { winningCard, losingCard });
            }
            else
            {
                var winningDeck = deck1Top > deck2Top ? deck1 : deck2;
                var winningCard = Math.Max(deck1Top, deck2Top);
                var losingCard = Math.Min(deck1Top, deck2Top);
                winningDeck.AddRange(new[] { winningCard, losingCard });
            }
        }

        private string GetKey(List<int> deck1, List<int> deck2)
        {
            return $"{string.Join(",", deck1)}|{string.Join(",", deck2)}";
        }

        private int PlayCombat(List<int> deck1, List<int> deck2)
        {
            while (deck1.Any() && deck2.Any())
            {
                PlayCombatRound(deck1, deck2);
            }

            return deck1.Any() ? 1 : 2;
        }

        private void PlayCombatRound(List<int> deck1, List<int> deck2)
        {
            var deck1Top = deck1[0];
            deck1.RemoveAt(0);
            var deck2Top = deck2[0];
            deck2.RemoveAt(0);

            var winningDeck = deck1Top > deck2Top ? deck1 : deck2;
            var winningCard = Math.Max(deck1Top, deck2Top);
            var losingCard = Math.Min(deck1Top, deck2Top);

            winningDeck.AddRange(new[] { winningCard, losingCard });
        }
    }
}