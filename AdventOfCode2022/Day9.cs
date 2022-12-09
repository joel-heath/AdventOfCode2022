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

    static void TailMove(int headX, int headY, ref int tailX, ref int tailY)
    {
        if (tailX + 2 == headX && tailY == headY) { tailX++; return; }
        if (tailX - 2 == headX && tailY == headY) { tailX--; return; }
        if (tailX == headX && tailY + 2 == headY) { tailY++; return; }
        if (tailX == headX && tailY - 2 == headY) { tailY--; return; }

        if (tailX + 1 == headX && tailY + 2 == headY) { tailX++; tailY++; return; }
        if (tailX - 1 == headX && tailY + 2 == headY) { tailX--; tailY++; return; }
        if (tailX + 1 == headX && tailY - 2 == headY) { tailX++; tailY--; return; }
        if (tailX - 1 == headX && tailY - 2 == headY) { tailX--; tailY--; return; }

        if (tailX + 2 == headX && tailY + 1 == headY) { tailX++; tailY++; return; }
        if (tailX - 2 == headX && tailY + 1 == headY) { tailX--; tailY++; return; }
        if (tailX + 2 == headX && tailY - 1 == headY) { tailX++; tailY--; return; }
        if (tailX - 2 == headX && tailY - 1 == headY) { tailX--; tailY--; return; }
    }

    static void Move(char dir, int mag, ref int headX, ref int headY, ref int tailX, ref int tailY)
    {
        for (int i = 0; i < mag; i++)
        {
            switch (dir)
            {
                case 'L':
                    headX--;
                    break;
                case 'R':
                    headX++;
                    break;
                case 'U':
                    headY--;
                    break;
                case 'D':
                    headY++;
                    break;
            }

            TailMove(headX, headY, ref tailX, ref tailY);

            if (!vis.Contains((tailX, tailY))) vis.Add((tailX, tailY));
        }
    }

    static List<(int, int)> vis = new();

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        int headX = 0; int headY = 0;
        int tailX = 0; int tailY = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ").Select(s => s.Trim(' ')).ToArray();

            Move(words[0][0], int.Parse(words[1]), ref headX, ref headY, ref tailX, ref tailY);
        }



        return $"{vis.Count}";
    }



    static (int, int) IntelliMove(int headX, int headY, int tailX, int tailY)
    {
        if (tailX + 2 == headX && tailY == headY) { tailX++; return (tailX, tailY); }
        if (tailX - 2 == headX && tailY == headY) { tailX--; return (tailX, tailY); }
        if (tailX == headX && tailY + 2 == headY) { tailY++; return (tailX, tailY); }
        if (tailX == headX && tailY - 2 == headY) { tailY--; return (tailX, tailY); }

        if (tailX + 1 == headX && tailY + 2 == headY) { tailX++; tailY++; return (tailX, tailY); }
        if (tailX - 1 == headX && tailY + 2 == headY) { tailX--; tailY++; return (tailX, tailY); }
        if (tailX + 1 == headX && tailY - 2 == headY) { tailX++; tailY--; return (tailX, tailY); }
        if (tailX - 1 == headX && tailY - 2 == headY) { tailX--; tailY--; return (tailX, tailY); }

        if (tailX + 2 == headX && tailY + 1 == headY) { tailX++; tailY++; return (tailX, tailY); }
        if (tailX - 2 == headX && tailY + 1 == headY) { tailX--; tailY++; return (tailX, tailY); }
        if (tailX + 2 == headX && tailY - 1 == headY) { tailX++; tailY--; return (tailX, tailY); }
        if (tailX - 2 == headX && tailY - 1 == headY) { tailX--; tailY--; return (tailX, tailY); }

        if (tailX + 2 == headX && tailY + 2 == headY) { tailX++; tailY++; return (tailX, tailY); }
        if (tailX - 2 == headX && tailY + 2 == headY) { tailX--; tailY++; return (tailX, tailY); }
        if (tailX + 2 == headX && tailY - 2 == headY) { tailX++; tailY--; return (tailX, tailY); }
        if (tailX - 2 == headX && tailY - 2 == headY) { tailX--; tailY--; return (tailX, tailY); }

        return (tailX, tailY);
    }

    static void MoveAll(char dir, int mag, (int x, int y)[] rope)
    {
        for (int i = 0; i < mag; i++)
        {
            switch (dir)
            {
                case 'L':
                    rope[0].x--;
                    break;
                case 'R':
                    rope[0].x++;
                    break;
                case 'U':
                    rope[0].y--;
                    break;
                case 'D':
                    rope[0].y++;
                    break;
            }
            for (int h = 1; h < 10; h++)
            {
                rope[h] = IntelliMove(rope[h - 1].x, rope[h - 1].y, rope[h].x, rope[h].y);
            }

            if (!vis.Contains(rope[9])) { vis.Add(rope[9]); }
        }
    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");

        (int, int)[] rope = new (int x, int y)[10];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ");

            MoveAll(words[0][0], int.Parse(words[1]), rope);
        }

        return $"{vis.Count}";
    }
}
