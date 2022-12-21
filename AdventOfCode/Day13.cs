using System.Collections;
using System.Net.Mime;
using System.Text.Json;
using AdventOfCode.Utils;
using Spectre.Console;

namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly string _input;
    private readonly List<string> _lines;

    private enum CompareStatus
    {
        VALID,
        EQUAL,
        BAD
    }

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

                return new ArrayElement(topSplits);
            }
            else
            {
                // dealing with a number
                var number = int.Parse(element);
                return new NumberElement(number);
            }
        }
    }

    private class ArrayElement : PacketElement
    {
        public ArrayElement(IList<string> elements)
        {
            Value = new Queue<PacketElement>();

            foreach (var element in elements)
            {
                Value.Enqueue(GetPacketElement(element));
            }
        }

        public Queue<PacketElement> Value { get; set; }
    }

    private class NumberElement : PacketElement
    {
        public NumberElement(int number)
        {
            Value = number;
        }

        public int Value { get; set; }

        public ArrayElement ToArrayElement()
        {
            return new ArrayElement(new List<string>()
            {
                Value.ToString()
            });
        }
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
        logger.Delay = 0;

        logger.WriteLine("===== PART 1 =====");

        List<int> validPairIndexes = new List<int>();

        // Do the message comparison
        foreach (var packetPair in _message.PacketPairs)
        {
            // compare elements in each pair
            var left = packetPair.Left;
            var right = packetPair.Right;

            // start comparing the top elements

            // start comparing the two Lists
            if (ComparePacketElements(left.PacketElement, right.PacketElement, logger) == CompareStatus.VALID)
            {
                validPairIndexes.Add(_message.PacketPairs.IndexOf(packetPair) + 1);
            }
        }

        logger.WriteLine($"Valid Pair Indexes: {JsonSerializer.Serialize(validPairIndexes)}");

        return new(validPairIndexes.Sum().ToString());
    }

    private CompareStatus ComparePacketElements(PacketElement leftPacketElement, PacketElement rightPacketElement,
        LogWrapper logger)
    {
        logger.WriteLine("======= Comparing Packet Elements =======");

        if (leftPacketElement is NumberElement leftPacketElementAsNumber &&
            rightPacketElement is NumberElement rightPacketElementAsNumber)
        {
            logger.WriteLine(
                $"Comparing two Number Elements: " +
                $"Left: {leftPacketElementAsNumber.Value} -- " +
                $"Right: {rightPacketElementAsNumber.Value}");

            if (leftPacketElementAsNumber.Value < rightPacketElementAsNumber.Value)
            {
                logger.WriteLine($"Left SMALLER than Right - Returning VALID");
                return CompareStatus.VALID;
            }

            if (leftPacketElementAsNumber.Value == rightPacketElementAsNumber.Value)
            {
                logger.WriteLine($"Left EQUALS Right - Returning EQUAL");
                return CompareStatus.EQUAL;
            }

            logger.WriteLine($"Left GREATER than Right - Returning BAD");
            return CompareStatus.BAD;
        }

        if (leftPacketElement is ArrayElement leftPacketElementAsArray &&
            rightPacketElement is ArrayElement rightPacketElementAsArray)
        {
            logger.WriteLine($"Comparing TWO Array Elements");

            // while The Left Element Has 
            while (leftPacketElementAsArray.Value.TryDequeue(out PacketElement left))
            {
                logger.WriteLine($"Dequeued Left Array Object");
                if (rightPacketElementAsArray.Value.TryDequeue(out PacketElement right))
                {
                    logger.WriteLine($"Dequeued Right Array Object and Comparing Results");

                    var compareResult = ComparePacketElements(left, right, logger);

                    if (compareResult == CompareStatus.BAD)
                    {
                        logger.WriteLine($"TWO Array Elements NOT VALID");
                        return CompareStatus.BAD;
                    }

                    if (compareResult == CompareStatus.VALID)
                    {
                        logger.WriteLine($"TWO Array Elements VALID");
                        return CompareStatus.VALID;
                    }
                }
                else
                {
                    logger.WriteLine(
                        $"Right Array did not Have Object to Return " +
                        $"- Right Array Ran out Of Items Before Left: BAD");
                    return CompareStatus.BAD;
                }
            }

            logger.WriteLine($"Finished Fully De-Queueing LEFT Array - Checking If Right Still Has Elements");

            if (rightPacketElementAsArray.Value.Count > 0)
            {
                logger.WriteLine($"RIGHT - Still HAD ELEMENTS - Returning Valid");
                return CompareStatus.VALID;
            }
            else
            {
                logger.WriteLine($"BOTH LEFT and RIGHT EXHAUSTED - Returning EQUAL");
                return CompareStatus.EQUAL;
            }
        }

        logger.WriteLine($"Comparing one ARRAY and one NUMBER element");

        if (leftPacketElement is NumberElement leftNumber)
        {
            ArrayElement castedLeft = leftNumber.ToArrayElement();

            logger.WriteLine($"CASTED Left Element as ARRAY - Comparing LEFT and RIGHT as BOTH Arrays");

            return ComparePacketElements(castedLeft, rightPacketElement, logger);
        }

        if (rightPacketElement is NumberElement rightNumber)
        {
            ArrayElement castedRight = rightNumber.ToArrayElement();

            logger.WriteLine($"CASTED Right Element as ARRAY - Comparing LEFT and RIGHT as BOTH Arrays");
            return ComparePacketElements(leftPacketElement, castedRight, logger);
        }

        return CompareStatus.BAD;
    }

    public override ValueTask<string> Solve_2()
    {
        // process the input string
        var logger = new LogWrapper(false);

        logger.WriteLine("===== PART 2 =====");

        return new("");
    }
}