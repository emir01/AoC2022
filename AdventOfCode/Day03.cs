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

        var scores = new List<int>();

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
                    var thisRucksackScore = GetCharacterPriority(rucksackPartOne[j]);
                    scores.Add(thisRucksackScore);
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
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }
}