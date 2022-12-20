using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day21 : IDay
{
    public int Day => 21;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "TestInput", "Output" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static void ParseInput(string input)
    {
        string[] lines = input.Split("\r\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(' ').Select(x => x.Trim()).ToArray();

            for (int j = 0; j < line.Length; j++)
            {
                string word = line[j];



            }
        }
    }

    public string SolvePart1(string input)
    {
        ParseInput(input);


        return $"{string.Empty}";
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }
}