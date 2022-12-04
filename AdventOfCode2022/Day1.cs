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

    public string SolvePart1(string input) => $"{input.Split("\r\n\r\n").Select(e => e.Split("\r\n").Select(int.Parse).Sum()).OrderDescending().First()}";

    public string SolvePart2(string input) => $"{input.Split("\r\n\r\n").Select(e => e.Split("\r\n").Select(int.Parse).Sum()).OrderDescending().Take(3).Sum()}";
}
