using System.Collections;
using System.Net.Mime;
using System.Text.Json;
using AdventOfCode.Utils;

namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private class Message
    {
        public List<PacketPair> PacketPairs;

        public Message()
        {
            PacketPairs = new List<PacketPair>();
        }
    }

    private class PacketPair
    {
        public Packet Left { get; set; }

        public Packet Right { get; set; }

        public PacketPair(string left, string right)
        {
            Left = new Packet(left);
            Right = new Packet(right);
        }
    }

    private class Packet
    {
        public string RawPacket { get; set; }

        public PacketElement PacketElement { get; set; }

        public Packet(string input)
        {
            RawPacket = input;
            PacketElement = PacketElement.GetPacketElement(RawPacket);
        }
    }

    private abstract class PacketElement
    {
        public static PacketElement GetPacketElement(string element)
        {
            // dealing with an array element
            if (element.StartsWith("["))
            {
                var elementWithoutBrackets = element.Substring(1, element.Length - 2);

                // split te elementsWithoutBrackets - Cannot use Split(",") as
                // there might be internal arrays

                List<string> topSplits = new List<string>();

                // how would I Parse this
                // [1],[2,3,4]
                var bracketStack = new Stack<int>();
                var segment = "";
                for (int i = 0; i < elementWithoutBrackets.Length; i++)
                {
                    var activeCharacter = elementWithoutBrackets[i].ToString();

                    if (activeCharacter == "[")
                    {
                        bracketStack.Push(i);
                        segment += activeCharacter;
                        continue;
                    }

                    if (activeCharacter == "]")
                    {
                        bracketStack.Pop();
                        segment += activeCharacter;

                        // if we've reached the end of the elementWithoutBrackets 
                        if (i == elementWithoutBrackets.Length - 1)
                        {
                            topSplits.Add(segment);
                        }

                        continue;
                    }

                    if (activeCharacter == ",")
                    {
                        // only if the stack is empty do we
                        if (!bracketStack.TryPeek(out int _))
                        {
                            topSplits.Add(segment);
                            segment = "";
                            continue;
                        }
                    }

                    segment += activeCharacter;

                    if (i == elementWithoutBrackets.Length - 1)
                    {
                        topSplits.Add(segment);
                    }
                }

                return new ArrayPacketElement(topSplits);
            }
            else
            {
                // dealing with a number
                var number = int.Parse(element);
                return new NumberElement(number);
            }
        }
    }

    private class ArrayPacketElement : PacketElement
    {
        public ArrayPacketElement(IList<string> elements)
        {
            Value = new List<PacketElement>();

            foreach (var element in elements)
            {
                Value.Add(GetPacketElement(element));
            }
        }

        public List<PacketElement> Value { get; set; }
    }

    private class NumberElement : PacketElement
    {
        public NumberElement(int number)
        {
            Value = number;
        }

        public int Value { get; set; }
    }


    private Message _message;

    public Day13()
    {
        _input = File.ReadAllText(InputFilePath);
        _lines = _input.Split(Constants.NEW_LINE).ToList();

        Console.WriteLine(JsonSerializer.Serialize(_lines));

        ConstructMessage();
    }

    private void ConstructMessage()
    {
        _message = new Message();

        for (int i = 0; i < _lines.Count; i += 3)
        {
            var one = _lines[i];
            var two = _lines[i + 1];

            _message.PacketPairs.Add(new PacketPair(one, two));
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // process the input string
        var logger = new LogWrapper();

        logger.WriteLine("===== PART 1 =====");

        return new("");
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("");
    }
}