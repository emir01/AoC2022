namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var maxCalories = 0;
        var currentSum = 0;
        // process the input string

        var lineInputs = _input.Split("\n");

        for (int i = 0; i < lineInputs.Length; i++)
        {
            var activeCalorieCount = lineInputs[i];

            if (string.IsNullOrWhiteSpace(activeCalorieCount))
            {
                if (currentSum > maxCalories)
                {
                    maxCalories = currentSum;
                }

                currentSum = 0;
            }
            else
            {
                currentSum += Int32.Parse(activeCalorieCount);
            }
        }

        return new(maxCalories.ToString());
    }

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
}