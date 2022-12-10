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

    private class CpuState
    {
        public int Cycle { get; set; }

        public List<CpuInstruction> LongRunningInstructions { get; set; }

        public int Register { get; set; }

        public int[] CyclesToCheckStrengths { get; set; }

        public int[] CyclesToCheck { get; set; }

        public int InstructionPointer { get; set; }

        public int CheckCycleIndex { get; set; }

        public CpuState(int[] cyclesToCheck)
        {
            Cycle = 0;
            Register = 1;

            // Cycles to Check
            CyclesToCheck = cyclesToCheck;
            CyclesToCheckStrengths = Enumerable.Repeat(0, cyclesToCheck.Length).ToArray();

            InstructionPointer = 0;
            CheckCycleIndex = 0;

            LongRunningInstructions = new List<CpuInstruction>();
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

        int[] cyclesToCheck =
        {
            20, 60, 100, 140, 180, 220
        };

        var stateAfterInstructions = RunInstructionsWithCheckCycles(cyclesToCheck, logger);

        logger.WriteLine($"Cycle Strengths: {JsonSerializer.Serialize(stateAfterInstructions.CyclesToCheckStrengths)}");

        var sum = stateAfterInstructions.CyclesToCheckStrengths.Sum();

        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Part 2");
    }

    private CpuState RunInstructionsWithCheckCycles(int[] cyclesToCheck, LogWrapper logger)
    {
        CpuState state = new CpuState(cyclesToCheck);

        logger.WriteLine($"Dealing with {_instructions.Count} instructions");

        // Start running the processor
        while (true)
        {
            state.Cycle++;

            // check if we have reached a cycle for which we need to check strength
            if (state.CheckCycleIndex < state.CyclesToCheck.Length &&
                state.Cycle == state.CyclesToCheck[state.CheckCycleIndex])
            {
                logger.WriteLine($"Have hit Cycle: {state.Cycle} after {state.InstructionPointer} instructions");

                state.CyclesToCheckStrengths[state.CheckCycleIndex] =
                    state.Register * state.CyclesToCheck[state.CheckCycleIndex];

                state.CheckCycleIndex++;
            }

            // if we executing adds we look at completing them
            // todo: This might be an issue if we get to execute multiple instructions per cycle
            // todo: we might need to use a FIFO queue 
            if (state.LongRunningInstructions.Any())
            {
                foreach (var executingAdd in state.LongRunningInstructions)
                {
                    executingAdd.CyclesLeft--;
                    if (executingAdd.CyclesLeft == 0)
                    {
                        if (executingAdd.Argument != null) state.Register += executingAdd.Argument.Value;
                    }
                }

                // remove the ones that are finished
                state.LongRunningInstructions = state.LongRunningInstructions.Where(x => x.CyclesLeft != 0).ToList();
            }
            else
            {
                // if we've reached all instructions we need to execute we break;
                if (state.InstructionPointer == _instructions.Count() && !state.LongRunningInstructions.Any())
                {
                    break;
                }

                var instructionToRun = _instructions[state.InstructionPointer];
                state.InstructionPointer++;

                if (instructionToRun.Command == InstuctionCommandTypes.ADDX)
                {
                    state.LongRunningInstructions.Add(new CpuInstruction()
                    {
                        Argument = instructionToRun.Argument,
                        Command = instructionToRun.Command,
                        CyclesLeft = 1
                    });
                }
            }
        }

        return state;
    }
}