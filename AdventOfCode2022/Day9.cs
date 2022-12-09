using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode2022;
internal class Day9 : IDay
{
    public int Day => 9;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1\r\nL 5\r\nR 2", "13" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "R 5\r\nU 8\r\nL 8\r\nD 3\r\nR 17\r\nD 10\r\nL 25\r\nU 20", "36" }
    };
    static (int, int) IntelliMove(int headX, int headY, int tailX, int tailY)
    {
        List<(int x, int y)> vectors = new()
        {
            { (2, 0) }, { (-2, 0) }, { (0, 2) }, { (0, -2) },
            { (1, 2) }, { (-1, 2) }, { (1, -2) }, { (-1, -2) },
            { (2, 1) }, { (-2, 1) }, { (2, -1) }, { (-2, -1) },
            { (2, 2) }, { (-2, 2) }, { (2, -2) }, { (-2, -2) },
        };

        foreach (var vect in vectors)
        {
            if ((headX, headY) == (tailX + vect.x, tailY + vect.y))
            {
                return (tailX + vect.x.CompareTo(0), tailY + vect.y.CompareTo(0));
            }
        }

        return (tailX, tailY);
    }

    static void MoveAll(char dir, int mag, (int x, int y)[] rope, List<(int, int)> visited)
    {
        for (int i = 0; i < mag; i++)
        {
            rope[0].x += dir switch { 'L' => -1, 'R' => 1, _ => 0 };
            rope[0].y += dir switch { 'U' => -1, 'D' => 1, _ => 0 };

            for (int h = 1; h < rope.Length; h++)
            {
                rope[h] = IntelliMove(rope[h - 1].x, rope[h - 1].y, rope[h].x, rope[h].y);
            }

            if (!visited.Contains(rope[^1])) { visited.Add(rope[^1]); }
        }
    }

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");
        List<(int, int)> vis = new();

        (int, int)[] rope = new (int x, int y)[2];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ");

            MoveAll(words[0][0], int.Parse(words[1]), rope, vis);
        }

        return $"{vis.Count}";
    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");

        (int, int)[] rope = new (int x, int y)[10];
        List<(int, int)> vis = new ();

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ");

            MoveAll(words[0][0], int.Parse(words[1]), rope, vis);
        }

        return $"{vis.Count}";
    }
}
