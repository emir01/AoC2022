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

        var pairs = lineInputs.Select(x => GetPairFromInput(x)).ToList();

        for (int i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            if (SectionFullyWithinTheOther(pair.First, pair.Second) ||
                SectionFullyWithinTheOther(pair.Second, pair.First))
            {
                includedPairs++;
            }
        }

        return new(includedPairs.ToString());
    }

    private bool SectionFullyWithinTheOther(Section one, Section second)
    {
        return one.Start >= second.Start && one.End <= second.End;
    }

    private bool SectionPartiallyWithinTheOther(Section one, Section second)
    {
        return one.End >= second.Start;
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
        // process the input string
        var lineInputs = _input.Split("\n");
        var partialllyIncludedPair = 0;

        var pairs = lineInputs.Select(x => GetPairFromInput(x)).ToList();

        for (int i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            if (SectionPartiallyWithinTheOther(pair.First, pair.Second) &&
                SectionPartiallyWithinTheOther(pair.Second, pair.First))
            {
                partialllyIncludedPair++;
            }
        }

        return new(partialllyIncludedPair.ToString());
    }
}