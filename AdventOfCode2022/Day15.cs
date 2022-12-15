using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace AdventOfCode2022;
internal partial class Day15 : IDay
{
    public int Day => 15;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "Sensor at x=2, y=18: closest beacon is at x=-2, y=15\r\nSensor at x=9, y=16: closest beacon is at x=10, y=16\r\nSensor at x=13, y=2: closest beacon is at x=15, y=3\r\nSensor at x=12, y=14: closest beacon is at x=10, y=16\r\nSensor at x=10, y=20: closest beacon is at x=10, y=16\r\nSensor at x=14, y=17: closest beacon is at x=10, y=16\r\nSensor at x=8, y=7: closest beacon is at x=2, y=10\r\nSensor at x=2, y=0: closest beacon is at x=2, y=10\r\nSensor at x=0, y=11: closest beacon is at x=2, y=10\r\nSensor at x=20, y=14: closest beacon is at x=25, y=17\r\nSensor at x=17, y=20: closest beacon is at x=21, y=22\r\nSensor at x=16, y=7: closest beacon is at x=15, y=3\r\nSensor at x=14, y=3: closest beacon is at x=15, y=3\r\nSensor at x=20, y=1: closest beacon is at x=15, y=3", "26" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static bool AddToGrid(Dictionary<int, char> grid, int x, char c)
    {
        if (grid.ContainsKey(x)) 
            return false;

        grid.Add(x, c);
        return true;
    }

    static Dictionary<int, char> ParseInput(string input, int line)
    {
        Dictionary<int, char> grid = new();

        //Regex r = InputParse();

        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {    
            int[] nums = InputParse().Match(lines[i]).Groups.Cast<Group>().Skip(1).Select(c => int.Parse(c.Value)).ToArray();

            if (nums[1] == line)
                AddToGrid(grid, nums[0], 'S'); // add sensor 

            if (nums[3] == line)
                AddToGrid(grid, nums[2], 'B'); // add beacon


            int distance = Math.Abs(nums[2] - nums[0]) + Math.Abs(nums[3] - nums[1]);

            if (-distance <= line - nums[1] && line - nums[1] <= distance)
            {
                int count = distance - (line - nums[1]); // num of # either side of midpoint

                for (int j = -count; j <= count; j++)
                {
                    AddToGrid(grid, nums[0] + j, '#');
                }
            }
        }


        return grid;
    }

    static void RenderLine(Dictionary<int, char> grid)
    {
        foreach (var kvp in grid)
        {
            Console.CursorLeft = kvp.Key+10;
            Console.Write(kvp.Value);
        }
    }

    public string SolvePart1(string input)
    {
        const int YLEVEL = 10;
        var grid = ParseInput(input, YLEVEL);
        Console.WriteLine("Input Parsed!");
        
        RenderLine(grid);

        /*
        Console.ReadKey(true);
        Console.Clear();
        DrawGrid(grid);

        Console.ReadKey(true);
        */


        return $"{grid.Where(kvp => kvp.Value != 'B').Count()}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }

    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex InputParse();
}
