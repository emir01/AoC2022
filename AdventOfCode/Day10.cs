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

    public class Crt
    {
        public List<string> Pixels { get; set; }

        public Crt()
        {
            Pixels = Enumerable.Range(1, 240).Select(x => ".").ToList();
        }

        public void PrintCrt(LogWrapper logger)
        {
            logger.WriteLine("================CRT START================");
            var drawnPixelCount = 0;
            for (int i = 0; i < Pixels.Count; i++)
            {
                logger.Write(Pixels[i]);

                drawnPixelCount++;

                if (drawnPixelCount == 40)
                {
                    drawnPixelCount = 0;
                    logger.Write("\n");
                }
            }

            logger.WriteLine("================CRT END==================");
        }

        public void UpdateAfterCycle(int cycle, int stateRegister, LogWrapper logger)
        {
            // check what needs to be drawn
            logger.WriteLine($"At Cycle {cycle} register is Set at: {stateRegister}");

            if (stateRegister == cycle || stateRegister + 1 == cycle || stateRegister - 1 == cycle)
            {
                logger.WriteLine("DRAWING");
                Pixels[cycle - 1] = "#";
            }
            else
            {
                logger.WriteLine("NOT DRAWING");
            }
        }
    }

    private class CpuState
    {
        public int Cycle { get; set; }

        public List<CpuInstruction> LongRunningInstructions { get; set; }

        public int RegisterAtEndOfCycle { get; set; }

        public int RegisterAtStartOfCycle { get; set; }

        public int[] CyclesToCheckStrengths { get; set; }

        public int[] CyclesToCheck { get; set; }

        public int InstructionPointer { get; set; }

        public int CheckCycleIndex { get; set; }

        public CpuState(int[] cyclesToCheck)
        {
            if (cyclesToCheck == null)
            {
                CyclesToCheck = new int[] { };
            }
            else
            {
                CyclesToCheck = cyclesToCheck;
            }

            Cycle = 0;
            RegisterAtEndOfCycle = 1;
            RegisterAtStartOfCycle = 1;


            // Cycles to Check
            CyclesToCheckStrengths = Enumerable.Repeat(0, CyclesToCheck.Length).ToArray();

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
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 1 =====");

        int[] cyclesToCheck =
        {
            20, 60, 100, 140, 180, 220
        };

        var stateAfterInstructions = RunInstructionsWithCheckCycles(logger, cyclesToCheck);

        var sum = stateAfterInstructions.Last().CyclesToCheckStrengths.Sum();

        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 2 =====");

        var crt = new Crt();

        logger.WriteLine($"CRT BEFORE Program Execution");
        crt.PrintCrt(logger);
        var cpuCyclesEnumerable = RunInstructionsWithCheckCycles(logger);
        foreach (var state in cpuCyclesEnumerable)
        {
            crt.UpdateAfterCycle(state.Cycle, state.RegisterAtStartOfCycle, logger);
        }

        logger.WriteLine($"CRT After Program Execution");
        crt.PrintCrt(logger);

        return new("ABCDEFG");
    }

    private IEnumerable<CpuState> RunInstructionsWithCheckCycles(LogWrapper logger, int[] cyclesToCheck = null)
    {
        CpuState state = new CpuState(cyclesToCheck);

        // Start running the processor
        while (true)
        {
            state.Cycle++;
            state.RegisterAtStartOfCycle = state.RegisterAtEndOfCycle;

            // check if we have reached a cycle for which we need to check strength
            if (state.CheckCycleIndex < state.CyclesToCheck.Length &&
                state.Cycle == state.CyclesToCheck[state.CheckCycleIndex])
            {
                logger.WriteLine($"Have hit Cycle: {state.Cycle} after {state.InstructionPointer} instructions");

                state.CyclesToCheckStrengths[state.CheckCycleIndex] =
                    state.RegisterAtEndOfCycle * state.CyclesToCheck[state.CheckCycleIndex];

                state.CheckCycleIndex++;
            }

            // if we executing adds we look at completing them
            if (state.LongRunningInstructions.Any())
            {
                foreach (var executingAdd in state.LongRunningInstructions)
                {
                    executingAdd.CyclesLeft--;
                    if (executingAdd.CyclesLeft == 0)
                    {
                        if (executingAdd.Argument != null) state.RegisterAtEndOfCycle += executingAdd.Argument.Value;
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

            // we return the state at the end of the cycle
            yield return state;
        }
    }
}