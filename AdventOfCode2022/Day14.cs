using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using System.Security;
using System.Numerics;
using System.Security.Cryptography;

namespace AdventOfCode2022;
internal class Day14 : IDay
{
    public int Day => 14;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "498,4 -> 498,6 -> 496,6\r\n503,4 -> 502,4 -> 502,9 -> 494,9", "24" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "498,4 -> 498,6 -> 496,6\r\n503,4 -> 502,4 -> 502,9 -> 494,9", "93" }
    };

    static void AddToGrid(Dictionary<int, List<int>> grid, int x, int y)
    {
        if (grid.TryGetValue(x, out List<int>? value))
        {
            value.Add(y);
        }
        else
        {
            grid[x] = new List<int>() { y };
        }
    }

    static void StraightLine(Dictionary<int, List<int>> grid, (int x, int y) p1, (int x, int y) p2)
    {
        if (p1.x == p2.x)
        {
            int small = Math.Min(p1.y, p2.y);
            int large = Math.Max(p1.y, p2.y);

            for (int i = small; i <= large; i++)
            {
                AddToGrid(grid, p1.x, i);
            }
        }
        else
        {
            int small = Math.Min(p1.x, p2.x);
            int large = Math.Max(p1.x, p2.x);

            for (int i = small; i <= large; i++)
            {
                AddToGrid(grid, i, p1.y);
            }
        }
    }

    static (int, int) FlowSand(Dictionary<int, List<int>> grid, (int x, int y) p, int abyssLevel)
    {
        var vectors = new (int x, int y)[]
        {
            ( 0, 1), // Down
            (-1, 1), // Down-Left
            ( 1, 1), // Down-Right
        };

        foreach (var (x, y) in vectors)
        {
            (int x, int y) newPoint = (p.x + x, p.y + y);

            if (newPoint.y == abyssLevel || grid.TryGetValue(newPoint.x, out List<int>? value) && value.Contains(newPoint.y))
            {
                continue;
            }
            else
            {
                return newPoint;
            }
        }

        return p;
    }

    static (Dictionary<int, List<int>>, int) ParseInput(string input)
    {
        Dictionary<int, List<int>> grid = new();
        int abyssLevel = 0;

        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(" -> ");
            string[] init = line[0].Split(',');

            (int x, int y) old = new(int.Parse(init[0]), int.Parse(init[1]));

            for (int j = 1; j < line.Length; j++)
            {
                string[] coord = line[j].Split(',');
                (int x, int y) curr = (int.Parse(coord[0]), int.Parse(coord[1]));

                if (curr.y > abyssLevel) abyssLevel = curr.y;

                StraightLine(grid, old, curr);
                old = curr;
            }
        }

        return (grid, abyssLevel + 2);
    }

    public string SolvePart1(string input)
    {
        (var grid, int abyssLevel) = ParseInput(input);
        
        int count = 0;
        bool reachedAbyss = false;
        while (!reachedAbyss)
        {
            bool rested = false;
            (int x, int y) pos = (500, 0);
            while (!rested)
            {
                (int x, int y) newPos = FlowSand(grid, pos, -1);

                if (newPos.x == pos.x && newPos.y == pos.y)
                {
                    rested = true;
                    count++;
                    AddToGrid(grid, pos.x, pos.y);
                }
                else
                {
                    pos = newPos;
                    if (pos.y > abyssLevel)
                    {
                        reachedAbyss = true;
                        //count--;
                        break;
                    }
                }
            }
        }

        return $"{count}";
    }

    public string SolvePart2(string input)
    {
        (var grid, int abyssLevel) = ParseInput(input);
        
        int count = 0;
        bool completelyFilled = false;
        while (!completelyFilled)
        {
            bool rested = false;
            (int x, int y) pos = (500, 0);
            while (!rested)
            {
                (int x, int y) newPos = FlowSand(grid, pos, abyssLevel);

                if (newPos.x == pos.x && newPos.y == pos.y)
                {
                    if (newPos.x == 500 && newPos.y == 0)
                    {
                        completelyFilled = true;
                    }
                    rested = true;
                    count++;
                    AddToGrid(grid, pos.x, pos.y);
                }
                else
                {
                    pos = newPos;
                }
            }
        }

        return $"{count}";
    }
}
