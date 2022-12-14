using System.Numerics;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private enum OperationType
    {
        ADD,
        MULTIPLY
    }

    private class Operation
    {
        public OperationType Type { get; set; }

        public long Argument { get; set; }
    }

    private class MonkeyItem
    {
        public long StartingValue { get; set; }

        public long CurrentValue { get; set; }

        public long PreviousValue { get; set; }

        public List<long> Factors { get; set; }

        public List<long> Additions { get; set; }

        public List<long> Divisors { get; set; }

        public List<Operation> OperationsInOrder { get; set; }

        public string NumberInStringFormat { get; set; }


        public MonkeyItem(long value)
        {
            StartingValue = value;
            CurrentValue = value;
            NumberInStringFormat = value.ToString();
            Factors = new List<long>();
            Additions = new List<long>();
            Divisors = new List<long>();
            OperationsInOrder = new List<Operation>();
            OperationsInOrder.Add(new Operation()
            {
                Type = OperationType.MULTIPLY,
                Argument = StartingValue
            });
        }
    }

    private class Monkey
    {
        // The Items the money is holding
        public Queue<MonkeyItem> Items { get; set; }

        public string[] OperationSegments { get; set; }

        public long Divider { get; set; }

        public int MonkeyIndexIfTrue { get; set; }

        public int MonkeyIndex { get; set; }

        public int MonkeyIndexIfFalse { get; set; }

        public long InspectTimes { get; set; }


        private LogWrapper _logger;

        private readonly long _worryLevelDivider;

        public Monkey(LogWrapper logger, int monkeyIndex, long worryLevelDivider)
        {
            Items = new Queue<MonkeyItem>();
            _logger = logger;
            _worryLevelDivider = worryLevelDivider;
            MonkeyIndex = monkeyIndex;
            InspectTimes = 0;
        }

        public void ParseItems(string items)
        {
            var splitItems = items.Split(",");
            foreach (var splitItem in splitItems)
            {
                Items.Enqueue(new MonkeyItem(Int32.Parse(splitItem.Trim())));
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

        public MonkeyItem InspectItem()
        {
            var item = Items.Dequeue();

            ApplyOperation(item);

            item.CurrentValue /= _worryLevelDivider;

            // what if we only deal with the last 3 digits?

            // _logger.WriteLine(
            //     $"Monkey with index:{MonkeyIndex} is inspecting original Item: {item} " +
            //     $"which peaks at: {itemAfterOperation} " +
            //     $"and then drops down to {itemAfterInspect}");

            InspectTimes++;

            return item;
        }

        // Should be the opposite of Apply
        public void UndoOperation(MonkeyItem item)
        {
            var isSelfMultiply = OperationSegments[2] == "old";

            long argument = isSelfMultiply
                ? item.CurrentValue
                : long.Parse(OperationSegments[2]);

            switch (OperationSegments[1])
            {
                case "+":

                    item.Additions.Add(argument);
                    item.OperationsInOrder.Add(new Operation()
                    {
                        Argument = argument,
                        Type = OperationType.ADD
                    });

                    item.CurrentValue += argument;

                    break;
                case "*":
                    item.Factors.Add(argument);
                    item.OperationsInOrder.Add(new Operation()
                    {
                        Argument = argument,
                        Type = OperationType.MULTIPLY
                    });
                    item.CurrentValue *= argument;


                    break;
            }
        }

        private void ApplyOperation(MonkeyItem item)
        {
            var isSelfMultiply = OperationSegments[2] == "old";

            long argument = isSelfMultiply
                ? item.CurrentValue
                : long.Parse(OperationSegments[2]);

            item.PreviousValue = item.CurrentValue;

            switch (OperationSegments[1])
            {
                case "+":

                    item.Additions.Add(argument);
                    item.OperationsInOrder.Add(new Operation()
                    {
                        Argument = argument,
                        Type = OperationType.ADD
                    });

                    item.CurrentValue += argument;

                    break;
                case "*":
                    item.Factors.Add(argument);
                    item.OperationsInOrder.Add(new Operation()
                    {
                        Argument = argument,
                        Type = OperationType.MULTIPLY
                    });

                    item.CurrentValue *= argument;

                    break;
            }
        }

        public int FindOutToWhichMonkeyToThrow(MonkeyItem item, List<Monkey> monkeys)
        {
            var condition = true;
            //
            // if (_worryLevelDivider == 1)
            // {
            //     // if we have not done anything with the number 
            //     // probably never the case
            //     if (!item.Factors.Any() && !item.Additions.Any())
            //     {
            //         condition = item.StartingValue % Divider == 0;
            //     }
            //     else if (!item.Additions.Any())
            //     {
            //         // if we only have multiplications so far
            //         condition = item.StartingValue % Divider == 0 || item.Factors.Any(x => x == Divider);
            //     }
            //     else if (!item.Factors.Any())
            //     {
            //         condition = (item.Additions.Sum() + item.StartingValue) % Divider == 0;
            //     }
            //     else
            //     {
            //         // start from the end of the operations
            //         long activeSum = 0;
            //         var divisible = false;
            //
            //         condition = false;
            //
            //         // // start from the start of the operations ? 
            //         // for (int i = 0; i < item.OperationsInOrder.Count; i++)
            //         // {
            //         //     var operation = item.OperationsInOrder[i];
            //         //
            //         //     if (operation.Type == OperationType.MULTIPLY)
            //         //     {
            //         //     }
            //         // }
            //     }
            // }
            // else
            // {
            //     condition = item.CurrentValue % Divider == 0;
            // }

            condition = item.CurrentValue % Divider == 0;

            var monkeyIndex = condition ? MonkeyIndexIfTrue : MonkeyIndexIfFalse;

            return monkeyIndex;
        }

        public bool HasItem()
        {
            return Items.TryPeek(out MonkeyItem _);
        }
    }

    private List<Monkey> _monkeys = new();

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 1 =====");

        ParseMonkeysFromInput(logger, 3);

        PlayRounds(20, logger);

        var monkeyInspectsByOrder = _monkeys.Select(x => x.InspectTimes).OrderByDescending(x => x).ToList();

        var solution = monkeyInspectsByOrder[0] * monkeyInspectsByOrder[1];

        return new(solution.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 2 =====");

        // ParseMonkeysFromInput(logger, 1);
        //
        // PlayRounds(10000, logger, true);
        //
        // var monkeyInspectsByOrder = _monkeys.Select(x => new { time = x.InspectTimes, index = x.MonkeyIndex })
        //     .OrderByDescending(x => x.time).ToList();
        //
        // logger.WriteLine($"Monkey Inspect times: {JsonSerializer.Serialize(monkeyInspectsByOrder)}");
        //
        // var solution = monkeyInspectsByOrder[0].time * monkeyInspectsByOrder[1].time;

        return new("FAIL");
    }

    private void PlayRounds(int rounds, LogWrapper logger, bool optimizeThrownValues = false)
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

                logger.WriteLine($"*** It's the turn of Monkey with Index:{activeMonkey.MonkeyIndex}");

                while (activeMonkey.HasItem())
                {
                    var inspectValue = activeMonkey.InspectItem();

                    logger.WriteLine(
                        $"======= Inspected Item with Value : {inspectValue.CurrentValue} - Monkey now has Items: {JsonSerializer.Serialize(activeMonkey.Items.Select(x => x.CurrentValue))}");

                    var throwToWhichMonkey = activeMonkey.FindOutToWhichMonkeyToThrow(inspectValue, _monkeys);

                    _monkeys[throwToWhichMonkey].Items.Enqueue(inspectValue);

                    logger.WriteLine(
                        $"======= Throwing Inspected Item: {inspectValue.CurrentValue} to {throwToWhichMonkey}");
                }
            }

            logger.WriteLine($"Monkey state after round {i}");

            foreach (var monkey in _monkeys)
            {
                logger.WriteLine(
                    $"Monkey[{monkey.MonkeyIndex}][{monkey.InspectTimes}]: {JsonSerializer.Serialize(monkey.Items.Select(x => x.CurrentValue).ToList())}");
            }
        }
    }

    private void ParseMonkeysFromInput(LogWrapper logger, long worryLevelDivider)
    {
        var monkeyIndex = 0;
        var activeMonkey = new Monkey(logger, monkeyIndex, worryLevelDivider);
        _monkeys = new List<Monkey>();
        for (int i = 0; i < _lines.Count; i++)
        {
            var activeLine = _lines[i];

            if (string.IsNullOrWhiteSpace(activeLine))
            {
                // done with processing the monkey 
                // add it to the list and create a new one
                _monkeys.Add(activeMonkey);
                monkeyIndex++;
                activeMonkey = new Monkey(logger, monkeyIndex, worryLevelDivider);
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
}