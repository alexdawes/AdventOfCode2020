using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace AoC._04
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
            var passports = await ParseInput();
            return passports.Count(p => p.IsValid(false));
        }

        private async Task<long> Part2()
        {
            var passports = await ParseInput();
            return passports.Count(p => p.IsValid(true));
        }

        private async Task<List<Passport>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("04/input");
            return text
                   .Split("\r\n\r\n")
                   .Select(Passport.Parse)
                   .ToList();
        }
    }

    public sealed class Passport
    {
        public IDictionary<Field, FieldValue> Fields { get; }

        public Passport(IDictionary<Field, FieldValue> fields)
        {
            Fields = fields;
        }

        public bool IsValid(bool validateValues)
        {
            var requiredFields = Enum.GetValues(typeof(Field)).Cast<Field>().Except(new[] {Field.cid});
            var valid = requiredFields.All(f => Fields.ContainsKey(f));
            return valid && (!validateValues || Fields.Values.All(f => f.IsValid()));
        }

        public static Passport Parse(string line)
        {
            return new Passport(line.Replace("\r\n", " ")
                                    .Split(' ')
                                    .Where(l => !string.IsNullOrEmpty(l))
                                    .Select(item =>
                                    {
                                        var trimmed = item.Trim();
                                        var split = trimmed.Split(':');
                                        var field = Enum.Parse<Field>(split[0]);
                                        string value = string.Join(":", split.Skip(1));
                                                                return (field, value);
                                    })
                                    .ToDictionary(
                                        kvp => kvp.field, 
                                        kvp => FieldValue.Parse(kvp.field, kvp.value)));
        }
    }

    public enum Field
    {
        byr,
        iyr,
        eyr,
        hgt,
        hcl,
        ecl,
        pid,
        cid
    }

    public abstract class FieldValue
    {
        public abstract bool IsValid();

        public static FieldValue Parse(Field field, string value)
        {
            switch (field)
            {
                case Field.byr: return new ByrFieldValue(value);
                case Field.iyr: return new IyrFieldValue(value);
                case Field.eyr: return new EyrFieldValue(value);
                case Field.hgt: return new HgtFieldValue(value);
                case Field.hcl: return new HclFieldValue(value);
                case Field.ecl: return new EclFieldValue(value);
                case Field.pid: return new PidFieldValue(value);
                case Field.cid: return new CidFieldValue(value);
                default: throw new Exception($"{nameof(Field)} {field} not recognised.");
            }
        }
    }

    public sealed class ByrFieldValue : FieldValue
    {
        private readonly string _value;

        public ByrFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^\\d{4}$");
            if (!regex.IsMatch(_value))
            {
                return false;
            }

            var val = Convert.ToInt32(_value);
            return 1920 <= val && val <= 2002;
        }
    }

    public sealed class IyrFieldValue : FieldValue
    {
        private readonly string _value;

        public IyrFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^\\d{4}$");
            if (!regex.IsMatch(_value))
            {
                return false;
            }

            var val = Convert.ToInt32(_value);
            return 2010 <= val && val <= 2020;
        }
    }

    public sealed class EyrFieldValue : FieldValue
    {
        private readonly string _value;

        public EyrFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^\\d{4}$");
            if (!regex.IsMatch(_value))
            {
                return false;
            }

            var val = Convert.ToInt32(_value);
            return 2020 <= val && val <= 2030;
        }
    }

    public sealed class HgtFieldValue : FieldValue
    {
        private readonly string _value;

        public HgtFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^(?<number>\\d+)(?<unit>(cm|in))$");
            var match = regex.Match(_value);
            if (match.Success)
            {
                var val = Convert.ToInt32(match.Groups["number"].Value);
                if (match.Groups["unit"].Value == "cm")
                {
                    return 150 <= val && val <= 193;
                }

                if (match.Groups["unit"].Value == "in")
                {
                    return 59 <= val && val <= 76;
                }

                return false;
            }

            return false;
        }
    }

    public sealed class HclFieldValue : FieldValue
    {
        private readonly string _value;

        public HclFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^#[0-9a-f]{6}$");
            return regex.IsMatch(_value);
        }
    }

    public sealed class EclFieldValue : FieldValue
    {
        private readonly string _value;

        public EclFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^(amb|blu|brn|gry|grn|hzl|oth)$");
            return regex.IsMatch(_value);
        }
    }

    public sealed class PidFieldValue : FieldValue
    {
        private readonly string _value;

        public PidFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            var regex = new Regex("^\\d{9}$");
            return regex.IsMatch(_value);
        }
    }

    public sealed class CidFieldValue : FieldValue
    {
        private readonly string _value;

        public CidFieldValue(string value)
        {
            _value = value;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}