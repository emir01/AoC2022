using System.Runtime.InteropServices;
using System.Text.Json;
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

    private List<Instruction> _instructions;

    public Day09()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        _instructions = _lines.Select(x => new Instruction().Parse(x)).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 1 =====");

        var numberOfKnots = 2;
        var knots = Enumerable.Range(0, numberOfKnots).Select(x => new BridgeLoc(0, 0)).ToList();

        logger.WriteLine($"Knot List: {JsonSerializer.Serialize(knots)}");

        // after moving the head 
        Dictionary<int, Dictionary<int, bool>> tailVisitedLocations =
            MoveHeadAndReturnTailLocations(knots, logger);

        // get all marked locations from the dictionary
        var visitedCount = tailVisitedLocations.SelectMany(x => x.Value.Values).Count();

        logger.WriteLine($"Visited Locations: {visitedCount}");

        return new(visitedCount.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 2 =====");

        var numberOfKnots = 10;
        var knots = Enumerable.Range(0, numberOfKnots).Select(x => new BridgeLoc(0, 0)).ToList();

        logger.WriteLine($"Knot List: {JsonSerializer.Serialize(knots)}");

        // after moving the head 
        Dictionary<int, Dictionary<int, bool>> tailVisitedLocations =
            MoveHeadAndReturnTailLocations(knots, logger);

        // get all marked locations from the dictionary
        var visitedCount = tailVisitedLocations.SelectMany(x => x.Value.Values).Count();

        logger.WriteLine($"Visited Locations: {visitedCount}");

        return new(visitedCount.ToString());
    }

    private Dictionary<int, Dictionary<int, bool>> MoveHeadAndReturnTailLocations(List<BridgeLoc> knots,
        LogWrapper logger)
    {
        // we need a data structure to track visited locations
        Dictionary<int, Dictionary<int, bool>> tailVisitedLocations = new Dictionary<int, Dictionary<int, bool>>
        {
            // set initial location as visited
            [0] = new() { { 0, true } }
        };

        foreach (var instruction in _instructions)
        {
            // break down the instructions
            var breakdown = instruction.Breakdown();

            foreach (var stepInstruction in breakdown)
            {
                // move the head of the knots
                stepInstruction.Apply(knots[0]);

                var leadKnot = knots[0];

                // move all the other knots based on current knot starting from the head  
                for (var i = 1; i < knots.Count; i++)
                {
                    // we update the knots[i] knot and if it moved and is the last know - TAIL we track its location 
                    if (UpdateNodeBasedOnDifference(leadKnot, knots[i], logger) && i == knots.Count - 1)
                    {
                        TrackLocation(tailVisitedLocations, knots[i]);
                    }

                    // move on to the next knot to move
                    leadKnot = knots[i];
                }
            }
        }

        return tailVisitedLocations;
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

    private bool UpdateNodeBasedOnDifference(BridgeLoc leaderNode, BridgeLoc followerNode, LogWrapper logger)
    {
        (int x, int y ) diff = (leaderNode.X - followerNode.X, leaderNode.Y - followerNode.Y);

        // logger.WriteLine($"Coord Differences between head and tail: [{diff.x},{diff.y}]");

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

            followerNode.X += diff.x;
            followerNode.Y += diff.y;

            return true;
        }

        return false;
    }
}