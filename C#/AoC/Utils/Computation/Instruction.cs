using System;
using System.Text.RegularExpressions;

namespace AoC.Utils.Computation
{
    public abstract class Instruction
    {
        public int Value { get; }

        public abstract void Invoke(Computer computer);

        protected Instruction(int value)
        {
            Value = value;
        }

        public static Instruction Parse(string s)
        {
            var regex = new Regex("^(?<operation>\\w+) (?<sign>(\\+|-))(?<value>\\d+)$");
            var match = regex.Match(s);
            if (!match.Success)
            {
                throw new InvalidOperationException($"{s} is not an {nameof(Instruction)}");
            }

            var signStr = match.Groups["sign"].Value;
            var valueStr = match.Groups["value"].Value;

            var sign = signStr == "-" ? -1 : 1;
            var value = Convert.ToInt32(valueStr);

            var operation = match.Groups["operation"].Value;
            switch (operation)
            {
                case "acc":
                    return new AccInstruction(sign * value);
                case "jmp":
                    return new JmpInstruction(sign * value);
                case "nop":
                    return new NopInstruction(sign * value);
                default:
                    throw new InvalidOperationException($"{operation} is not a recognised operation");
            }
        }
    }

    public sealed class AccInstruction : Instruction
    {
        public AccInstruction(int value) : base(value)
        {
        }

        public override void Invoke(Computer computer)
        {
            computer.Accumulator += Value;
            computer.Pointer += 1;
        }
    }

    public sealed class JmpInstruction : Instruction
    {
        public JmpInstruction(int value) : base(value)
        {
        }

        public override void Invoke(Computer computer)
        {
            computer.Pointer += Value;
        }
    }

    public sealed class NopInstruction : Instruction
    {
        public NopInstruction(int value) : base(value)
        {
        }

        public override void Invoke(Computer computer)
        {
            computer.Pointer += 1;
        }
    }
}
