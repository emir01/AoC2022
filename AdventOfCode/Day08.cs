using System.Text.Json;
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

        public int GetScenicScore()
        {
            var treesWest = Surroudings.WestTree.y >= 0 ? Y - Surroudings.WestTree.y : 1;
            var treesEast = Surroudings.EastTree.y >= 0 ? Surroudings.EastTree.y - Y : 1;

            var treesNorth = Surroudings.NorthTree.x >= 0 ? X - Surroudings.NorthTree.x : 1;
            var treesSouth = Surroudings.SouthTree.x >= 0 ? Surroudings.SouthTree.x - X : 1;

            var score = treesEast * treesNorth * treesWest * treesSouth;

            Console.WriteLine(
                $"Scenic Score Components for Tree [{X}, {Y}]: West: {treesWest}, North: {treesNorth}, East: {treesEast}, South: {treesSouth} for a Score: {score}");

            return score;
        }
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
        public List<Tree> InternalOptimalTrees { get; set; }
        public List<Tree> OptimalEdgeTrees { get; set; }

        public TreeScanResults()
        {
            InternalOptimalTrees = new List<Tree>();
            OptimalEdgeTrees = new List<Tree>();
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

        return new((results.InternalOptimalTrees.Count + results.OptimalEdgeTrees.Count).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("=======  PART 2 =======");

        var results = ScanTrees(logger);

        // calculate the scenic score for each optimal tree and get the max score
        var scenicScores = results.InternalOptimalTrees.Select(x => x.GetScenicScore()).ToList();

        logger.WriteLine(JsonSerializer.Serialize(scenicScores));

        return new(scenicScores.Max().ToString());
    }

    /// <summary>
    /// We need to scan ALL trees - including those in the edges to figure out their surroundings and see
    /// how much other trees they can see  - to find the Scenic Score
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    private TreeScanResults ScanTrees(LogWrapper logger)
    {
        TreeScanResults results = new TreeScanResults();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var currentTree = trees[i, j];

                var treeSurroundings = GetTreeSurroundings(i, j);
                (int west, int north, int east, int south) maxTreesInDirection =
                    treeSurroundings.MaxTreesInDirection;

                // check if there is at least one direction where the biggest tree is smaller than the current tree
                if (maxTreesInDirection.east < currentTree || maxTreesInDirection.west < currentTree ||
                    maxTreesInDirection.south < currentTree || maxTreesInDirection.north < currentTree)
                {
                    // quick fix to separate between edge and internal optimal trees
                    // after spending time to have this work with edge trees as well.
                    if (i != 0 && i < rows - 1 && j > 0 && j < columns - 1)
                    {
                        results.InternalOptimalTrees.Add(new Tree
                        {
                            Height = currentTree, X = i, Y = j, Surroudings = treeSurroundings
                        });
                    }
                    else
                    {
                        results.OptimalEdgeTrees.Add(new Tree
                        {
                            Height = currentTree, X = i, Y = j, Surroudings = treeSurroundings
                        });
                    }

                    logger.WriteLine(
                        $"Tree at [{i},{j}] with height {currentTree} is visible with max directions: " +
                        $"{maxTreesInDirection.west}, {maxTreesInDirection.north}, {maxTreesInDirection.east}, {maxTreesInDirection.south}");
                }
            }
        }

        return results;
    }


    private TreeSurroudings GetTreeSurroundings(int currentTreeX,
        int currentTreeY)
    {
        // as we are now scanning all trees currentTreeX and Y could be on the edge

        var result = new TreeSurroudings();

        // check east/west and keep track of the Max Tree Index for the Current Tree
        // Can be used for calculating Scenic Scores? 
        int maxWest = -1,
            maxEast = -1;

        (int, int) maxWestCords = (-1, -1);
        (int, int) maxEastCords = (-1, -1);

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

        int maxNorth = -1,
            maxSouth = -1;

        (int, int) maxNorthCords = (-1, -1);
        (int, int) maxSouthCords = (-1, -1);

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