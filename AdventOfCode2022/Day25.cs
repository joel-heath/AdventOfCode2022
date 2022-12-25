using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2022;
internal class Day25 : IDay
{
    public int Day => 25;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1=-0-2\r\n12111\r\n2=0=\r\n21\r\n2=01\r\n111\r\n20012\r\n112\r\n1=-1=\r\n1-12\r\n12\r\n1=\r\n122", "2=-1=0" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "It will take 50 stars to fill the blender.", "Start the blender" }
    };

    static string DecToSNAFU(long dec)
        => dec == 0 ? "" : (dec % 5) switch {
            0 => DecToSNAFU(dec / 5) + "0",
            1 => DecToSNAFU(dec / 5) + "1",
            2 => DecToSNAFU(dec / 5) + "2",
            3 => DecToSNAFU((dec + 2) / 5) + "=",
            _ => DecToSNAFU((dec + 1) / 5) + "-"};

    public string SolvePart1(string input)
    => DecToSNAFU(input.Split("\r\n").Select(l => l.Select((c, i) => c switch { '=' => -2, '-' => -1, _ => int.Parse($"{c}") } * (long)Math.Pow(5, l.Length - i - 1)).Sum()).Sum());
    
    public string SolvePart2(string input) => "Start the blender";
}