using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day7 : IDay
{
    public int Day => 7;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
    };

    static void AddToDict<TKey>(Dictionary<TKey, int> dict, TKey key, int value)
    {
        if (dict.TryGetValue(key, out int oldValue))
        {
            dict[key] = oldValue + value;
        }
        else
        {
            dict[key] = value;
        }
    }
    static void AddToDict<TKey>(Dictionary<TKey, string> dict, TKey key, string value)
    {
        if (dict.TryGetValue(key, out string? oldValue))
        {
            dict[key] = oldValue + value;
        }
        else
        {
            dict[key] = value;
        }
    }

    static T?[] GetNeighbours<T>(T[,] grid, int y, int x, bool diagonals = false)
    {
        var coordsToCheck = new (int, int)[]
        {
            (y - 1, x), // Up
            (y, x + 1), // Right
            (y + 1, x), // Down
            (y, x - 1), // Left
        };

        if (diagonals)
        {
            coordsToCheck = coordsToCheck.Concat(new (int, int)[]
            {
                (y - 1, x + 1), // Up-Right
                (y + 1, x + 1), // Down-Right
                (y + 1, x - 1), // Down-Left
                (y - 1, x - 1), // Up-Left

            }).ToArray();
        }

        return coordsToCheck.Select(c => y < grid.GetLength(0) && x < grid.GetLength(1) ? grid[c.Item1, c.Item2] : default).ToArray();
    }

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        int[] items = new int[15].Select(s => 5).ToArray();

        Dictionary<string, int> result = new Dictionary<string, int>();
        int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ").Select(s => s.Trim(' ')).ToArray();



            for (int j = 0; j < words.Length; j++)
            {
                string word = words[j];
                if (word == "") continue;


            }
        }



        return $"{string.Empty}";
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }
}
