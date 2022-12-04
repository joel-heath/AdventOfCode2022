using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022;
internal class Day4 : IDay
{
    public int Day => 4;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", "2" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", "4" }
    };

    public string SolvePart1(string input)
    => $"{input.Split("\r\n").Select(l => l.Split(",")
                .Select(s => s.Split("-").Select(int.Parse).ToArray()).ToArray()).ToArray()
                .Select(l => ((l[1][1] >= l[0][1] && l[1][0] <= l[0][0]) || (l[0][1] >= l[1][1] && l[0][0] <= l[1][0]) ? 1 : 0)).Sum()}";

    public string SolvePart2(string input)
    => $"{input.Split("\r\n").Select(l => l.Split(",")
            .Select(s => s.Split("-").Select(int.Parse).ToArray())
            .Select(s => Enumerable.Range(s[0], s[1] - s[0] + 1).ToArray()).ToArray())
            .Select(l => l[0].Intersect(l[1]).Any() ? 1 : 0).Sum()}";
}
