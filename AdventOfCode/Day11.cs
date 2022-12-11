using System.Diagnostics;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private class Monkey
    {
        // The Items the money is holding
        public Queue<int> MonkeyItems { get; set; }

        public string[] OperationSegments { get; set; }

        public int Divider { get; set; }

        public int MonkeyIndexIfTrue { get; set; }

        public int MonkeyIndex { get; set; }

        public int MonkeyIndexIfFalse { get; set; }

        public int InspectTimes { get; set; }

        private LogWrapper _logger;

        public Monkey(LogWrapper logger, int monkeyIndex)
        {
            MonkeyItems = new Queue<int>();
            _logger = logger;
            MonkeyIndex = monkeyIndex;
            InspectTimes = 0;
        }

        public void ParseItems(string items)
        {
            var splitItems = items.Split(",");
            foreach (var splitItem in splitItems)
            {
                MonkeyItems.Enqueue(Int32.Parse(splitItem.Trim()));
            }
        }

        public void ParseOperation(string operation)
        {
            var operationSegments = operation.Trim().Split("=")[1].Trim().Split(" ");
            OperationSegments = operationSegments;
        }

        public void ParseTest(string test)
        {
            var splitTest = test.Split(" ").Last();
            Divider = Int32.Parse(splitTest.Trim());
        }

        public void ParseIfTrue(string ifTrue)
        {
            MonkeyIndexIfTrue = Int32.Parse(ifTrue.Split(" ").Last().Trim());
        }

        public void ParseIfFalse(string ifFalse)
        {
            MonkeyIndexIfFalse = Int32.Parse(ifFalse.Split(" ").Last().Trim());
        }

        public int InspectItem()
        {
            var item = MonkeyItems.Dequeue();
            var itemAfterOperation = ApplyOperation(item);
            int itemAfterInspect = itemAfterOperation / 3;

            // _logger.WriteLine(
            //     $"Monkey with index:{MonkeyIndex} is inspecting original Item: {item} " +
            //     $"which peaks at: {itemAfterOperation} " +
            //     $"and then drops down to {itemAfterInspect}");

            InspectTimes++;

            return itemAfterInspect;
        }

        private int ApplyOperation(int item)
        {
            var result = item;
            int argument = OperationSegments[2] == "old" ? item : Int32.Parse(OperationSegments[2]);

            switch (OperationSegments[1])
            {
                case "+":
                    result += argument;
                    break;
                case "*":
                    result *= argument;
                    break;
            }

            return result;
        }

        public int FindOutToWhichMonkeyToThrow(int inspectValue)
        {
            if (inspectValue % Divider == 0)
            {
                return MonkeyIndexIfTrue;
            }

            return MonkeyIndexIfFalse;
        }

        public bool HasItem()
        {
            return MonkeyItems.TryPeek(out int _);
        }
    }

    private List<Monkey> _monkeys = new List<Monkey>();

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        ParseMonkeysFromInput(logger);

        // test parsing
        foreach (var monkey in _monkeys)
        {
            logger.WriteLine("===========================");
            logger.WriteLine($"Monkey: {JsonSerializer.Serialize(monkey)}");
            logger.WriteLine("===========================");
        }

        PlayRounds(20, logger);

        var monkeyInspectsByOrder = _monkeys.Select(x => x.InspectTimes).OrderByDescending(x => x).ToList();

        logger.WriteLine($"Monkey Inspect times: {JsonSerializer.Serialize(monkeyInspectsByOrder)}");

        var solution = monkeyInspectsByOrder[0] * monkeyInspectsByOrder[1];

        return new(solution.ToString());
    }

    private void PlayRounds(int rounds, LogWrapper logger)
    {
        for (int i = 1; i <= rounds; i++)
        {
            // Monkey inspects an item with a worry level of 79.
            //     Worry level is multiplied by 19 to 1501.
            //     Monkey gets bored with item. Worry level is divided by 3 to 500.
            //     Current worry level is not divisible by 23.
            //     Item with worry level 500 is thrown to monkey 3.

            for (int j = 0; j < _monkeys.Count(); j++)
            {
                var activeMonkey = _monkeys[j];

                //logger.WriteLine($"*** It's the turn of Monkey with Index:{activeMonkey.MonkeyIndex}");

                while (activeMonkey.HasItem())
                {
                    var inspectValue = activeMonkey.InspectItem();

                    // logger.WriteLine(
                    //     $"======= Inspected Item with Value : {inspectValue} - Monkey now has Items: {JsonSerializer.Serialize(activeMonkey.MonkeyItems)}");

                    var throwToWhichMonkey = activeMonkey.FindOutToWhichMonkeyToThrow(inspectValue);
                    _monkeys[throwToWhichMonkey].MonkeyItems.Enqueue(inspectValue);

                    // logger.WriteLine(
                    //     $"======= Throwing Inspected Item: {inspectValue} to {throwToWhichMonkey}");
                }
            }

            logger.WriteLine($"Monkey state after round {i}");
            foreach (var monkey in _monkeys)
            {
                logger.WriteLine($"Monkey[{monkey.MonkeyIndex}]: {JsonSerializer.Serialize(monkey.MonkeyItems)}");
            }
        }
    }

    private void ParseMonkeysFromInput(LogWrapper logger)
    {
        var monkeyIndex = 0;
        var activeMonkey = new Monkey(logger, monkeyIndex);
        for (int i = 0; i < _lines.Count; i++)
        {
            var activeLine = _lines[i];

            if (string.IsNullOrWhiteSpace(activeLine))
            {
                // done with processing the monkey 
                // add it to the list and create a new one
                _monkeys.Add(activeMonkey);
                monkeyIndex++;
                activeMonkey = new Monkey(logger, monkeyIndex);
                continue;
            }

            var split = activeLine.Split(":");
            if (split[0].Contains("Starting"))
            {
                activeMonkey.ParseItems(split[1].Trim());
            }

            if (split[0].Contains("Operation"))
            {
                activeMonkey.ParseOperation(split[1].Trim());
            }

            if (split[0].Contains("Test"))
            {
                activeMonkey.ParseTest(split[1].Trim());
            }

            if (split[0].Contains("If true"))
            {
                activeMonkey.ParseIfTrue(split[1].Trim());
            }

            if (split[0].Contains("If false"))
            {
                activeMonkey.ParseIfFalse(split[1].Trim());
            }

            // check if last line 
            if (i == _lines.Count - 1)
            {
                _monkeys.Add(activeMonkey);
            }
        }
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("SOL 2");
    }
}