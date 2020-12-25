using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._25
{
    public sealed class Solution : ISolution
    {
        public async Task Run()
        {
            var part1 = await Part1();
            Console.WriteLine($"Part 1: {part1}");
        }

        private async Task<long> Part1()
        {
            var (doorPublicKey, cardPublicKey) = await ParseInput();

            var doorLoopSize = 0;
            var cardLoopSize = 0;

            var subject = 7L;
            var doorValue = 1L;
            var cardValue = 1L;

            while (true)
            {
                doorLoopSize++;
                doorValue *= subject;
                doorValue = doorValue % 20201227;
                if (doorValue == doorPublicKey)
                {
                    break;
                }
            }

            while (true)
            {
                cardLoopSize++;
                cardValue *= subject;
                cardValue = cardValue % 20201227;
                if (cardValue == cardPublicKey)
                {
                    break;
                }
            }

            //Console.WriteLine($"DoorLoopSize: {doorLoopSize}");
            //Console.WriteLine($"CardLoopSize: {cardLoopSize}");

            var encryptionKey = 1L;
            for (var i = 0; i < cardLoopSize; i++)
            {
                encryptionKey *= doorValue;
                encryptionKey = encryptionKey % 20201227;
            }

            return encryptionKey;
        }

        public static async Task<(int DoorPublicKey, int CardPublicKey)> ParseInput()
        {
            var text = await File.ReadAllTextAsync("25/input");
            var doorPublicKey = Convert.ToInt32(text.Split("\r\n")[0]);
            var cardPublicKey = Convert.ToInt32(text.Split("\r\n")[1]);
            return (doorPublicKey, cardPublicKey);
        }
    }
}