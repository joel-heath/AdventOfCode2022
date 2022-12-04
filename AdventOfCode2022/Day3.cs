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
        { "vJrwpWtwJgWrhcsFMMfFFhFp\r\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\r\nPmmdzqPrVvPwwTWBwg\r\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\r\nttgJtRGJQctTZtZT\r\nCrZsJsPPZsGzwwsLwLmpwMDw", "157" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "vJrwpWtwJgWrhcsFMMfFFhFp\r\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\r\nPmmdzqPrVvPwwTWBwg\r\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\r\nttgJtRGJQctTZtZT\r\nCrZsJsPPZsGzwwsLwLmpwMDw", "70" }
    };

    public string SolvePart1(string input)
    => $"{input.Split("\r\n").Select(line => (line[..(line.Length / 2)].Where(ch => line[(line.Length / 2)..].Contains(ch))).Select(l => l > 91 ? l - 96 : l - 38).First()).Sum()}";

    public string SolvePart2(string input)
    => $"{input.Split("\r\n").Chunk(3).Select(line => line[0].Where(ch => line[1].Contains(ch) && line[2].Contains(ch)).Select(l => l > 91 ? l - 96 : l - 38).First()).Sum()}";
}
