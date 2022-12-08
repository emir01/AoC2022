using System.ComponentModel;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly string _input;

    private readonly List<string> _lines;

    private int rows;
    private int columns;
    private int[,] trees;

    public Day08()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        // Parse the trees in constructor to use for both solves
        rows = _lines.Count;
        columns = _lines[0].Length;

        trees = new int [rows, columns];

        // parsed the matrix
        for (var i = 0; i < _lines.Count; i++)
        {
            var line = _lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                trees[i, j] = Int32.Parse(line[j].ToString());
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper(false);
        var numberOfVisibleTrees = 0;

        logger.WriteLine("=======  PART 1 =======");

        numberOfVisibleTrees = rows * 2 + ((columns * 2) - 4);

        logger.WriteLine($"Trees visible on Edge: {numberOfVisibleTrees}");

        // iterate each 
        for (int i = 1; i < rows - 1; i++)
        {
            for (int j = 1; j < columns - 1; j++)
            {
                var currentTree = trees[i, j];
                (int west, int north, int east, int south) maxTreesInDirection =
                    GetMaxTreesInDirections(i, j);

                // check if there is at least one direction where the biggest tree is smaller than the current tree
                if (maxTreesInDirection.east < currentTree || maxTreesInDirection.west < currentTree ||
                    maxTreesInDirection.south < currentTree || maxTreesInDirection.north < currentTree)
                {
                    numberOfVisibleTrees++;
                    logger.WriteLine(
                        $"Tree at [{i},{j}] with height {currentTree} is visible with max directions: " +
                        $"{maxTreesInDirection.west}, {maxTreesInDirection.north}, {maxTreesInDirection.east}, {maxTreesInDirection.south}");
                }
            }
        }

        return new(numberOfVisibleTrees.ToString());
    }


    // Brute force solution with for loops
    private (int west, int north, int east, int south) GetMaxTreesInDirections(int currentTreeX,
        int currentTreeY)
    {
        (int west, int north, int east, int south) result = (0, 0, 0, 0);

        // check east/west
        int maxWest = 0,
            maxEast = 0;

        for (var j = 0; j < columns; j++)
        {
            // checking trees west of our current tree
            if (j < currentTreeY)
            {
                if (trees[currentTreeX, j] > maxWest)
                {
                    maxWest = trees[currentTreeX, j];
                }
            }

            if (j > currentTreeY)
            {
                if (trees[currentTreeX, j] > maxEast)
                {
                    maxEast = trees[currentTreeX, j];
                }
            }
        }

        int maxNorth = 0,
            maxSouth = 0;

        for (var i = 0; i < rows; i++)
        {
            // checking trees north of our current tree
            if (i < currentTreeX)
            {
                if (trees[i, currentTreeY] > maxNorth)
                {
                    maxNorth = trees[i, currentTreeY];
                }
            }

            // checking trees south of our current tree
            if (i > currentTreeX)
            {
                if (trees[i, currentTreeY] > maxSouth)
                {
                    maxSouth = trees[i, currentTreeY];
                }
            }
        }

        result.east = maxEast;
        result.south = maxSouth;
        result.west = maxWest;
        result.north = maxNorth;

        return result;
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);
        var size = 0;

        logger.WriteLine("=======  PART 2 =======");


        return new(size.ToString());
    }
}