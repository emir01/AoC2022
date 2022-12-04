using System.Linq.Expressions;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string _input;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var lineInputs = _input.Split("\n");
        var score = 0;

        for (var i = 0; i < lineInputs.Length; i++)
        {
            var rucksackContent = lineInputs[i];
            var rucksackPartOne = rucksackContent.Substring(0, rucksackContent.Length / 2);
            var rucksackPartTwo = rucksackContent.Substring(rucksackContent.Length / 2, rucksackContent.Length / 2);

            // keep some sort of structure to verify which character is found in the other
            for (var j = 0; j < rucksackPartOne.Length; j++)
            {
                if (rucksackPartTwo.Contains(rucksackPartOne[j]))
                {
                    score += GetCharacterPriority(rucksackPartOne[j]);
                    break;
                }
            }
        }

        return new(score.ToString());
    }

    private int GetCharacterPriority(char c)
    {
        if (char.IsUpper(c))
        {
            return ((int)c) - 38;
        }

        return ((int)c) - 96;
    }

    public override ValueTask<string> Solve_2()
    {
        var lineInputs = _input.Split("\n");
        var score = 0;

        var groupsCount = lineInputs.Length / 3;

        for (var i = 0; i < groupsCount; i++)
        {
            var groupItemOccurrenceCounter = new Dictionary<char, int>();

            var knapsackOne = lineInputs[i * 3];
            var knapsackTwo = lineInputs[i * 3 + 1];
            var knapsackThree = lineInputs[i * 3 + 2];

            ProcessKnapsackToCounter(knapsackOne, groupItemOccurrenceCounter);
            ProcessKnapsackToCounter(knapsackTwo, groupItemOccurrenceCounter);
            ProcessKnapsackToCounter(knapsackThree, groupItemOccurrenceCounter);

            var itemOccuredInAllKnapsacks = groupItemOccurrenceCounter.FirstOrDefault(x => x.Value == 3);

            score += GetCharacterPriority(itemOccuredInAllKnapsacks.Key);
        }

        return new(score.ToString());
    }

    private void ProcessKnapsackToCounter(string knapsack, Dictionary<char, int> counter)
    {
        // first create a unique list of all the item types in the current knapsack as we don't want to count
        // items that appear multiple times in the same knapsack.
        // We are only interested in item type occurrences across the 3 knapsacks in a group
        var uniqueKnapsack = "";
        foreach (var item in knapsack)
        {
            if (!uniqueKnapsack.Contains(item))
            {
                uniqueKnapsack += item;
            }
        }

        foreach (var item in uniqueKnapsack)
        {
            if (counter.ContainsKey(item))
            {
                counter[item] += 1;
            }
            else
            {
                counter[item] = 1;
            }
        }
    }
}