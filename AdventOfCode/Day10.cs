using System.Runtime.InteropServices;
using System.Text.Json;
using AdventOfCode.Utils;
using Spectre.Console.Rendering;

namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private static class InstuctionCommandTypes
    {
        public static string NOOP = "noop";
        public static string ADDX = "addx";
    }

    private class CpuInstruction
    {
        public string Command { get; set; }

        public int? Argument { get; set; }

        public int CyclesLeft { get; set; }

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

        int instructionPointer = 0;
        int checkCycleIndex = 0;

        List<CpuInstruction> executingLongInstructions = new List<CpuInstruction>();

        logger.WriteLine($"Dealing with {_instructions.Count} instructions");

        // Start running the processor
        while (true)
        {
            cycle++;

            // check if we have reached a cycle for which we need to check strength
            if (checkCycleIndex < cyclesToCheck.Length && cycle == cyclesToCheck[checkCycleIndex])
            {
                logger.WriteLine($"Have hit Cycle: {cycle} after {instructionPointer} instructions");
                cyclesToCheckStrengths[checkCycleIndex] = register * cyclesToCheck[checkCycleIndex];
                checkCycleIndex++;
            }

            // if we executing adds we look at completing them
            // todo: This might be an issue if we get to execute multiple instructions per cycle
            // todo: we might need to use a FIFO queue 
            if (executingLongInstructions.Any())
            {
                foreach (var executingAdd in executingLongInstructions)
                {
                    executingAdd.CyclesLeft--;
                    if (executingAdd.CyclesLeft == 0)
                    {
                        if (executingAdd.Argument != null) register += executingAdd.Argument.Value;
                    }
                }

                // remove the ones that are finished
                executingLongInstructions = executingLongInstructions.Where(x => x.CyclesLeft != 0).ToList();
            }
            else
            {
                // if we've reached all instructions we need to execute we break;
                if (instructionPointer == _instructions.Count() && !executingLongInstructions.Any())
                {
                    break;
                }

                var instructionToRun = _instructions[instructionPointer];
                instructionPointer++;

                if (instructionToRun.Command == InstuctionCommandTypes.ADDX)
                {
                    executingLongInstructions.Add(new CpuInstruction()
                    {
                        Argument = instructionToRun.Argument,
                        Command = instructionToRun.Command,
                        CyclesLeft = 1
                    });
                }
            }
        }

        logger.WriteLine($"Cycle Strengths: {JsonSerializer.Serialize(cyclesToCheckStrengths)}");

        var sum = cyclesToCheckStrengths.Sum();

        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Part 2");
    }
}