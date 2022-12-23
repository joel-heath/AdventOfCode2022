using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace AdventOfCode2022;
internal class Day23 : IDay
{
    public int Day => 23;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { ".....\r\n..##.\r\n..#..\r\n.....\r\n..##.\r\n.....", "25" },
        { "....#..\r\n..###.#\r\n#...#.#\r\n.#...##\r\n#.###..\r\n##.#.##\r\n.#..#..", "110" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static Point[] ParseInput(string input)
    {
        List<Point> map = new();
        string[] lines = input.Split("\r\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '#')
                    map.Add((j, i));
            }
        }

        return map.ToArray();
    }

    static Point? ConsiderMove(Point elf, Point[] elves, int num)
    {
        Point[] north = new Point[] { (-1, -1), (0, -1), (1, -1) }.Select(p => elf + p).ToArray();
        Point[] south = new Point[] { (-1, 1), (0, 1), (1, 1) }.Select(p => elf + p).ToArray();
        Point[] west = new Point[] { (-1, -1), (-1, 0), (-1, 1) }.Select(p => elf + p).ToArray();
        Point[] east = new Point[] { (1, -1), (1, 0), (1, 1) }.Select(p => elf + p).ToArray();
        Point[][] directions = { north, south, west, east };

        Point[] adjacents = north.Concat(south).Concat(new Point[] { east[1], west[1] }).ToArray();
        //Point[] allAdjacents = { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) }.Select(a => elf + a);
        if (!adjacents.Any(elves.Contains)) return null;

        for (int i = num; i < num + 4; i++)
        {
            Point[] direction = directions[i % 4];
            if (!direction.Any(elves.Contains))
                return direction[1];
        }

        return null;
    }

    public string SolvePart1(string input)
    {
        Point[] elves = ParseInput(input);
        /*
        Console.Clear();
        for (int j = 0; j < elves.Length; j++)
        {
            Console.SetCursorPosition(elves[j].X, elves[j].Y);
            Console.Write('#');
        }
        Console.ReadKey(true);*/

        for (int i = 0; i < 10; i++)
        {
            // first half
            Point?[] newPoses = elves.Select(e => ConsiderMove(e, elves, i)).ToArray();
            /*
            Point?[] newPoses = new Point[elves.Length];

            for (int j = 0; j < elves.Length; j++)
            {
                newPoses[j] = ConsiderMove(elves[j], elves, i);
            }
            */

            // second half
            
            for (int j = 0; j < elves.Length; j++)
            {
                if (newPoses.Count(p => p != null && p == newPoses[j]) == 1)
                    elves[j] = newPoses[j];
            }

            //foreach ((Point? e, int i) pos in newPoses.Select((e, i) => (e, i)).Where(e => e.e is not null).DistinctBy(e => e.e)) elves[pos.i] = pos.e!;
            /*
            Console.Clear();
            for (int j = 0; j < elves.Length; j++)
            {
                Console.SetCursorPosition(elves[j].X, elves[j].Y);
                Console.Write('#');
            }
            Console.ReadKey(true);*/
        }
        /*
        for (int j = 0; j < elves.Length; j++)
        {
            try
            {
                Console.SetCursorPosition(elves[j].X, elves[j].Y);
                Console.Write('#');
            }
            catch { }
        }
        Console.ReadKey(true);*/


        Point bottomRight = (elves.Max(e => e.X), elves.Max(e => e.Y));
        Point topLeft = (elves.Min(e => e.X), elves.Min(e => e.Y));

        int count = 0;
        for (int i = topLeft.X; i <= bottomRight.X; i++)
        {
            for (int j = topLeft.Y; j <= bottomRight.Y; j++)
            {
                if (!elves.Contains((i, j))) count++;
            }
        }


        return $"{count}";
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }
}
