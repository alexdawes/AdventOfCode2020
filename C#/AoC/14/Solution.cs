using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._14
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
            var instructions = await ParseInput();
            var state = new State();
            foreach (var inst in instructions)
            {
                inst.Invoke1(state);
            }
            return state.Values.Values.Sum(v => (long)v);
        }

        private async Task<long> Part2()
        {
            var instructions = await ParseInput();
            var state = new State();
            foreach (var inst in instructions)
            {
                inst.Invoke2(state);
            }
            return state.Values.Values.Sum(v => (long)v);
        }

        private async Task<List<Instruction>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("14/input");
            return text.Split("\r\n").Select(line => Instruction.Parse(line.Trim())).ToList();
        }
    }

    public sealed class State
    {
        public Dictionary<BitString, BitString> Values { get; }

        public Mask? Mask { get; private set; }

        public State(Dictionary<BitString, BitString> values = null, Mask? mask = null)
        {
            Values = values ?? new Dictionary<BitString, BitString>();
            Mask = mask;
        }

        public void SetMask(Mask mask)
        {
            Mask = mask;
        }

        public void SetMemoryValue(BitString key, BitString value)
        {
            Values[key] = value;
        }
    }

    public abstract class Instruction
    {
        public abstract void Invoke1(State state);
        public abstract void Invoke2(State state);

        public static Instruction Parse(string line)
        {
            var maskRegex = new Regex("^mask = (?<mask>(X|0|1){36})$");
            var memoryRegex = new Regex("^mem\\[(?<address>\\d+)\\] = (?<value>\\d+)$");
            if (maskRegex.IsMatch(line))
            {
                var match = maskRegex.Match(line);
                return new MaskInstruction(new Mask(match.Groups["mask"].Value));
            }

            if (memoryRegex.IsMatch(line))
            {
                var match = memoryRegex.Match(line);
                return new MemoryInstruction(Convert.ToInt64(match.Groups["address"].Value), Convert.ToInt64(match.Groups["value"].Value));
            }

            throw new Exception($"Unrecognised {nameof(Instruction)}: {line}");
        }
    }

    public sealed class MaskInstruction : Instruction
    {
        private readonly Mask _mask;

        public MaskInstruction(Mask mask)
        {
            _mask = mask;
        }

        public override void Invoke1(State state)
        {
            state.SetMask(_mask);
        }
        public override void Invoke2(State state)
        {
            state.SetMask(_mask);
        }
    }

    public sealed class MemoryInstruction : Instruction
    {
        private readonly BitString _address;
        private readonly BitString _value;

        public MemoryInstruction(BitString address, BitString value)
        {
            _address = address;
            _value = value;
        }

        public override void Invoke1(State state)
        {
            state.SetMemoryValue(_address, state.Mask?.MaskValue(_value) ?? _value);
        }
        public override void Invoke2(State state)
        {
            if (!state.Mask.HasValue)
            {
                state.SetMemoryValue(_address, _value);
            }
            else
            {
                var addresses = state.Mask.Value.MaskAddress(_address);
                foreach (var addr in addresses)
                {
                    state.SetMemoryValue(addr, _value);
                }
            }
        }
    }

    public struct Mask
    {
        private readonly string _value;

        public Mask(string value)
        {
            _value = value;
        }

        public BitString MaskValue(BitString bitString)
        {
            char[] result = ((string)bitString).ToCharArray();
            for (var i = 0; i < Math.Min(result.Length, _value.Length); i++)
            {
                var maskChar = _value[_value.Length - 1 - i];
                if (maskChar == 'X')
                {
                    continue;
                }

                result[result.Length - 1 - i] = maskChar;
            }

            return (BitString)string.Join(string.Empty, result);
        }

        public List<BitString> MaskAddress(BitString bitString)
        {
            List<char[]> results = new List<char[]> { ((string)bitString).ToCharArray() };

            for (var i = 0; i < _value.Length; i++)
            {
                var maskChar = _value[_value.Length - 1 - i];
                if (maskChar == '0')
                {
                    continue;
                }

                if (maskChar == '1')
                {
                    foreach (var result in results)
                    {
                        result[result.Length - 1 - i] = '1';
                    }

                    continue;
                }

                if (maskChar == 'X')
                {
                    results = results.Aggregate(new List<char[]>(), (agg, r) =>
                    {
                        agg.AddRange(new[] {'0', '1'}.Select(v =>
                        {
                            var res = r.ToArray();
                            res[res.Length - 1 - i] = v;
                            return res;
                        }));
                        return agg;
                    });
                }
            }

            return results.Select(r => new BitString(string.Join("", r))).ToList();
        }
    }

    public struct BitString : IEquatable<BitString>
    {
        private readonly string _value;

        public BitString(string value)
        {
            if (value.Length > 36)
            {
                _value = value.Substring(value.Length - 36);
            }
            else if (value.Length < 36)
            {
                _value = string.Join("", Enumerable.Range(0, 36 - value.Length).Select((i) => "0")) + value;
            }
            else
            {
                _value = value;
            }
        }

        public BitString(long value) : this(Convert.ToString(value, 2))
        {
        }

        public static implicit operator string(BitString bs) => bs._value;
        public static implicit operator BitString(string s) => new BitString(s);
        public static implicit operator long(BitString bs) => Convert.ToInt64(bs._value, 2);
        public static implicit operator BitString(long l) => new BitString(l);

        public override string ToString() => _value;

        public bool Equals(BitString other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is BitString other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }
    }
}