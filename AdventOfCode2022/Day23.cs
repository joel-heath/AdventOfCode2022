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
        { "....#..\r\n..###.#\r\n#...#.#\r\n.#...##\r\n#.###..\r\n##.#.##\r\n.#..#..", "20" }
    };

    static Point[] ParseInput(string input)
    => input.Split("\r\n").Select((l, i) => (l, i)).Select(r => r.l.Select((c, i) => (c, i)).Where(c => c.c == '#').Select(c => new Point(c.i, r.i))).SelectMany(p => p).ToArray();

    static Point? ConsiderMove(Point elf, Point[] elves, int offset)
    {
        Point[] cardinals = new Point[] { (0, -1), (0, 1), (-1, 0), (1, 0) }.Select(p => elf + p).ToArray(); // order is the game order -> north south west east
        Point[] diagonals = new Point[] { (1, -1), (-1, 1), (-1, -1), (1, 1) }.Select(p => elf + p).ToArray(); // order is cardinals +45 deg
        Point[] adjacents = cardinals.Concat(diagonals).ToArray();

        Point[] north = { cardinals[0], diagonals[0], diagonals[2] };
        Point[] south = { cardinals[1], diagonals[1], diagonals[3] };
        Point[] west  = { cardinals[2], diagonals[1], diagonals[2] };
        Point[] east  = { cardinals[3], diagonals[0], diagonals[3] };
        Point[][] directions = { north, south, west, east };

        if (!adjacents.Any(elves.Contains)) return null;
        for (int i = offset; i < offset + 4; i++)
        {
            Point[] direction = directions[i % 4];
            if (!direction.Any(elves.Contains)) return direction[0];
        }
        return null;
    }

    public string SolvePart1(string input)
    {
        Point[] elves = ParseInput(input);

        for (int i = 0; i < 10; i++)
        {
            Point?[] propositions = elves.Select(e => ConsiderMove(e, elves, i)).ToArray();
            elves = elves.Select((e, j) => propositions.Count(p => p != null && p == propositions[j]) == 1 ? propositions[j]! : e).ToArray();
        }

        Point topLeft = (elves.Min(e => e.X), elves.Min(e => e.Y));
        Point dimensions = (elves.Max(e => e.X) - topLeft.X + 1, elves.Max(e => e.Y) - topLeft.Y + 1);

        return $"{Enumerable.Range(topLeft.X, dimensions.X)
            .Select(x => Enumerable.Range(topLeft.Y, dimensions.Y)
            .Where(y => !elves.Contains((x, y))).Count()).Sum()}";
    }

    public string SolvePart2(string input)
    {
        Point[] elves = ParseInput(input);
        Point?[] propositions = elves; // temp value

        int roundCount = 0;
        while (propositions.Any(p => p is not null))
        {
            propositions = elves.Select(e => ConsiderMove(e, elves, roundCount++)).ToArray();
            elves = elves.Select((e, j) => propositions.Count(p => p != null && p == propositions[j]) == 1 ? propositions[j]! : e).ToArray();
        }

        return $"{roundCount}";
    }
}
