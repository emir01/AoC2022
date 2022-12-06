namespace AdventOfCode.Utils;

public static class StringLists
{
    /// <summary>
    /// For a given list of strings, will combine consecutive empty strings into a single empty string.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="numbers"></param>
    /// <returns></returns>
    /// <remarks>Implemented to work with a fixed input, knowing that there always will be numberOfEmptyToCombine empty strings in the List</remarks>
    public static List<string> ReplaceConsecutiveEmptyStringsInList(this List<string> list, int emptyStringsToCombine)
    {
        var newList = new List<string>();

        var consecutiveIndex = 0;
        for (var i = 0; i < list.Count; i++)
        {
            var currentString = list[i];
            if (!string.IsNullOrWhiteSpace(currentString))
            {
                newList.Add(currentString);
            }
            else
            {
                // indicate that we found an empty string
                consecutiveIndex++;

                if (consecutiveIndex == emptyStringsToCombine)
                {
                    newList.Add(" ");
                    consecutiveIndex = 0;
                }
            }
        }

        return newList;
    }
}