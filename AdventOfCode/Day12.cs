using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private class Tile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Value { get; set; }

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
    }

    private readonly string _input;
    private readonly List<string> _lines;

    private Tile[,] _map;

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

        PrintCurrentMap(logger, true);

        var startNode = _map[startCoordinates.x, startCoordinates.y];
        var pathCost = GetOptimalPath(startNode, 0);

        return new("Solution");
    }

    private int GetOptimalPath(Tile node, int currentVisitedTiles)
    {
        if (node.IsEnd)
        {
            return 1;
        }

        return 1;
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("Solution");
    }

    private void PrintCurrentMap(LogWrapper wrapper, bool showPlayerAndEnd)
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                var tile = _map[i, j];
                if (tile.ContainsMe)
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