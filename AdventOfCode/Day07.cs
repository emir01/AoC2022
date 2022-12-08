using System.ComponentModel;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day07 : BaseDay
{
    private static class OsConstants
    {
        public const string CommandPrefix = "$";
        public const string DirectoryPrefix = "dir";

        public const string DirectoryUp = "..";

        public const string Command_ChangeDirectory = "cd";

        public const int SizeTotal = 70000000;
        public const int SizeUpdateNeeded = 30000000;
        public const int SizeMaxFolder = 100000;
    }

    public enum OsNodeType
    {
        FOLDER,
        FILE
    }

    private class OsNode
    {
        public string Name { get; set; }

        public OsNodeType Type { get; set; }

        public int Size { get; set; }

        public List<OsNode> Children { get; set; }

        [JsonIgnore] public OsNode Parent { get; set; }

        public OsNode()
        {
            Children = new List<OsNode>();
            Parent = null;
        }

        public void AddFolder(string lineDirectoryName)
        {
            Children.Add(new OsNode()
            {
                Type = OsNodeType.FOLDER,
                Name = lineDirectoryName,
                Parent = this
            });
        }

        public void AddFile(string lineFileName, int lineSize)
        {
            Children.Add(new OsNode()
            {
                Type = OsNodeType.FILE,
                Name = lineFileName,
                Size = lineSize,
                Parent = this
            });
        }
    }

    private class TerminalLine
    {
        public string Line { get; set; }

        public bool IsCommand { get; set; }

        public string Command { get; set; }
        public string Args { get; set; }

        public int Size { get; set; }

        public string DirectoryName { get; set; }

        public string FileName { get; set; }

        public TerminalLine Parse(string input)
        {
            Line = input;
            if (input.StartsWith(OsConstants.CommandPrefix))
            {
                IsCommand = true;
                var splitCommand = input.Split(" ");
                Command = splitCommand[1];
                if (splitCommand.Length == 3)
                {
                    Args = splitCommand[2];
                }
            }

            // not a command so either a "dir folder" or "size fileName"

            else
            {
                IsCommand = false;

                if (Line.StartsWith(OsConstants.DirectoryPrefix))
                {
                    DirectoryName = Line.Split(" ")[1];
                }
                else
                {
                    var splitFileName = Line.Split(" ");
                    Size = Int32.Parse(splitFileName[0]);
                    FileName = splitFileName[1];
                }
            }

            return this;
        }

        public bool IsDirectory()
        {
            return !string.IsNullOrWhiteSpace(DirectoryName);
        }

        public override string ToString()
        {
            return Line;
        }
    }

    private readonly string _input;

    /*
     *  
     */

    private OsNode _osRoot;

    private List<int> _totalFolderSizes;

    public Day07()
    {
        _input = File.ReadAllText(InputFilePath);

        // Create our Data Structures Here and have Solve01 and 02 run the final calcualtions
        var lines = _input.Split(Constants.NEW_LINE);

        /*
         *
         * 1. Parse our Input into Terminal Lines representing the Actions, Output, Data.
         * 
         */
        var terminalLines = lines.Select(x => new TerminalLine().Parse(x)).ToList();

        // process the data here

        _osRoot = BuildOsRoot(terminalLines);

        /*
         *
         * 3. Calculate Folder Sizes
         * We can do this while also keeping track of the
         * total sizes of folders as we come back from the internal calculations as we visit each folder.
         * Request is to output sum of sizes without folders - not knowing folder names should work for now
         */

        _totalFolderSizes = new List<int>();

        CalcualteAndTrackFolderNodeSizes(_osRoot, _totalFolderSizes);
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper(false);
        var size = 0;

        logger.WriteLine("=======  PART 1 =======");

        size = _totalFolderSizes.Where(x => x <= OsConstants.SizeMaxFolder).Sum();

        return new(size.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);
        var minFolderSizeToDeleteToUpdate = 0;

        logger.WriteLine("=======  PART 2 =======");

        var freeSpace = OsConstants.SizeTotal - _osRoot.Size;
        var minNeeded = OsConstants.SizeUpdateNeeded - freeSpace;

        logger.WriteLine($"Min Needed: {minNeeded}");
        logger.WriteLine(JsonSerializer.Serialize(_totalFolderSizes));

        minFolderSizeToDeleteToUpdate = _totalFolderSizes.Where(x => x >= minNeeded).Min();

        return new(minFolderSizeToDeleteToUpdate.ToString());
    }

    private int CalcualteAndTrackFolderNodeSizes(OsNode node, List<int> sizes)
    {
        var calculatedSize = 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];

            if (child.Type == OsNodeType.FILE)
            {
                calculatedSize += child.Size;
            }
            else
            {
                var childFolderSize = CalcualteAndTrackFolderNodeSizes(child, sizes);
                calculatedSize += childFolderSize;
            }
        }

        // here we finished calculating the total folder size so we can also track it in sizes
        sizes.Add(calculatedSize);
        node.Size = calculatedSize;

        return calculatedSize;
    }

    private static OsNode BuildOsRoot(List<TerminalLine> terminalLines)
    {
        var osRoot = new OsNode()
        {
            Name = "/"
        };

        var activeNode = osRoot;

        for (int i = 2; i < terminalLines.Count; i++)
        {
            var line = terminalLines[i];

            // Dealing with a Listed Item (either file or folder)
            if (!line.IsCommand)
            {
                if (line.IsDirectory())
                {
                    activeNode.AddFolder(line.DirectoryName);
                }
                else
                {
                    activeNode.AddFile(line.FileName, line.Size);
                }
            }
            else
            {
                if (line.Command.Equals(OsConstants.Command_ChangeDirectory))
                {
                    // change the active node to the child
                    if (line.Args != OsConstants.DirectoryUp)
                    {
                        activeNode = activeNode.Children.First(x => x.Name.Equals(line.Args));
                    }

                    else
                    {
                        activeNode = activeNode.Parent;
                    }
                }
            }
        }

        return osRoot;
    }
}