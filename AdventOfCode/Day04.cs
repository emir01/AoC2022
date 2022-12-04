using System.Linq.Expressions;
using System.Threading.Tasks.Sources;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day04 : BaseDay
{
    private record Section(int Start, int End);

    private record Pair(Section First, Section Second);

    private readonly string _input;

    public Day04()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var lineInputs = _input.Split("\n");
        var includedPairs = 0;

        //18-41,17-40
        var pairs = lineInputs.Select(x => GetPairFromInput(x)).ToList();

        for (int i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            if (SectionWithinSecond(pair.First, pair.Second) || SectionWithinSecond(pair.Second, pair.First))
            {
                includedPairs++;
            }
        }

        return new(includedPairs.ToString());
    }

    private void OutputPair(Pair pair)
    {
        Console.WriteLine($"{pair.First.Start}-{pair.First.End} === {pair.Second.Start}-{pair.Second.End}");
    }

    private bool SectionWithinSecond(Section one, Section second)
    {
        return one.Start >= second.Start && one.End <= second.End;
    }

    private Pair GetPairFromInput(string input)
    {
        //18-41,17-40
        var splitString = input.Split(",");
        var pairOne = splitString[0].Split("-");
        var pairTwo = splitString[1].Split("-");

        var pair = new Pair(new Section(Int32.Parse(pairOne[0]), Int32.Parse(pairOne[1])),
            new Section(Int32.Parse(pairTwo[0]), Int32.Parse(pairTwo[1])));

        return pair;
    }

    public override ValueTask<string> Solve_2()
    {
        var lineInputs = _input.Split("\n");
        var score = 0;

        return new(score.ToString());
    }
}