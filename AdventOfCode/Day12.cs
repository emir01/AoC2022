using System.Diagnostics;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private class Path
    {
        public List<Node> NodesInPath { get; set; }

        public bool EndReached { get; set; }

        // Determines from which specific node this path was created
        // Used to figure out where we came from and to not backtrack.
        // Avoid situations: S -> EAST -> WEST (S)
        public Node PathCreatedAtNode { get; set; }

        public Path(Node node, Node pathCreatedAtNode = null)
        {
            PathCreatedAtNode = pathCreatedAtNode;
            NodesInPath = new List<Node>();
            NodesInPath.Add(node);
        }

        public Path(Path extendedOnThisPath, Node addedThisNode, Node pathCreatedAtNode)
        {
            PathCreatedAtNode = pathCreatedAtNode;
            NodesInPath = new List<Node>();

            NodesInPath.AddRange(extendedOnThisPath.NodesInPath);
            NodesInPath.Add(addedThisNode);
        }
    }

    private class VisitedNodeRecord
    {
        public Node VisitedNode { get; set; }

        public int PathCountWhenVisited { get; set; }
    }

    private class Node
    {
        public int X { get; set; }

        public int Y { get; set; }

        private string _value;

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                Cost = GetCost(value[0]);
            }
        }

        public bool IsEnd { get; set; }

        public bool ContainsMe { get; set; }

        public int Cost { get; set; }

        public Node North { get; set; }
        public Node East { get; set; }
        public Node South { get; set; }
        public Node West { get; set; }

        public Node(int x, int y, string value)
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

        public bool CouldVisit(Node nodeToCheckIfCouldVisit, Path currentPath)
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

            if (nodeToCheckIfCouldVisit.Cost <= Cost)
            {
                return true;
            }

            if (nodeToCheckIfCouldVisit.Cost - Cost > 1)
            {
                return false;
            }

            return true;
        }

        public int Heuristic(Node node, (int x, int y) coordinatesOfEndNode)
        {
            // Given the Node where we are right now: node and the node where we want to go: this
            // and given the coordinates of the end node - calculate a heuristic value to determine the 
            // benefit of moving to this node

            var xDiffTargetNode = (coordinatesOfEndNode.x - X);
            var yDiffTargetNode = (coordinatesOfEndNode.y - Y);

            // var xDiffCurrentNode = Math.Abs(coordinatesOfEndNode.x - node.X);
            // var yDiffCurrentNode = Math.Abs(coordinatesOfEndNode.y - node.Y);

            // bigger numbers are going to be worse? 

            return xDiffTargetNode + yDiffTargetNode;
        }

        public List<Node> GetPotentialVisitNodes(Path path, List<VisitedNodeRecord> globallyVisitedTiles)
        {
            var visitableNodes = new List<Node>();
            if (CouldVisit(East, path))
            {
                //logger.WriteLine($"Starting from  [{X},{Y}] can go to EAST: [{East.X},{East.Y}]");
                visitableNodes.Add(East);
            }

            if (CouldVisit(West, path))
            {
                //logger.WriteLine($"Starting from  [{X},{Y}] can go to EAST: [{East.X},{East.Y}]");
                visitableNodes.Add(West);
            }

            if (CouldVisit(North, path))
            {
                //logger.WriteLine($"Starting from  [{X},{Y}] can go to EAST: [{East.X},{East.Y}]");
                visitableNodes.Add(North);
            }

            if (CouldVisit(South, path))
            {
                //logger.WriteLine($"Starting from  [{X},{Y}] can go to EAST: [{East.X},{East.Y}]");
                visitableNodes.Add(South);
            }

            return visitableNodes;
        }
    }

    private readonly string _input;
    private readonly List<string> _lines;

    private Node[,] _map;

    // We have to keep track of all visited Tiles - but also the path when they were visited
    // if on a next visit we are visiting with a lower cost path 
    // we can continue down the node.
    // otherwise there is no reason to check if our current path is longer in reaching that node.
    private List<VisitedNodeRecord> _globallyVisitedTiles = new();

    private int _rows;
    private int _columns;

    private (int x, int y) startCoordinates = (0, 0);

    private (int x, int y) endCoordinates = (0, 0);

    private long _nodesVisited = 0;

    public Day12()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        _rows = _lines.Count;
        _columns = _lines[0].Length;

        CreateMap();
    }


    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Solution");
    }


    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        // PrintCurrentMap(logger, true, true);

        var startNode = _map[startCoordinates.x, startCoordinates.y];

        logger.WriteLine($"End Coordinates: {endCoordinates}");

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
        _nodesVisited++;


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
                logger.WriteLine($"Stopping As Node Seen with better cost");
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

        // ===== NOTE: It might be good to determine which paths we can go to here that have some benefit
        // Also this can all be merged to find the nodes to visit and do a quick iteration 
        // instead of treating each one individually.

        var visitableNodes = node.GetPotentialVisitNodes(path, _globallyVisitedTiles);

        // we have to sort the visitable nodes by some score - 
        // how close they get us to the goal ?
        visitableNodes = visitableNodes.OrderBy(x => x.Heuristic(node, endCoordinates)).ToList();

        logger.WriteLine($"Looking at Node: [{node.X},{node.Y}]({node.Value})  with Count Visited: {_nodesVisited}");

        logger.WriteLine(
            "Ordered Visitable Nodes: " +
            $"{JsonSerializer.Serialize(visitableNodes.Select(visNode => new { visNode.X, visNode.Y, Heur = visNode.Heuristic(node, endCoordinates) }))}");


        logger.WriteLine("----");

        foreach (var visitableNode in visitableNodes)
        {
            var pathIncludingTheNewVisitableNode = new Path(path, visitableNode, node);

            // traverse that potential path
            var pathsBranchingFurtherFromEastNode = GetPaths(pathIncludingTheNewVisitableNode, logger);

            var pathsThatReachEndStartingFromThisVisitableNode =
                pathsBranchingFurtherFromEastNode.Where(x => x.EndReached).ToList();

            if (pathsThatReachEndStartingFromThisVisitableNode.Any())
            {
                var shortestDistanceTowardsEast =
                    pathsThatReachEndStartingFromThisVisitableNode.Min(x => x.NodesInPath.Count);

                var shortestPathTowardsEast =
                    pathsThatReachEndStartingFromThisVisitableNode.FirstOrDefault(x =>
                        x.NodesInPath.Count == shortestDistanceTowardsEast);

                pathsReachingTheEnd.Add(shortestPathTowardsEast);
                break;
            }
        }

        return pathsReachingTheEnd;
    }


    private void CreateMap()
    {
        _map = new Node[_rows, _columns];

        // parse the Map

        for (int i = 0; i < _rows; i++)
        {
            var line = _lines[i];

            for (int j = 0; j < line.Length; j++)
            {
                var currentTileValue = line[j].ToString();

                var tile = new Node(i, j, currentTileValue);

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
                    endCoordinates = (i, j);
                }

                _map[i, j] = tile;
            }
        }

        // Set 
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