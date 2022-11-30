using System.Text;

namespace AdventOfCode2022;
class CreateFiles
{
    public static void InitialiseRepo()
    {
        // CREATE CS DAYS
        var workingDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!;
        string path = workingDirectory.FullName;
        string fileContents = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day{{DAY}} : IDay
{
    public int Day => {{DAY}};
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { ""TestInput"", ""Output"" },
        { ""Input2"", ""Output"" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { ""TestInput"", ""Output"" },
        { ""Input2"", ""Output"" },
    };

    public string SolvePart1(string input)
    {
        return string.Empty;
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }
}
";
        for (int i = 1; i <= 25; i++)
        {
            using StreamWriter sw = new(new FileStream(Path.Join(path, $"Day{i}.cs"), FileMode.Create));
            sw.Write(fileContents.Replace("{{DAY}}", $"{i}"));
        }


        // CREATE INPUT TXTS
        path = workingDirectory.Parent!.FullName; 

        for (int i = 1; i <= 25; i++)
        {
            using StreamWriter sw = new(new FileStream(Path.Join(path, $"day{i}.txt"), FileMode.Create));
            sw.Write(string.Empty);
        }
    }
}