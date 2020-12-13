using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AoC._12
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

        private async Task<int> Part1()
        {
            var instructions = await ParseInput();
            Coordinate initialPosition = (0, 0);
            var initialBearing = Direction.East;
            var ship = new Ship(initialPosition, initialBearing);

            foreach (var instruction in instructions)
            {
                ship.Accept(instruction);
            }

            return ship.Position.L1(initialPosition);
        }

        private async Task<int> Part2()
        {
            var instructions = await ParseInput();
            Coordinate initialPosition = (0, 0);
            var initialBearing = Direction.East;
            var ship = new Ship(initialPosition, initialBearing);
            var waypoint = new Waypoint(initialPosition + (10, -1));

            foreach (var instruction in instructions)
            {
                ship.Accept(instruction, waypoint);
            }

            return ship.Position.L1(initialPosition);
        }


        private async Task<List<Instruction>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("12/input");
            return text.Split("\r\n").Select(l => l.Trim()).Select(Instruction.Parse).ToList();
        }
    }

    public sealed class Ship
    {
        public Coordinate Position { get; set; }

        public Direction Bearing { get; set; }

        public Ship(Coordinate position, Direction bearing)
        {
            Position = position;
            Bearing = bearing;
        }

        public void Accept(Instruction instruction)
        {
            instruction.Invoke(this);
        }

        public void Accept(Instruction instruction, Waypoint waypoint)
        {
            instruction.Invoke(this, waypoint);
        }
    }

    public sealed class Waypoint
    {
        public Coordinate Position { get; set; }

        public Waypoint(Coordinate position)
        {
            Position = position;
        }
    }

    public abstract class Instruction
    {
        public abstract void Invoke(Ship ship);

        public abstract void Invoke(Ship ship, Waypoint waypoint);

        public static Instruction Parse(string line)
        {
            var first = line[0];
            var value = Convert.ToInt32(line.Substring(1));
            switch (first)
            {
                case 'F':
                    return new ForwardInstruction(value);
                case 'L':
                    return new TurnLeftInstruction(value);
                case 'R':
                    return new TurnRightInstruction(value);
                case 'N':
                    return new ShiftNorthInstruction(value);
                case 'E':
                    return new ShiftEastInstruction(value);
                case 'S':
                    return new ShiftSouthInstruction(value);
                case 'W':
                    return new ShiftWestInstruction(value);
                default:
                    throw new InvalidOperationException($"Invalid {nameof(Instruction)}: {line}");
            }
        }
    }

    public abstract class TurnInstruction : Instruction
    {
        private readonly int _value;
        private readonly bool _clockwise;

        protected TurnInstruction(int value, bool clockwise)
        {
            _value = value;
            _clockwise = clockwise;
        }

        public override void Invoke(Ship ship)
        {
            ship.Bearing = _clockwise ? ship.Bearing.RotateClockwise(_value) : ship.Bearing.RotateAnticlockwise(_value);
        }

        public override void Invoke(Ship ship, Waypoint waypoint)
        {
            waypoint.Position = _clockwise
                ? waypoint.Position.RotateClockwise(_value, ship.Position)
                : waypoint.Position.RotateAnticlockwise(_value, ship.Position);
        }
    }

    public sealed class TurnRightInstruction : TurnInstruction
    {
        public TurnRightInstruction(int value) : base(value, true)
        {

        }
    }

    public sealed class TurnLeftInstruction : TurnInstruction
    {
        public TurnLeftInstruction(int value) : base(value, false)
        {
        }
    }

    public abstract class ShiftInstruction : Instruction
    {
        private readonly Direction _direction;
        private readonly int _value;

        protected ShiftInstruction(Direction direction, int value)
        {
            _direction = direction;
            _value = value;
        }

        public override void Invoke(Ship ship)
        {
            ship.Position = ship.Position.Move(_direction, _value);
        }

        public override void Invoke(Ship ship, Waypoint waypoint)
        {
            waypoint.Position = waypoint.Position.Move(_direction, _value);
        }
    }

    public sealed class ShiftNorthInstruction : ShiftInstruction
    {
        public ShiftNorthInstruction(int value) : base(Direction.North, value)
        {
        }
    }

    public sealed class ShiftEastInstruction : ShiftInstruction
    {
        public ShiftEastInstruction(int value) : base(Direction.East, value)
        {
        }
    }

    public sealed class ShiftSouthInstruction : ShiftInstruction
    {
        public ShiftSouthInstruction(int value) : base(Direction.South, value)
        {
        }
    }

    public sealed class ShiftWestInstruction : ShiftInstruction
    {
        public ShiftWestInstruction(int value) : base(Direction.West, value)
        {
        }
    }

    public sealed class ForwardInstruction : Instruction
    {
        private readonly int _value;

        public ForwardInstruction(int value)
        {
            _value = value;
        }

        public override void Invoke(Ship ship)
        {
            ship.Position = ship.Position.Move(ship.Bearing, _value);
        }

        public override void Invoke(Ship ship, Waypoint waypoint)
        {
            var xDiff = waypoint.Position.X - ship.Position.X;
            var yDiff = waypoint.Position.Y - ship.Position.Y;
            ship.Position += (_value * xDiff, _value * yDiff);
            waypoint.Position += (_value * xDiff, _value * yDiff);
        }
    }

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public sealed class Coordinate : IEquatable<Coordinate>
    {
        public int X { get; }

        public int Y { get; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int L1(Coordinate other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public Coordinate Move(Direction direction, int value)
        {
            (int X, int Y) diff;
            switch (direction)
            {
                case Direction.North:
                    diff = (0, -value);
                    break;
                case Direction.East:
                    diff = (value, 0);
                    break;
                case Direction.South:
                    diff = (0, value);
                    break;
                case Direction.West:
                    diff = (-value, 0);
                    break;
                default:
                    throw new Exception($"{direction} is not a recognised {nameof(Direction)}");
            }

            return this + diff;
        }

        public static implicit operator (int X, int Y)(Coordinate c)
        {
            return (c.X, c.Y);
        }

        public static implicit operator Coordinate((int X, int Y) c)
        {
            return new Coordinate(c.X, c.Y);
        }

        public static Coordinate operator +(Coordinate coord, (int X, int Y) vector)
        {
            return new Coordinate(coord.X + vector.X, coord.Y + vector.Y);
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

            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Coordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    public static class RotationExtensions
    {
        public static Direction RotateClockwise(this Direction direction, int degrees)
        {
            if (degrees < 0)
            {
                return RotateAnticlockwise(direction, -degrees);
            }
            var quarterTurns = degrees / 90;
            var newDirection = direction;
            for (var i = 0; i < quarterTurns; i++)
            {
                newDirection = Rotate90DegreesClockwise(newDirection);
            }

            return newDirection;
        }
        
        public static Direction Rotate90DegreesClockwise(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.East;
                case Direction.East:
                    return Direction.South;
                case Direction.South:
                    return Direction.West;
                case Direction.West:
                    return Direction.North;
                default:
                    throw new Exception($"{direction} is not a recognised {nameof(Direction)}");
            }
        }

        public static Direction RotateAnticlockwise(this Direction direction, int degrees)
        {
            if (degrees < 0)
            {
                return RotateClockwise(direction, -degrees);
            }
            var quarterTurns = degrees / 90;
            var newDirection = direction;
            for (var i = 0; i < quarterTurns; i++)
            {
                newDirection = Rotate90DegreesAnticlockwise(newDirection);
            }

            return newDirection;
        }

        public static Direction Rotate90DegreesAnticlockwise(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.West;
                case Direction.East:
                    return Direction.North;
                case Direction.South:
                    return Direction.East;
                case Direction.West:
                    return Direction.South;
                default:
                    throw new Exception($"{direction} is not a recognised {nameof(Direction)}");
            }
        }

        public static Coordinate RotateClockwise(this Coordinate self, int degrees, Coordinate pivot)
        {
            if (degrees < 0)
            {
                return RotateAnticlockwise(self, -degrees, pivot);
            }
            var quarterTurns = degrees / 90;
            var newPoint = self;
            for (var i = 0; i < quarterTurns; i++)
            {
                newPoint = Rotate90DegreesClockwise(newPoint, pivot);
            }

            return newPoint;
        }


        public static Coordinate Rotate90DegreesClockwise(this Coordinate self, Coordinate pivot)
        {
            var xDiff = self.X - pivot.X;
            var yDiff = self.Y - pivot.Y;
            return pivot + (-yDiff, xDiff);
        }

        public static Coordinate RotateAnticlockwise(this Coordinate self, int degrees, Coordinate pivot)
        {
            if (degrees < 0)
            {
                return RotateClockwise(self, -degrees, pivot);
            }
            var quarterTurns = degrees / 90;
            var newPoint = self;
            for (var i = 0; i < quarterTurns; i++)
            {
                newPoint = Rotate90DegreesAnticlockwise(newPoint, pivot);
            }

            return newPoint;
        }

        public static Coordinate Rotate90DegreesAnticlockwise(this Coordinate self, Coordinate pivot)
        {
            var xDiff = self.X - pivot.X;
            var yDiff = self.Y - pivot.Y;
            return pivot + (yDiff, -xDiff);
        }

    }
}