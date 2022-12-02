namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var lineInputs = _input.Split("\n");
        var currentScore = 0;

        /*
         * Define a structure to easily reference the scores we get based on our and enemies moves
         * Rock       X     A
         * Paper      Y     B
         * Scissors   Z     C
         */

        Dictionary<string, Dictionary<string, int>> rules = new Dictionary<string, Dictionary<string, int>>
        {
            { "X", new Dictionary<string, int>() { { "score", 1 }, { "A", 3 }, { "B", 0 }, { "C", 6 } } },
            { "Y", new Dictionary<string, int>() { { "score", 2 }, { "A", 6 }, { "B", 3 }, { "C", 0 } } },
            { "Z", new Dictionary<string, int>() { { "score", 3 }, { "A", 0 }, { "B", 6 }, { "C", 3 } } }
        };

        for (var i = 0; i < lineInputs.Length; i++)
        {
            var moves = lineInputs[i].Split(" ");
            var opponentMove = moves[0];
            var myMove = moves[1];

            var moveScore = rules[myMove]["score"];
            var outcomeScore = rules[myMove][opponentMove];

            currentScore += moveScore + outcomeScore;
        }

        return new(currentScore.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }
}