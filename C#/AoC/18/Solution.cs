using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._18
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
            var equations = await ParseInput();
            return equations.Sum(Solve);
        }

        private async Task<long> Part2()
        {
            var equations = await ParseInput();
            return equations.Sum(Solve2);
        }
        
        private long Solve2(string equation)
        {
            var numberRegex = new Regex("^\\d+$");
            var current = equation;
            while (!numberRegex.IsMatch(current))
            {
                var bracketRegex = new Regex("\\((?<equation>[\\d\\+\\*\\s]+)\\)");
                var bracketMatch = bracketRegex.Match(current);
                if (bracketMatch.Success)
                {
                    current = current.Replace(bracketMatch.Value, Solve2(bracketMatch.Groups["equation"].Value).ToString());
                    continue;
                }

                var addRegex = new Regex("(?<start>(^|\\s))(?<left>\\d+)\\s\\+\\s(?<right>\\d+)(?<end>($|\\s))");
                var addMatch = addRegex.Match(current);
                if (addMatch.Success)
                {
                    current = current.Replace(addMatch.Value,
                                              $"{addMatch.Groups["start"].Value}{Convert.ToInt64(addMatch.Groups["left"].Value) + Convert.ToInt64(addMatch.Groups["right"].Value)}{addMatch.Groups["end"].Value}");
                    continue;
                }

                var multiplyRegex = new Regex("(?<start>(^|\\s))(?<left>\\d+)\\s\\*\\s(?<right>\\d+)(?<end>($|\\s))");
                var multiplyMatch = multiplyRegex.Match(current);
                if (multiplyMatch.Success)
                {
                    current = current.Replace(multiplyMatch.Value,
                                              $"{multiplyMatch.Groups["start"].Value}{Convert.ToInt64(multiplyMatch.Groups["left"].Value) * Convert.ToInt64(multiplyMatch.Groups["right"].Value)}{multiplyMatch.Groups["end"].Value}");
                    continue;
                }
            }

            return Convert.ToInt64(current);
        }

        private long Solve(string equation)
        {
            return Solve(equation.Replace("(", "( ").Replace(")", " )").Split(" ").ToArray());
        }

        private long Solve(string[] equation)
        {
            long counter = 0;
            long value = 0;
            string currentOperation = "+";

            while (counter < equation.Length)
            {
                var current = equation[counter];

                if (int.TryParse(current, out int currentInt))
                {
                    switch (currentOperation)
                    {
                        case "+":
                            value += currentInt;
                            break;
                        case "*":
                            value *= currentInt;
                            break;
                        default:
                            break;
                    }

                    counter++;
                    continue;
                }

                if (current == "*")
                {
                    currentOperation = "*";
                    counter++;
                    continue;
                }

                if (current == "+")
                {
                    currentOperation = "+";
                    counter++;
                    continue;
                }

                if (current == "(")
                {
                    var section = new List<string>();
                    var braceCount = 1;
                    while (true)
                    {
                        counter++;
                        if (equation[counter] == "(")
                        {
                            braceCount++;
                        }

                        if (equation[counter] == ")")
                        {
                            braceCount--;
                        }

                        if (braceCount == 0)
                        {
                            break;
                        }

                        section.Add(equation[counter]);
                    }

                    var sectionValue = Solve(section.ToArray());

                    switch (currentOperation)
                    {
                        case "+":
                            value += sectionValue;
                            break;
                        case "*":
                            value *= sectionValue;
                            break;
                        default:
                            break;
                    }

                    counter++;
                    continue;
                }

            }

            return value;
        }

        public static async Task<List<string>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("18/input");
            return text.Split("\r\n").Select(s => s.Trim())
                       .ToList();
        }
    }
}