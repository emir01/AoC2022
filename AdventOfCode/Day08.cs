using System.Data;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    /// <summary>
    /// Small utility class to keep track of visible trees and some of their attributes. 
    /// </summary>
    private class Tree
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }

        public TreeSurroudings Surroudings { get; set; }
    }


    private class TreeSurroudings
    {
        public (int west, int north, int east, int south) MaxTreesInDirection { get; set; }

        public (int x, int y) WestTree { get; set; }
        public (int x, int y) NorthTree { get; set; }
        public (int x, int y) EastTree { get; set; }
        public (int x, int y) SouthTree { get; set; }

        public void SetMaxTreeValueInDirections(int maxEast, int maxSouth, int maxWest, int maxNorth)
        {
            MaxTreesInDirection = (maxWest, maxNorth, maxEast, maxSouth);
        }
    }

    private class TreeScanResults
    {
        public int VisibleTrees { get; set; }

        public List<Tree> OptimalInternalTrees { get; set; }

        public TreeScanResults()
        {
            OptimalInternalTrees = new List<Tree>();
        }
    }

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

        logger.WriteLine("=======  PART 1 =======");

        var results = ScanTrees(logger);

        return new(results.VisibleTrees.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("=======  PART 2 =======");

        var results = ScanTrees(logger);

        return new(results.VisibleTrees.ToString());
    }

    private TreeScanResults ScanTrees(LogWrapper logger)
    {
        var numberOfVisibleTreesOnEdges = rows * 2 + ((columns * 2) - 4);
        TreeScanResults results = new TreeScanResults()
        {
            VisibleTrees = numberOfVisibleTreesOnEdges
        };

        logger.WriteLine($"Trees visible on Edge: {results.VisibleTrees}");

        for (int i = 1; i < rows - 1; i++)
        {
            for (int j = 1; j < columns - 1; j++)
            {
                var currentTree = trees[i, j];

                var treeSurroundings = GetTreeSurroundings(i, j);
                (int west, int north, int east, int south) maxTreesInDirection =
                    treeSurroundings.MaxTreesInDirection;

                // check if there is at least one direction where the biggest tree is smaller than the current tree
                if (maxTreesInDirection.east < currentTree || maxTreesInDirection.west < currentTree ||
                    maxTreesInDirection.south < currentTree || maxTreesInDirection.north < currentTree)
                {
                    results.VisibleTrees++;

                    results.OptimalInternalTrees.Add(new Tree
                    {
                        Height = currentTree, X = i, Y = j, Surroudings = treeSurroundings
                    });

                    logger.WriteLine(
                        $"Tree at [{i},{j}] with height {currentTree} is visible with max directions: " +
                        $"{maxTreesInDirection.west}, {maxTreesInDirection.north}, {maxTreesInDirection.east}, {maxTreesInDirection.south}");
                }
            }
        }

        return results;
    }


    // Brute force solution with for loops
    private TreeSurroudings GetTreeSurroundings(int currentTreeX,
        int currentTreeY)
    {
        var result = new TreeSurroudings();

        // check east/west and keep track of the Max Tree Index for the Current Tree
        // Can be used for calculating Scenic Scores? 
        int maxWest = 0,
            maxEast = 0;

        (int, int) maxWestCords = (0, 0);
        (int, int) maxEastCords = (0, 0);

        for (var j = 0; j < columns; j++)
        {
            // checking trees west of our current tree
            if (j < currentTreeY)
            {
                if (trees[currentTreeX, j] > maxWest)
                {
                    maxWest = trees[currentTreeX, j];
                    maxWestCords = (currentTreeX, j);
                }
            }

            if (j > currentTreeY)
            {
                if (trees[currentTreeX, j] > maxEast)
                {
                    maxEast = trees[currentTreeX, j];
                    maxEastCords = (currentTreeX, j);
                }
            }
        }

        int maxNorth = 0,
            maxSouth = 0;

        (int, int) maxNorthCords = (0, 0);
        (int, int) maxSouthCords = (0, 0);

        for (var i = 0; i < rows; i++)
        {
            // checking trees north of our current tree
            if (i < currentTreeX)
            {
                if (trees[i, currentTreeY] > maxNorth)
                {
                    maxNorth = trees[i, currentTreeY];
                    maxNorthCords = (i, currentTreeY);
                }
            }

            // checking trees south of our current tree
            if (i > currentTreeX)
            {
                if (trees[i, currentTreeY] > maxSouth)
                {
                    maxSouth = trees[i, currentTreeY];
                    maxSouthCords = (i, currentTreeY);
                }
            }
        }

        result.SetMaxTreeValueInDirections(maxEast, maxSouth, maxWest, maxNorth);

        result.EastTree = maxEastCords;
        result.WestTree = maxWestCords;
        result.NorthTree = maxNorthCords;
        result.SouthTree = maxSouthCords;

        return result;
    }
}