using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AoC.Utils.Computation;

namespace AoC._08
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
            var computer = new Computer(0, 0);
            var program = await ParseInput();
            computer.Run(program);
            return computer.Accumulator;
        }

        private async Task<long> Part2()
        {
            var program = await ParseInput();
            for (var i = 0; i < program.Count; i++)
            {
                var newProgram = program.ToList();
                if (newProgram[i] is JmpInstruction jmpInstruction)
                {
                    newProgram[i] = new NopInstruction(jmpInstruction.Value);
                }
                else if (newProgram[i] is NopInstruction nopInstruction)
                {
                    newProgram[i] = new JmpInstruction(nopInstruction.Value);
                }
                else
                {
                    continue;
                }
                var computer = new Computer(0, 0);
                var returnCode = computer.Run(newProgram);
                if (returnCode == ReturnCode.Completed)
                {
                    return computer.Accumulator;
                }
            }

            throw new Exception("Correct program not found.");
        }

        private async Task<List<Instruction>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("08/input");
            return text
                   .Split("\r\n")
                   .Select(Instruction.Parse)
                   .ToList();
        }

    }
}