using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using System.Collections;

namespace AdventOfCode2022;
internal class Day6 : IDay
{
    public int Day => 6;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "mjqjpqmgbljsphdztnvjfqwrcgsmlb", "7" },
        { "bvwbjplbgvbhsrlpgdmjqwftvncz", "5" },
        { "nppdvjthqldpwncqszvftbrmjlhg", "6" },
        { "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", "10" },
        { "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", "11" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "mjqjpqmgbljsphdztnvjfqwrcgsmlb", "19" },
        { "bvwbjplbgvbhsrlpgdmjqwftvncz", "23" },
        { "nppdvjthqldpwncqszvftbrmjlhg", "23" },
        { "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", "29" },
        { "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", "26" }
    };

    public string SolvePart1(string input)
        => $"{Enumerable.Range(4, input.Length).Where(i => Enumerable.Range(0, 4).Select(j => input[i - j]).Distinct().Count() == 4).First() + 1}";

    public string SolvePart2(string input)
        => $"{Enumerable.Range(14, input.Length).Where(i => Enumerable.Range(0, 14).Select(j => input[i - j]).Distinct().Count() == 14).First() + 1}";
}
