using System.Runtime.InteropServices;
using System.Text.Json;
using AdventOfCode.Utils;
using Spectre.Console.Rendering;

namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private static class InstuctionTypes
    {
        public static string NOOP = "noop";
        public static string ADDX = "addx";
    }

    private class CpuInstruction
    {
        public string Command { get; set; }

        public int? Argument { get; set; }

        public CpuInstruction Parse(string input)
        {
            var splits = input.Split(" ");

            Command = splits[0];

            if (splits.Length > 1)
            {
                Argument = Int32.Parse(splits[1]);
            }

            return this;
        }
    }

    private List<CpuInstruction> _instructions;

    public Day10()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        _instructions = _lines.Select(x => new CpuInstruction().Parse(x)).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        int cycle = 0;
        int register = 1;

        int[] cyclesToCheck =
        {
            20, 60, 100, 140, 180, 220
        };

        int[] cyclesToCheckStrengths =
        {
            0, 0, 0, 0, 0, 0
        };

        int checkCycleIndex = 0;

        logger.WriteLine($"Dealing with {_instructions.Count} instructions");

        for (int i = 0; i < _instructions.Count; i++)
        {
            logger.WriteLine($"Running Cycle: {cycle}");
            // check if we have reached a
            if (checkCycleIndex < cyclesToCheck.Length - 1 && cycle == cyclesToCheck[checkCycleIndex])
            {
                logger.WriteLine($"Have hit Cycle: {cycle} after {i + 1} instructions");
                cyclesToCheckStrengths[checkCycleIndex] = register * cyclesToCheck[checkCycleIndex];
                checkCycleIndex++;
            }

            var instruction = _instructions[i];

            if (instruction.Command == InstuctionTypes.NOOP)
            {
                cycle++;
            }
            else
            {
                // increasing this by 2 would not work as we will jump the important cycles
                cycle += 2;
                if (instruction.Argument != null) register += instruction.Argument.Value;
            }
        }

        logger.WriteLine($"Cycle Strengths: {JsonSerializer.Serialize(cyclesToCheckStrengths)}");

        return new("Part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Part 2");
    }
}