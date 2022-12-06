using AdventOfCode.Utils;

namespace AdventOfCode
{
    public class Day06 : BaseDay
    {
        private readonly string _input;

        public Day06()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            // process the input string
            var logger = new LogWrapper(false);

            var markerIndex = 0;

            logger.WriteLine("=======  PART 1 =======");

            logger.WriteLine(_input);

            markerIndex = FindIndexForUniqueLengthMarker(_input, 4, logger);

            return new(markerIndex.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            // process the input string
            var logger = new LogWrapper(false);

            var markerIndex = 0;

            logger.WriteLine("=======  PART 2 =======");

            logger.WriteLine(_input);

            markerIndex = FindIndexForUniqueLengthMarker(_input, 14, logger);

            return new(markerIndex.ToString());
        }

        private int FindIndexForUniqueLengthMarker(string input, int markerLength, LogWrapper logger)
        {
            var markerIndex = 0;
            var tempString = "";

            for (var i = 0; i < input.Length; i++)
            {
                tempString += input[i];
                if (tempString.Length > markerLength)
                {
                    tempString = tempString.Substring(1, tempString.Length - 1);
                }

                // if we are finally dealing with a string of length 4 - check if all characters are unique

                if (tempString.Length == markerLength)
                {
                    logger.WriteLine($"Checking if {tempString} is all unique characters!");

                    var allUniqueChars =
                        tempString.All(workChar => tempString.Count(otherChar => otherChar == workChar) == 1);

                    if (allUniqueChars)
                    {
                        markerIndex = i + 1;
                        break;
                    }
                }
            }

            return markerIndex;
        }
    }
}