using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day12 : BaseDay
{
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

        public bool CouldVisit(Node nodeToCheckIfCouldVisit)
        {
            if (nodeToCheckIfCouldVisit == null)
            {
                return false;
            }

            if (nodeToCheckIfCouldVisit.Cost <= Cost)
            {
                return true;
            }

            return nodeToCheckIfCouldVisit.Cost - Cost <= 1;
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


        public List<Node> VisitableNodes()
        {
            var visitableNode = new List<Node>();

            if (CouldVisit(East))
            {
                visitableNode.Add(East);
            }

            if (CouldVisit(West))
            {
                visitableNode.Add(West);
            }

            if (CouldVisit(North))
            {
                visitableNode.Add(North);
            }

            if (CouldVisit(South))
            {
                visitableNode.Add(South);
            }

            return visitableNode;
        }
    }

    private readonly string _input;
    private readonly List<string> _lines;

    private Node[,] _map;

    private int _rows;
    private int _columns;

    private (int x, int y) originalStartCoords = (0, 0);

    private (int x, int y) endCoordinates = (0, 0);

    private long _nodesVisited = 0;

    private List<Node> _lowNodes = new List<Node>();

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
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 2 =====");

        // PrintCurrentMap(logger, true, true);

        var pathValues = new List<int>();

        foreach (var lowNode in _lowNodes)
        {
            var path = GetPathFromStartNode(lowNode);

            if (path.Count > 0)
            {
                pathValues.Add(path.Count - 1);
            }

            logger.WriteLine($"Calculated path from Start Node: [{lowNode.X},{lowNode.Y}] with Steps:{path.Count - 1}");
        }


        var minPath = pathValues.Min();

        return new(minPath.ToString());
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        // PrintCurrentMap(logger, true, true);

        var startNode = _map[originalStartCoords.x, originalStartCoords.y];

        var pathFromEnd = GetPathFromStartNode(startNode);

        return new((pathFromEnd.Count - 1).ToString());
    }

    private static List<Node> GetPathFromStartNode(Node startNode)
    {
        var frontier = new Queue<Node>();
        var visited = new Dictionary<Node, Node>();

        frontier.Enqueue(startNode);
        visited[startNode] = null;

        Node endNode = null;

        // while we have frontier nodes
        while (frontier.TryPeek(out _))
        {
            // get this current frontier node
            var currentFrontierNode = frontier.Dequeue();

            // check if this is the end node
            if (currentFrontierNode.IsEnd)
            {
                endNode = currentFrontierNode;
                break;
            }

            var visitableNodes = currentFrontierNode.VisitableNodes();

            foreach (var node in visitableNodes)
            {
                if (!visited.ContainsKey(node))
                {
                    frontier.Enqueue(node);


                    visited.Add(node, currentFrontierNode);
                }
            }
        }

        var pathFromEnd = new List<Node>();

        if (endNode != null)
        {
            var currentNode = endNode;
            pathFromEnd.Add(currentNode);

            while (visited.ContainsKey(currentNode) && visited[currentNode] != null)
            {
                pathFromEnd.Add(visited[currentNode]);
                currentNode = visited[currentNode];
            }
        }

        return pathFromEnd;
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

                var node = new Node(i, j, currentTileValue);

                if (i > 0)
                {
                    node.North = _map[i - 1, j];
                }

                if (i < _rows - 1)
                {
                    node.South = _map[i + 1, j];
                }

                if (j > 0)
                {
                    node.West = _map[i, j - 1];
                }

                if (j < _columns - 1)
                {
                    node.East = _map[i, j + 1];
                }

                if (currentTileValue == "S")
                {
                    // tile value is actually a
                    node.Value = "a";
                    node.ContainsMe = true;
                    originalStartCoords = (i, j);
                }
                else if (currentTileValue == "E")
                {
                    node.Value = "z";
                    node.IsEnd = true;
                    endCoordinates = (i, j);
                }

                _map[i, j] = node;

                if (node.Value.Equals("a"))
                {
                    _lowNodes.Add(node);
                }
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