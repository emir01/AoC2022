using System.Diagnostics;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private class Path
    {
        public List<Tile> NodesInPath { get; set; }

        public bool EndReached { get; set; }

        // Determines from which specific node this path was created
        // Used to figure out where we came from and to not backtrack.
        // Avoid situations: S -> EAST -> WEST (S)
        public Tile PathCreatedAtNode { get; set; }

        public Path(Tile node, Tile pathCreatedAtNode = null)
        {
            PathCreatedAtNode = pathCreatedAtNode;
            NodesInPath = new List<Tile>();
            NodesInPath.Add(node);
        }

        public Path(Path extendedOnThisPath, Tile addedThisNode, Tile pathCreatedAtNode)
        {
            PathCreatedAtNode = pathCreatedAtNode;
            NodesInPath = new List<Tile>();

            NodesInPath.AddRange(extendedOnThisPath.NodesInPath);
            NodesInPath.Add(addedThisNode);
        }
    }

    private class VisitedNodeRecord
    {
        public Tile VisitedNode { get; set; }

        public int PathCountWhenVisited { get; set; }
    }

    private class Tile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Value
        {
            get => Value;
            set { Cost = GetCost(value[0]); }
        }

        public bool IsEnd { get; set; }

        public bool ContainsMe { get; set; }

        public int Cost { get; set; }

        public Tile North { get; set; }
        public Tile East { get; set; }
        public Tile South { get; set; }
        public Tile West { get; set; }

        public Tile(int x, int y, string value)
        {
            X = x;
            Y = y;
            Value = value;
            Cost = GetCost(value[0]);
        }

        private int GetCost(char c)
        {
            return ((int)c) - 96;
        }

        public bool CouldVisit(Tile nodeToCheckIfCouldVisit, Path currentPath,
            List<VisitedNodeRecord> visitedTiles = null)
        {
            if (nodeToCheckIfCouldVisit == null)
            {
                return false;
            }

            // prevents backtrack and cycle
            if (currentPath.NodesInPath.Any(x => x == nodeToCheckIfCouldVisit))
            {
                return false;
            }

            // if we have visited the node we are checking previously we don't have to go down that path again
            // as a route could already have been found.
            // if (visitedTiles != null && visitedTiles.Any(x => x.VisitedNode == nodeToCheckIfCouldVisit))
            // {
            //     return false;
            // }

            var costDifference = Math.Abs(Cost - nodeToCheckIfCouldVisit.Cost);
            if (costDifference == 1 || costDifference == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private readonly string _input;
    private readonly List<string> _lines;

    private Tile[,] _map;

    // We have to keep track of all visited Tiles - but also the path when they were visited
    // if on a next visit we are visiting with a lower cost path 
    // we can continue down the node.
    // otherwise there is no reason to check if our current path is longer in reaching that node.
    private List<VisitedNodeRecord> _globallyVisitedTiles =
        new List<VisitedNodeRecord>();

    private int _rows;
    private int _columns;

    private (int x, int y) startCoordinates = (0, 0);

    private (int x, int y) finishCoordinates = (0, 0);

    public Day12()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        _rows = _lines.Count;
        _columns = _lines[0].Length;

        CreateMap();
    }

    private void CreateMap()
    {
        _map = new Tile[_rows, _columns];

        // parse the Map

        for (int i = 0; i < _rows; i++)
        {
            var line = _lines[i];

            for (int j = 0; j < line.Length; j++)
            {
                var currentTileValue = line[j].ToString();

                var tile = new Tile(i, j, currentTileValue);

                if (i > 0)
                {
                    tile.North = _map[i - 1, j];
                }

                if (i < _rows - 1)
                {
                    tile.South = _map[i + 1, j];
                }

                if (j > 0)
                {
                    tile.West = _map[i, j - 1];
                }

                if (j < _columns - 1)
                {
                    tile.East = _map[i, j + 1];
                }

                if (currentTileValue == "S")
                {
                    // tile value is actually a
                    tile.Value = "a";
                    tile.ContainsMe = true;
                    startCoordinates = (i, j);
                }
                else if (currentTileValue == "E")
                {
                    tile.Value = "z";
                    tile.IsEnd = true;
                    finishCoordinates = (i, j);
                }

                _map[i, j] = tile;
            }
        }

        // Set Neighbords
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var tile = _map[i, j];

                if (i > 0)
                {
                    tile.North = _map[i - 1, j];
                }

                if (i < _rows - 1)
                {
                    tile.South = _map[i + 1, j];
                }

                if (j > 0)
                {
                    tile.West = _map[i, j - 1];
                }

                if (j < _columns - 1)
                {
                    tile.East = _map[i, j + 1];
                }
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        // PrintCurrentMap(logger, true, true);

        var startNode = _map[startCoordinates.x, startCoordinates.y];

        var pathStartingFromStartNode = new Path(startNode);

        // return the paths from the starting node that reach the end.
        // starting from a given node (let's say in the center) we can have up to 4 paths (E,W,N,S)
        // that reach the end.
        // Each of those could have a given number of nodes they travers
        // solution will probably be to pick the lowest numbered traverse
        var pathsToEnd = GetPaths(pathStartingFromStartNode, logger);

        foreach (var path in pathsToEnd)
        {
            logger.WriteLine(
                $"There is a Path from the Start - First Heading Towards: [{path.NodesInPath[1].X}, {path.NodesInPath[1].Y}] that reaches the end in {path.NodesInPath.Count - 1} steps");

            logger.WriteLine($"===  PRINTING PATH COORDS ====");

            for (int i = 0; i < path.NodesInPath.Count; i++)
            {
                logger.WriteLine($"[{path.NodesInPath[i].X},{path.NodesInPath[i].Y}]");
            }
        }

        var bestPathNodeCount = pathsToEnd.Min(x => x.NodesInPath.Count);
        var bestPath = pathsToEnd.FirstOrDefault(x => x.NodesInPath.Count == bestPathNodeCount);

        var solution = "Solution";
        if (bestPath != null)
        {
            solution = (bestPath.NodesInPath.Count - 1).ToString();
        }

        return new(solution);
    }

    private List<Path> GetPaths(Path path, LogWrapper logger)
    {
        // the last node in the current path
        // we inspect all the paths that can originate from this node
        var node = path.NodesInPath.Last();

        var pathsReachingTheEnd = new List<Path>();

        // hitting the node for the first time
        var previousNodeVisit = _globallyVisitedTiles.FirstOrDefault(x => x.VisitedNode == node);
        if (previousNodeVisit == null)
        {
            _globallyVisitedTiles.Add(new VisitedNodeRecord()
            {
                VisitedNode = node,
                PathCountWhenVisited = path.NodesInPath.Count
            });
        }
        else
        {
            // we have visited this node before with some path which had lower cost
            // from our current. Our current can never be better starting from that same node
            if (previousNodeVisit.PathCountWhenVisited < path.NodesInPath.Count)
            {
                // so we don't have to process further
                return pathsReachingTheEnd;
            }

            // if our current path is better up to the point we replace visited record and continue exploring
            _globallyVisitedTiles.Remove(previousNodeVisit);
            _globallyVisitedTiles.Add(new VisitedNodeRecord()
            {
                VisitedNode = node,
                PathCountWhenVisited = path.NodesInPath.Count
            });
        }


        // if this last node is an end node 
        // we don't have to explore further so we return the List with a single path:the current one
        if (node.IsEnd)
        {
            path.EndReached = true;
            pathsReachingTheEnd.Add(path);
            return pathsReachingTheEnd;
        }

        // we are now going to check if our current node can expand in any direction
        // to somehow reach the end

        // check if we can go East at all (based on cost/height)
        if (node.CouldVisit(node.East, path, _globallyVisitedTiles))
        {
            //logger.WriteLine($"Starting from  [{node.X},{node.Y}] can go to EAST: [{node.East.X},{node.East.Y}]");

            // create a new path that will now also additionally include the east node  
            var pathIncludingTheEastNode = new Path(path, node.East, node);

            // traverse that potential path
            var pathsBranchingFurtherFromEastNode = GetPaths(pathIncludingTheEastNode, logger);

            var pathsThatReachedTheEndGoingEast = pathsBranchingFurtherFromEastNode.Where(x => x.EndReached).ToList();

            if (pathsThatReachedTheEndGoingEast.Any())
            {
                var shortestDistanceTowardsEast = pathsThatReachedTheEndGoingEast.Min(x => x.NodesInPath.Count);

                var shortestPathTowardsEast =
                    pathsThatReachedTheEndGoingEast.FirstOrDefault(x =>
                        x.NodesInPath.Count == shortestDistanceTowardsEast);

                pathsReachingTheEnd.Add(shortestPathTowardsEast);
            }
        }

        if (node.CouldVisit(node.West, path, _globallyVisitedTiles))
        {
            // logger.WriteLine($"Starting from  [{node.X},{node.Y}] can go to WEST [{node.West.X},{node.West.Y}]");

            // create a new path that will now also additionally include the east node  
            var pathIncludingTheWestNode = new Path(path, node.West, node);

            // traverse that potential path
            var pathsBranchingFurtherFromWestNode = GetPaths(pathIncludingTheWestNode, logger);

            var pathsThatReachedTheEndGoingWest = pathsBranchingFurtherFromWestNode.Where(x => x.EndReached).ToList();

            if (pathsThatReachedTheEndGoingWest.Any())
            {
                var shortestDistanceTowardsWest = pathsThatReachedTheEndGoingWest.Min(x => x.NodesInPath.Count);

                var shortestPathTowardsWest =
                    pathsThatReachedTheEndGoingWest.FirstOrDefault(x =>
                        x.NodesInPath.Count == shortestDistanceTowardsWest);

                pathsReachingTheEnd.Add(shortestPathTowardsWest);
            }
        }

        if (node.CouldVisit(node.North, path, _globallyVisitedTiles))
        {
            // logger.WriteLine($"Starting from  [{node.X},{node.Y}] can go to NORTH [{node.North.X},{node.North.Y}]");

            // create a new path that will now also additionally include the east node  
            var pathIncludingTheNorthNode = new Path(path, node.North, node);

            // traverse that potential path
            var pathsBranchingFurtherFromNorthNode = GetPaths(pathIncludingTheNorthNode, logger);

            var pathsThatReachedTheEndGoingNorth = pathsBranchingFurtherFromNorthNode.Where(x => x.EndReached).ToList();

            if (pathsThatReachedTheEndGoingNorth.Any())
            {
                var shortestDistanceTowardsNorth = pathsThatReachedTheEndGoingNorth.Min(x => x.NodesInPath.Count);

                var shortestPathTowardsNorth =
                    pathsThatReachedTheEndGoingNorth.FirstOrDefault(x =>
                        x.NodesInPath.Count == shortestDistanceTowardsNorth);

                pathsReachingTheEnd.Add(shortestPathTowardsNorth);
            }
        }

        if (node.CouldVisit(node.South, path, _globallyVisitedTiles))
        {
            // logger.WriteLine($"Starting from  [{node.X},{node.Y}] can go to SOUTH [{node.South.X},{node.South.Y}]");

            // create a new path that will now also additionally include the east node  
            var pathIncludingTheSouthNode = new Path(path, node.South, node);

            // traverse that potential path
            var pathsBranchingFurtherFromSouthNode = GetPaths(pathIncludingTheSouthNode, logger);

            var pathsThatReachedTheEndGoingSouth = pathsBranchingFurtherFromSouthNode.Where(x => x.EndReached).ToList();

            if (pathsThatReachedTheEndGoingSouth.Any())
            {
                var shortestDistanceTowardsSouth = pathsThatReachedTheEndGoingSouth.Min(x => x.NodesInPath.Count);

                var shortestPathTowardsSouth =
                    pathsThatReachedTheEndGoingSouth.FirstOrDefault(x =>
                        x.NodesInPath.Count == shortestDistanceTowardsSouth);

                pathsReachingTheEnd.Add(shortestPathTowardsSouth);
            }
        }

        return pathsReachingTheEnd;
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Solution");
    }

    private void PrintCurrentMap(LogWrapper wrapper, bool showPlayerAndEnd = false, bool showCosts = false)
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var tile = _map[i, j];

                if (showCosts)
                {
                    wrapper.Write($" [{tile.Cost}] ");
                    if (tile.Cost.ToString().Length == 1)
                    {
                        wrapper.Write(" ");
                    }

                    continue;
                }

                if (tile.ContainsMe && showPlayerAndEnd)
                {
                    wrapper.Write("P");
                    continue;
                }

                if (tile.IsEnd)
                {
                    wrapper.Write("E");
                    continue;
                }

                wrapper.Write(tile.Value);
            }

            wrapper.WriteLine("");
        }
    }
}