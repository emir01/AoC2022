namespace AdventOfCode.Utils;

public class Output
{
    public static void PrintDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var pair in dictionary)
        {
            Console.WriteLine($"Key: {pair.Key} ----- Value: {pair.Value}");
        }
    }
}