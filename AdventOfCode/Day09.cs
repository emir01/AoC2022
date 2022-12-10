using System.Runtime.InteropServices;
using AdventOfCode.Utils;
using Spectre.Console.Rendering;

namespace AdventOfCode;

public class Day09 : BaseDay
{
    private class BridgeLoc
    {
        public int X { get; set; }

        public int Y { get; set; }

        public BridgeLoc(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    private class InputConstants
    {
        public static string UP = "U";
        public static string DOWN = "D";
        public static string LEFT = "L";
        public static string RIGHT = "R";
    }

    private enum BridgeDirection
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    private class Instruction
    {
        public int Steps { get; set; }

        public BridgeDirection Direction { get; set; }

        public Instruction()
        {
        }

        public Instruction(int steps, BridgeDirection direction)
        {
            Steps = steps;
            Direction = direction;
        }

        public List<Instruction> Breakdown()
        {
            // break down the current instruction if it contains multiple steps into single steps
            return Steps > 1
                ? Enumerable.Repeat(new Instruction(1, Direction), Steps).ToList()
                : new List<Instruction>() { this };
        }

        public Instruction Parse(string line)
        {
            var splits = line.Split(" ");

            if (splits[0] == InputConstants.UP)
            {
                Direction = BridgeDirection.UP;
            }

            if (splits[0] == InputConstants.DOWN)
            {
                Direction = BridgeDirection.DOWN;
            }

            if (splits[0] == InputConstants.LEFT)
            {
                Direction = BridgeDirection.LEFT;
            }

            if (splits[0] == InputConstants.RIGHT)
            {
                Direction = BridgeDirection.RIGHT;
            }

            Steps = Int32.Parse(splits[1]);

            return this;
        }

        public void Apply(BridgeLoc location)
        {
            // apply the instruction to the location
            switch (Direction)
            {
                case BridgeDirection.UP:
                    location.Y += Steps;
                    break;
                case BridgeDirection.RIGHT:
                    location.X += Steps;
                    break;
                case BridgeDirection.DOWN:
                    location.Y -= Steps;
                    break;
                case BridgeDirection.LEFT:
                    location.X -= Steps;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private readonly string _input;
    private readonly List<string> _lines;

    private List<Instruction> _instructions = new List<Instruction>();

    public Day09()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        _instructions = _lines.Select(x => new Instruction().Parse(x)).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var result = "";
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 1 =====");

        var head = new BridgeLoc(0, 0);
        var tail = new BridgeLoc(0, 0);

        // we need a data structure to track visited locations
        Dictionary<int, Dictionary<int, bool>> visitedLocations = new Dictionary<int, Dictionary<int, bool>>
        {
            // set initial location as visited
            [0] = new() { { 0, true } }
        };

        foreach (var instruction in _instructions)
        {
            var breakdown = instruction.Breakdown();

            foreach (var stepInstruction in breakdown)
            {
                stepInstruction.Apply(head);

                if (UpdateTail(head, tail, logger))
                {
                    // tail was moved so we need to track its location
                    TrackLocation(visitedLocations, tail);
                }
            }
        }

        // get all marked locations from the dictionary
        var visitedCount = visitedLocations.SelectMany(x => x.Value.Values).Count();

        logger.WriteLine($"Visited Locations: {visitedCount}");

        return new(result);
    }

    private void TrackLocation(Dictionary<int, Dictionary<int, bool>> visitedLocations, BridgeLoc tail)
    {
        if (visitedLocations.ContainsKey(tail.X))
        {
            var yCoordsForX = visitedLocations[tail.X];

            // if we don't have the Key for the given X, Y Coords we need to track it.
            // Otherwise we don't have to do anything.
            if (!yCoordsForX.ContainsKey(tail.Y))
            {
                yCoordsForX[tail.Y] = true;
            }
        }
        else
        {
            // first time we've hit this X location
            // So we create an entry and Track the Y Location
            visitedLocations[tail.X] = new Dictionary<int, bool>()
            {
                { tail.Y, true }
            };
        }
    }

    private bool UpdateTail(BridgeLoc head, BridgeLoc tail, LogWrapper logger)
    {
        (int x, int y ) diff = (head.X - tail.X, head.Y - tail.Y);

        //logger.WriteLine($"Coord Differences between head and tail: [{diff.x},{diff.y}]");

        // we should move the tail
        if (Math.Abs(diff.x) > 1 || Math.Abs(diff.y) > 1)
        {
            // for the value that is above Abs greater that one we have to reduce
            if (Math.Abs(diff.x) > 1)
            {
                if (diff.x < 0)
                {
                    diff.x = -1;
                }
                else
                {
                    diff.x = 1;
                }
            }

            if (Math.Abs(diff.y) > 1)
            {
                if (diff.y < 0)
                {
                    diff.y = -1;
                }
                else
                {
                    diff.y = 1;
                }
            }

            tail.X += diff.x;
            tail.Y += diff.y;

            return true;
        }

        return false;
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var result = "";

        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new(result);
    }
}