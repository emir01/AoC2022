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
            var lineInputs = _input.Split("\n");
            var topCrates = "";

            /*
             *
             *  1. Build a Data Structure that will hold the information of the current Stacks and the Instructions 
             * 
             */

            var supplyProblem = CreateSupplyProblem(lineInputs);

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

            Console.WriteLine($"Final Result: {topCrates}");

            return new(topCrates);
        }

        private SupplyProblem CreateSupplyProblem(string[] lineInputs)
        {
            SupplyProblem supplyProblem = new SupplyProblem();

            // Try to find the empty line and read the stacks from top to bottom?
            var emptyLineIndex = FindEmptyLine(lineInputs);

            // Get the line with the stack numbers and process to figure out how many Stacks we have
            supplyProblem.Stacks = ParseStacks(lineInputs, emptyLineIndex);

            // Read the Instructions
            supplyProblem.Instructions = ParseInstructions(lineInputs, emptyLineIndex);

            return supplyProblem;
        }

        private static List<Instruction> ParseInstructions(string[] lineInputs, int emptyLineIndex)
        {
            List<Instruction> instructions =
                lineInputs.Skip(emptyLineIndex + 1).Select(
                    x => new Instruction(x.Split(" ")
                        .Where(y => Int32.TryParse(y, out _)).ToList())).ToList();

            foreach (var instruction in instructions)
            {
                Console.WriteLine(JsonSerializer.Serialize(instruction));
            }

            return instructions;
        }

        private static List<Stack<string>> ParseStacks(string[] lineInputs, int emptyLineIndex)
        {
            var stackIndexLine = lineInputs[emptyLineIndex - 1];
            var stackIndexValues = stackIndexLine.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Create our stacks and start reading from the bottom up 

            Console.WriteLine($"Creating A List of {stackIndexValues.Count} Stacks");
            List<Stack<string>> crateStacks =
                Enumerable.Range(1, stackIndexValues.Count).Select(i => new Stack<string>()).ToList();

            for (var i = emptyLineIndex - 2; i >= 0; i--)
            {
                var stacksLine = lineInputs[i].Split(" ").ToList().ReplaceConsecutiveEmptyStringsInList(4);

                Console.WriteLine("Current Layout Line: " + JsonSerializer.Serialize(stacksLine));

                for (int j = 0; j < crateStacks.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(stacksLine[j]))
                    {
                        Console.WriteLine($"Adding {stacksLine[j]} to Crate Stack {j}");
                        crateStacks[j].Push(stacksLine[j][1].ToString());
                    }
                }
            }

            return crateStacks;
        }

        private static int FindEmptyLine(string[] lineInputs)
        {
            var emptyLineIndex = 0;
            var currentLine = lineInputs[emptyLineIndex];
            while (!string.IsNullOrWhiteSpace(currentLine))
            {
                emptyLineIndex++;
                currentLine = lineInputs[emptyLineIndex];
            }

            Console.WriteLine($"Empty Line Index:{emptyLineIndex}");
            return emptyLineIndex;
        }


        public override ValueTask<string> Solve_2()
        {
            // process the input string
            var lineInputs = _input.Split("\n");
            var score = 0;


            return new(score.ToString());
        }
    }
}