using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal class Day3 : IDay
{
    public int Day => 3;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
    };

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ").Select(s => s.Trim(' ')).ToArray();
        }



        return string.Empty;
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }
}
