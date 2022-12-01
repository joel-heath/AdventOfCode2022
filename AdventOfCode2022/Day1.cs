using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day1 : IDay
{
    public int Day => 1;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1000\r\n2000\r\n3000\r\n\r\n4000\r\n\r\n5000\r\n6000\r\n\r\n7000\r\n8000\r\n9000\r\n\r\n10000", "24000" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "1000\r\n2000\r\n3000\r\n\r\n4000\r\n\r\n5000\r\n6000\r\n\r\n7000\r\n8000\r\n9000\r\n\r\n10000", "45000" },
    };

    public string SolvePart1(string input)
    {
        int max = 0;
        int currentTotal = 0;
        string[] lines = input.Split("\r\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string c = lines[i];

            if (c != "")
            {
                currentTotal += int.Parse(c);
            }
            if (c == "" || i == lines.Length - 1)
            { 
                if (currentTotal > max)
                {
                    max = currentTotal;
                }
                currentTotal = 0;
            }
        }

        return $"{max}";
    }

    public string SolvePart2(string input)
    {
        List<int> Elves = new();
        int currentTotal = 0;
        string[] lines = input.Split("\r\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string l = lines[i];
            
            if (l != "")
            {
                currentTotal += int.Parse(l);
            }
            if (l == "" || i == lines.Length - 1)
            {
                Elves.Add(currentTotal);
                currentTotal = 0;
            }
        }

        Elves.Sort();
        return $"{Elves.GetRange(Elves.Count - 3, 3).Sum()}";
    }
}
