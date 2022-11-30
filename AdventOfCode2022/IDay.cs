using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022;
public interface IDay
{
    public int Day { get; }
    public Dictionary<string, string> UnitTestsP1 { get; }
    public Dictionary<string, string> UnitTestsP2 { get; }
    public string SolvePart1(string input);
    public string SolvePart2(string input);
}
class Day
{
    public static IDay GetDay()
    {
        IDay? day = null;
        while (day == null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Which day would you like to solve for? ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string dayStr = Console.ReadLine() ?? "1";

            var allDays = typeof(IDay).Assembly.GetTypes().Where(t => typeof(IDay).IsAssignableFrom(t)).Where(t => t != typeof(IDay));
            var lookup = allDays.ToDictionary(d => d.Name[3..]);

            if (lookup.TryGetValue(dayStr, out Type? value))
                day = (IDay?)Activator.CreateInstance(value);
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        return day;
    }
}