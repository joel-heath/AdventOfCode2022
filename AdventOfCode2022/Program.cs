using System.IO;
using System.Text;

namespace AdventOfCode2022;
internal class Program
{
    static void UnitTests(IDay day, int part)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Unit Tests");

        var tests = (part == 1) ? day.UnitTestsP1 : day.UnitTestsP2;

        foreach (string testI in tests.Keys)
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine($"Input: {testI}");
            Console.Write($"Output: ");

            string testO = (part == 1) ? day.SolvePart1(testI) : day.SolvePart2(testI);
            Console.WriteLine(testO);
            

            if (testO == tests[testI])
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failure!");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Expected Output: {tests[testI]}");
            }
        }
    }
    static bool BinaryChoice(char opt1, char opt2)
    {
        // waits for a valid choice, returns true for opt1, false for opt2
        Console.ForegroundColor = ConsoleColor.Yellow;

        bool? choice = null;
        while (!choice.HasValue)
        {
            char key = Console.ReadKey(true).KeyChar.ToString().ToUpper()[0];

            if (key == opt1) { choice = true; Console.WriteLine(opt1); }
            else if (key == opt2) { choice = false; Console.WriteLine(opt2); }
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        return choice.Value;
    }
    static void Main(string[] args)
    {
        string startupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.FullName;
        IDay day;
        int part;

        if (args.Contains("init"))
        {
            CreateFiles.InitialiseRepo();
            Environment.Exit(0);
            return;
        }
        else if (args.Length == 3)
        {
            day = Day.TryGetDay(args[0]) ?? throw new ArgumentNullException("Invalid number of days");
            if (!int.TryParse(args[1].Trim(' '), out part) || !(part == 1 || part == 2)) { throw new ArgumentNullException("Invalid part number"); }
            if (args[2] == "1") { UnitTests(day, part); }
        }
        else
        {
            day = Day.GetUserDay();
            Console.Write("Solve for part 1 or 2? ");
            part = BinaryChoice('1', '2') ? 1 : 2;
            Console.Write("Run Test Inputs? ");
            if (BinaryChoice('Y', 'N')) { Console.WriteLine(); UnitTests(day, part); }
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nDay {day.Day} Part {part}");

        string input = File.ReadAllText(Path.Combine(startupPath, @$"Inputs\day{day.Day}.txt"));

        Console.ForegroundColor = ConsoleColor.White;
        //Console.WriteLine($"Input: {input}");
        Console.Write($"Output: ");
        string output = (part == 1) ? day.SolvePart1(input) : day.SolvePart2(input);
        
        Console.WriteLine(output);

        string outputLocation = Path.Combine(startupPath, @$"Outputs\day{day.Day}.txt");

        using StreamWriter sr = new(new FileStream(outputLocation, FileMode.Create), Encoding.UTF8);
        sr.Write(output);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(@$"The output has also been written to {outputLocation}");
        
        Console.ReadKey();
    }
}