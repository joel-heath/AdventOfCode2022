using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode2022;
internal class Day8 : IDay
{
    public int Day => 8;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "30373\r\n25512\r\n65332\r\n33549\r\n35390", "21" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "30373\r\n25512\r\n65332\r\n33549\r\n35390", "8" }
    };

    static int[][] ParseInput(string input)
        => input.Split("\r\n").Select(l => l.ToArray().Select(c => int.Parse($"{c}")).ToArray()).ToArray();

    static bool IsVisible(int[][] map, int y, int x)
        => !map[y].Take(x).Where(r => r >= map[y][x]).Any()
        || !map[y].Reverse().Take(map[y].Length - x - 1).Where(r => r >= map[y][x]).Any()
        || !map.Reverse().Take(map.Length - y - 1).Where(r => r[x] >= map[y][x]).Any()
        || !map.Take(y).Where(r => r[x] >= map[y][x]).Any();

    static int GetScenicScore(int[][] map, int y, int x)
        => (y - map.Take(y).Select((r, i) => (r, i)).LastOrDefault(t => t.r[x] >= map[y][x]).i) *
           (x - map[y].Take(x).Select((r, i) => (r, i)).LastOrDefault(t => t.r >= map[y][x]).i) *
           (map.Length - 1 - y - map.Skip(y + 1).Reverse().Select((r, i) => (r, i)).LastOrDefault(t => t.r[x] >= map[y][x]).i) *
           (map[y].Length - 1 - x - map[y].Skip(x + 1).Reverse().Select((r, i) => (r, i)).LastOrDefault(t => t.r >= map[y][x]).i);

    public string SolvePart1(string input)
    {
        int[][] grid = ParseInput(input);
        return $"{grid.Select((r, y) => r.Where((_, x) => IsVisible(grid, y, x)).Count()).Sum()}";
    }

    public string SolvePart2(string input)
    {
        int[][] grid = ParseInput(input);
        return $"{grid.Select((r, y) => r.Select((_, x) => GetScenicScore(grid, y, x)).Max()).Max()}";
    }
}
