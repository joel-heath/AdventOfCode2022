using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace AdventOfCode2022;
internal class Day2 : IDay
{
    public int Day => 2;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "A Y\r\nB X\r\nC Z", "15" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "A Y\r\nB X\r\nC Z", "12" },
    };

    public string SolvePart1(string input)
    => $"{input.Split("\r\n").Select(l => l.Split(" ").Select(w => w[0] % 'A' % ('A' - 'X')).ToArray()).Select(l => (l[1] + 1, (l[1] - l[0] + 2) % 3)).Select(l => l.Item1 + (l.Item2 == 0 ? 6 : l.Item2 == 1 ? 0 : 3)).Sum()}";
    
    public string SolvePart2(string input)
    => SolvePart1(string.Join("\r\n", input.Split("\r\n").Select(l => l.Split(" ").Select(w => w[0] % 'A' % ('A' - 'X')).ToArray()).Select(l => new string($"{(char)(l[0] + 65)} {(char)(((l[0] + l[1] + 2) % 3) + 88)}"))));
}
