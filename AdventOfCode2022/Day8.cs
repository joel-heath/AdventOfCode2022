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

    public int[][] ParseInput(string input)
        => input.Split("\r\n").Select(l => l.ToArray().Select(c => int.Parse($"{c}")).ToArray()).ToArray();

    public bool IsVisible(int[][] map, int y, int x)
    {
        int height = map[y][x];

        bool north = true;
        for (int i = y - 1; i >= 0; i--)
        {
            if (map[i][x] >= height)
            {
                north = false;
            }
        }
        if (north) return true;

        // south
        bool south = true;
        for (int i = y + 1; i < map.GetLength(0); i++)
        {
            if (map[i][x] >= height)
            {
                south = false;
            }
        }
        if (south) return true;

        //east
        bool east = true;
        for (int j = x + 1; j < map[0].GetLength(0); j++)
        {
            if (map[y][j] >= height)
            {
                east = false;
            }
        }
        if (east) return true;

        //west
        bool west = true;
        for (int j = x - 1; j >= 0; j--)
        {
            if (map[y][j] >= height)
            {
                west = false;
            }
        }
        return west;
    }

    public int GetScenicScore(int[][] map, int y, int x)
    {
        int height = map[y][x];
        int score = 0;

        int north = 0;
        for (int i = y - 1; i >= 0; i--)
        {
            north++;
            if (map[i][x] >= height)
            {
                break;
            }
        }

        int south = 0;
        for (int i = y + 1; i < map.GetLength(0); i++)
        {
            south++;
            if (map[i][x] >= height)
            {
                break;
            }
        }

        int east = 0;
        for (int j = x + 1; j < map[0].GetLength(0); j++)
        {
            east++;
            if (map[y][j] >= height)
            {
                break;
            }
        }

        int west = 0;
        for (int j = x - 1; j >= 0; j--)
        {
            west++;
            if (map[y][j] >= height)
            {
                break;
            }
        }
        return north * south * east * west;
    }

    public string SolvePart1(string input)
    {
        int[][] grid = ParseInput(input);
        int count = 0;

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid[0].GetLength(0); x++)
            {
                if (IsVisible(grid, y, x))
                {
                    //Console.WriteLine($"({y}, {x}): {grid[y][x]} is visible");
                    count++;
                }
            }
        }

        return $"{count}";
    }

    public string SolvePart2(string input)
    {
        int[][] grid = ParseInput(input);
        int max = 0;

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid[0].GetLength(0); x++)
            {
                int score = GetScenicScore(grid, y, x);
                if (score > max)
                {
                    max = score;
                }
            }
        }

        return $"{max}";
    }
}
