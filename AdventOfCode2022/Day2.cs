using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day2 : IDay
{
    public int Day => 2;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "hello", "Output" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" },
    };

    public string SolvePart1(string input)
    {



        return $"{UnitTestsP1}";
    }

    public string SolvePart2(string input)
    {


        return $"{UnitTestsP2}";
    }
}
