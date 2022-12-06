using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode
{
    public class Instruction
    {
        public int CratesCount { get; set; }

        public int FromStack { get; set; }

        public int ToStack { get; set; }

        public Instruction(List<string> instructionNumbers)
        {
            CratesCount = Int32.Parse(instructionNumbers[0]);
            FromStack = Int32.Parse(instructionNumbers[1]);
            ToStack = Int32.Parse(instructionNumbers[2]);
        }
    }

    public class SupplyProblem
    {
        public List<Instruction> Instructions { get; set; }

        public List<Stack<string>> Stacks { get; set; }
    }

    public class Day05 : BaseDay
    {
        private readonly string _input;

        public Day05()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            // process the input string
            var logger = new LogWrapper(false);
            var lineInputs = _input.Split("\n");
            var topCrates = "";

            logger.WriteLine("=======  PART 1 =======");

            /*
             *
             *  1. Build a Data Structure that will hold the information of the current Stacks and the Instructions 
             * 
             */

            var supplyProblem = CreateSupplyProblem(lineInputs, logger);

            /*
             *
             * 2. Solve the Supply Problem
             * 
             */

            foreach (var instruction in supplyProblem.Instructions)
            {
                // take from origin stack
                for (var j = 0; j < instruction.CratesCount; j++)
                {
                    supplyProblem.Stacks[instruction.ToStack - 1]
                        .Push(supplyProblem.Stacks[instruction.FromStack - 1].Pop());
                }
            }

            /*
             *
             * 3. Extract Results from the Current Stacks
             * 
             */

            foreach (var supplyProblemStack in supplyProblem.Stacks)
            {
                if (supplyProblemStack.TryPop(out var popped))
                {
                    topCrates += popped;
                }
            }

            logger.WriteLine($"Final Result: {topCrates}");

            return new(topCrates);
        }


        public override ValueTask<string> Solve_2()
        {
            /*
             *   === Part 2 ====
             *   Will use the same setup and will only differ in the execution of the instructions.
             *   We will pick up X Crates but put them back in the same order
             *
             *   todo: We can potentially see about not having to create the SupplyProblem again as it is the same. 
             * 
             */

            var logger = new LogWrapper(false);
            // process the input string
            var lineInputs = _input.Split("\n");
            var topCrates = "";

            logger.WriteLine("=======  PART 2 =======");

            /*
             *
             *  1. Build a Data Structure that will hold the information of the current Stacks and the Instructions
             * 
             * 
             */

            var supplyProblem = CreateSupplyProblem(lineInputs, logger);

            /*
             *
             * 2. Solve the Supply Problem
             * 
             */

            foreach (var instruction in supplyProblem.Instructions)
            {
                // for this solve we will use a temp stack and first store all the items there

                var tempStack = new Stack<string>();
                for (var j = 0; j < instruction.CratesCount; j++)
                {
                    tempStack.Push(supplyProblem.Stacks[instruction.FromStack - 1].Pop());
                }

                foreach (var item in tempStack)
                {
                    supplyProblem.Stacks[instruction.ToStack - 1].Push(item);
                }
            }

            /*
             *
             * 3. Extract Results from the Current Stacks
             * 
             */

            foreach (var supplyProblemStack in supplyProblem.Stacks)
            {
                if (supplyProblemStack.TryPop(out var popped))
                {
                    topCrates += popped;
                }
            }

            logger.WriteLine($"Final Result: {topCrates}");

            return new(topCrates);
        }

        private SupplyProblem CreateSupplyProblem(string[] lineInputs, LogWrapper logWrapper)
        {
            SupplyProblem supplyProblem = new SupplyProblem();

            // Try to find the empty line and read the stacks from top to bottom?
            var emptyLineIndex = FindEmptyLine(lineInputs, logWrapper);

            // Get the line with the stack numbers and process to figure out how many Stacks we have
            supplyProblem.Stacks = ParseStacks(lineInputs, emptyLineIndex, logWrapper);

            // Read the Instructions
            supplyProblem.Instructions = ParseInstructions(lineInputs, emptyLineIndex, logWrapper);

            return supplyProblem;
        }

        private static List<Instruction> ParseInstructions(string[] lineInputs, int emptyLineIndex, LogWrapper logger)
        {
            List<Instruction> instructions =
                lineInputs.Skip(emptyLineIndex + 1).Select(
                    x => new Instruction(x.Split(" ")
                        .Where(y => Int32.TryParse(y, out _)).ToList())).ToList();

            foreach (var instruction in instructions)
            {
                logger.WriteLine(JsonSerializer.Serialize(instruction));
            }

            return instructions;
        }

        private static List<Stack<string>> ParseStacks(string[] lineInputs, int emptyLineIndex, LogWrapper logger)
        {
            var stackIndexLine = lineInputs[emptyLineIndex - 1];
            var stackIndexValues = stackIndexLine.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Create our stacks and start reading from the bottom up 

            logger.WriteLine($"Creating A List of {stackIndexValues.Count} Stacks");
            List<Stack<string>> crateStacks =
                Enumerable.Range(1, stackIndexValues.Count).Select(i => new Stack<string>()).ToList();

            for (var i = emptyLineIndex - 2; i >= 0; i--)
            {
                var stacksLine = lineInputs[i].Split(" ").ToList().ReplaceConsecutiveEmptyStringsInList(4);

                logger.WriteLine("Current Layout Line: " + JsonSerializer.Serialize(stacksLine));

                for (int j = 0; j < crateStacks.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(stacksLine[j]))
                    {
                        logger.WriteLine($"Adding {stacksLine[j]} to Crate Stack {j}");
                        crateStacks[j].Push(stacksLine[j][1].ToString());
                    }
                }
            }

            return crateStacks;
        }

        private static int FindEmptyLine(string[] lineInputs, LogWrapper logger)
        {
            var emptyLineIndex = 0;
            var currentLine = lineInputs[emptyLineIndex];
            while (!string.IsNullOrWhiteSpace(currentLine))
            {
                emptyLineIndex++;
                currentLine = lineInputs[emptyLineIndex];
            }

            logger.WriteLine($"Empty Line Index:{emptyLineIndex}");
            return emptyLineIndex;
        }
    }
}