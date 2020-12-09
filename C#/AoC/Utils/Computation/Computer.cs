using System.Collections.Generic;

namespace AoC.Utils.Computation
{
    public sealed class Computer
    {
        public int Pointer { get; set; }

        public int Accumulator { get; set; }

        public Computer(int pointer, int accumulator)
        {
            Pointer = pointer;
            Accumulator = accumulator;
        }

        public ReturnCode Run(List<Instruction> program)
        {
            var seen = new HashSet<int>();
            while (!seen.Contains(Pointer))
            {
                if (Pointer >= program.Count)
                {
                    return ReturnCode.Completed;
                }
                var next = program[Pointer];
                seen.Add(Pointer);
                next.Invoke(this);
            }

            return ReturnCode.Looping;
        }
    }

    public enum ReturnCode
    {
        Completed = 0,
        Looping = 1,
    }
}
