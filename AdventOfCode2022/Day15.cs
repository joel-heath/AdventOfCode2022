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
using System.Text.Unicode;

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
        { "Sensor at x=2, y=18: closest beacon is at x=-2, y=15\r\nSensor at x=9, y=16: closest beacon is at x=10, y=16\r\nSensor at x=13, y=2: closest beacon is at x=15, y=3\r\nSensor at x=12, y=14: closest beacon is at x=10, y=16\r\nSensor at x=10, y=20: closest beacon is at x=10, y=16\r\nSensor at x=14, y=17: closest beacon is at x=10, y=16\r\nSensor at x=8, y=7: closest beacon is at x=2, y=10\r\nSensor at x=2, y=0: closest beacon is at x=2, y=10\r\nSensor at x=0, y=11: closest beacon is at x=2, y=10\r\nSensor at x=20, y=14: closest beacon is at x=25, y=17\r\nSensor at x=17, y=20: closest beacon is at x=21, y=22\r\nSensor at x=16, y=7: closest beacon is at x=15, y=3\r\nSensor at x=14, y=3: closest beacon is at x=15, y=3\r\nSensor at x=20, y=1: closest beacon is at x=15, y=3", "56000011" }
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
                int count = distance - Math.Abs((line - nums[1])); // num of # either side of midpoint

                for (int j = -count; j <= count; j++)
                {
                    AddToGrid(grid, nums[0] + j, '#');
                }
            }
        }

        return grid;
    }

    public string SolvePart1(string input)
    {
        int YLEVEL = UnitTestsP1.ContainsKey(input) ? 10 : 2000000;
        var grid = ParseInput(input, YLEVEL);

        return $"{grid.Where(kvp => kvp.Value != 'B').Count()}";
    }

    static void RangeCombiner((int Start, int End)?[] ranges)
    {
        for (int i = 0; i < ranges.Length; i++)
        {
            if (ranges[i] == null) continue;

            for (int j = 0; j < ranges.Length; j++)
            {
                if (ranges[j] == null) continue;
                if (i == j) continue;

                int startLow = Math.Min(ranges[i]!.Value.Start, ranges[j]!.Value.Start);
                int startHigh = Math.Max(ranges[i]!.Value.Start, ranges[j]!.Value.Start);
                int endLow = Math.Min(ranges[i]!.Value.End, ranges[j]!.Value.End);
                int endHigh = Math.Max(ranges[i]!.Value.End, ranges[j]!.Value.End);

                if (startHigh - 1 <= endLow)
                {
                    ranges[i] = new(startLow, endHigh);
                    ranges[j] = null;
                    i = 0;
                    j = 0;
                }
            }
        }
    }

    static (int y, (int, int)[] x) FindRow(Dictionary<Point, int> sensors, int MAX)
    {
        for (int r = 0; r <= MAX; r++)
        {
            List<(int, int)?> ranges = new();

            foreach (var sensor in sensors)
            {
                var manhattenDistance = sensor.Value;
                var distanceFromCentre = Math.Abs(sensor.Key.Y - r);

                if (distanceFromCentre <= manhattenDistance)
                {
                    var min = sensor.Key.X - (manhattenDistance - distanceFromCentre);
                    var max = sensor.Key.X + (manhattenDistance - distanceFromCentre);
                    (int, int) sensorRange = (Math.Max(0, min), Math.Min(max, MAX));
                    
                    
                    ranges.Add(sensorRange);
                }
            }

            var range = ranges.ToArray();
            RangeCombiner(range);

            (int, int)[] final = range.Where(r => r != null).Select(r => r!.Value).ToArray();


            if (final.Length > 1)
            {
                return (r, final);
            }
        }

        throw new("Cannot find row with no beacon");
    }

    static Dictionary<Point, int> ParseInput2(string input)
    {
        Dictionary<Point, int> sensors = new();

        Regex r = InputParse();

        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {
            int[] nums = r.Match(lines[i]).Groups.Cast<Group>().Skip(1).Select(c => int.Parse(c.Value)).ToArray();

            int distance = Math.Abs(nums[2] - nums[0]) + Math.Abs(nums[3] - nums[1]);

            sensors[(nums[0], nums[1])] = distance;
        }

        return sensors;
    }

    public string SolvePart2(string input)
    {
        int MAX = UnitTestsP2.ContainsKey(input) ? 20 : 4000000;
        Dictionary<Point, int> sensors = ParseInput2(input);

        (int y, (int, int)[] xVals) = FindRow(sensors, MAX);
        long x = xVals.OrderBy(r => r.Item1).First().Item2 + 1;

        return $"{x * 4000000 + y}";
    }

    [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
    private static partial Regex InputParse();
}
