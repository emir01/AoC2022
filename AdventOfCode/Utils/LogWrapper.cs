namespace AdventOfCode.Utils;

/// <summary>
/// A Log Wrapper which can easily be configured as On/Off to allow better debugging when solving Part 1 and Part 2
/// </summary>
public class LogWrapper
{
    private bool _logState = true;

    public LogWrapper(bool logState = true)
    {
        _logState = logState;
    }

    public int Delay { get; set; }

    public void WriteLine(string message)
    {
        if (_logState)
        {
            Console.WriteLine(message);

            if (Delay > 0)
            {
                Thread.Sleep(Delay);
            }
        }
    }

    public void Write(string message)
    {
        if (_logState)
        {
            Console.Write(message);

            if (Delay > 0)
            {
                Thread.Sleep(Delay);
            }
        }
    }

    public void Write(object message)
    {
        if (_logState)
        {
            Console.Write(message);

            if (Delay > 0)
            {
                Thread.Sleep(Delay);
            }
        }
    }
}