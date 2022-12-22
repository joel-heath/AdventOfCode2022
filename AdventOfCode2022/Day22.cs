using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Numerics;

namespace AdventOfCode2022;
internal partial class Day22 : IDay
{
    public int Day => 22;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "        ...#\r\n        .#..\r\n        #...\r\n        ....\r\n...#.......#\r\n........#...\r\n..#....#....\r\n..........#.\r\n        ...#....\r\n        .....#..\r\n        .#......\r\n        ......#.\r\n\r\n10R5L5R10L4R5L5", "6032" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "        ...#\r\n        .#..\r\n        #...\r\n        ....\r\n...#.......#\r\n........#...\r\n..#....#....\r\n..........#.\r\n        ...#....\r\n        .....#..\r\n        .#......\r\n        ......#.\r\n\r\n10R5L5R10L4R5L5", "5031" }
    };
    static int Mod(int x, int m) => (x % m + m) % m;
    static void AddToDict(Dictionary<int, Dictionary<int, char>> dict, int y, int x, char s)
    {
        if (dict.TryGetValue(y, out var rows)) rows[x] = s;
        else dict[y] = new Dictionary<int, char>() { { x, s } };
    }

    static (Dictionary<int, Dictionary<int, char>>, (int m, char d)[]) ParseInput(string input)
    {
        string[] sectors = input.Split("\r\n\r\n");
        string[] mapLines = sectors[0].Split("\r\n");
        string directions = sectors[1];

        //         y              x     wall or space
        Dictionary<int, Dictionary<int, char>> map = new();

        for (int i = 0; i < mapLines.Length; i++)
        {
            (char w, int i)[] line = mapLines[i].Select((w, i) => (w, i)).Where(t => t.w != ' ').ToArray();

            for (int j = 0; j < line.Length; j++)
                AddToDict(map, i, line[j].i, line[j].w);
        }
        //                                                                                                                     | catch last distance with fake turn right at end
        return (map, InstructionParse().Matches(directions).Cast<Match>().Select(m => (int.Parse(m.Value[..^1]), m.Value[^1])).Append((int.Parse($"{directions[^1]}"), 'R')).ToArray());
    }

    static (int, int, int) MovePlayer2D(Dictionary<int, Dictionary<int, char>> dict, int y, int x, int oldY, int oldX, int d_, int n_)
    {
        if (dict.TryGetValue(y, out var row) && row.TryGetValue(x, out var position))
            return position == '.' ? (y, x, d_) : (oldY, oldX, d_);

        // fallen off x-axis
        if (oldY == y)
        {
            var closeX = row!.Min(x => x.Key);
            int farX = row!.Max(x => x.Key);

            // fallen off right side 
            if (farX == oldX && oldX + 1 == x)
                return row![closeX] == '.' ? (y, closeX, d_) : (oldY, oldX, d_);

            // fallen off left side
            return row![farX] == '.' ? (y, farX, d_) : (oldY, oldX, d_);
        }

        // fallen off y-axis
        var column = dict.Where(r => r.Value.ContainsKey(x));
        int closeY = column.Min(k => k.Key);
        int farY = column.Max(k => k.Key);

        // off top
        if (closeY == oldY && oldY - 1 == y)
            return dict[farY].TryGetValue(x, out char posit) && posit == '.' ? (farY, x, d_) : (oldY, oldX, d_);

        // off bottom
        return dict[closeY].TryGetValue(x, out char pos) && pos == '.' ? (closeY, x, d_) : (oldY, oldX, d_);
    }

    static (int, int, int) MovePlayer3D(Dictionary<int, Dictionary<int, char>> dict, int y, int x, int oldY, int oldX, int facing, int n)
    {
        if (dict.TryGetValue(y, out var row) && row.TryGetValue(x, out var position))
            return position == '.' ? (y, x, facing) : (oldY, oldX, facing);

        // This algorithm is based entirely on my input's cube net--it doesnt work on the test input and may not work on yours
        // quadrants 0 and 1
        if (oldY < n)
        {
            // west of quadrant 0 -> west of quad 3
            if (x < n)
            {
                y = (3 * n) - oldY - 1;
                x = oldX - n;
                facing = 0;
            }
            // north of quadrant 0 -> west of quad 5
            else if (x < 2 * n)
            {
                y = oldX + (2 * n);
                x = oldY;
                facing = 0;
            }

            // top or bottom of quadrant 1
            else if (x < 3 * n)
            {
                // south of quadrant 1 -> east of quad 2
                if (y == n)
                {
                    y = oldX - n;
                    x = oldY + n;
                    facing = 2;
                }
                // north of quadrant 1 -> south of quad 5
                else
                {
                    y = (4 * n) - 1;
                    x = oldX - (2 * n);
                    facing = 3;
                }
            }
            // east of quad 1 -> east of quad 4
            else
            {
                y = (3 * n) - oldY - 1;
                x = oldX - n;
                facing = 2;
            }
        }
        // quadrant 2
        else if (oldY < 2 * n)
        {
            // west of quadrant 2 -> north of quad 3
            if (x < n)
            {
                y = 2 * n;
                x = oldY - n;
                facing = 1;
            }
            // east of quadrant 2 -> south of quad 1
            else
            {
                y = oldX - n;
                x = oldY + n;
                facing = 3;
            }
        }
        // quadrants 3 or 4
        else if (oldY < 3 * n)
        {
            // west of quad 3 -> west of quad 0
            if (x < 0)
            {
                y = (3 * n) - oldY - 1;
                x = oldX + n;
                facing = 0;
            }
            // north of quad 3 -> west of quad 2
            else if (y < 2 * n)
            {
                y = oldX + n;
                x = n;
                facing = 0;
            }
            // south of quad 4 -> east of quad 5
            else if (y == 3 * n)
            {
                y = oldX + (2 * n);
                x = oldY - (2 * n);
                facing = 2;
            }
            // east of quad 4 -> east of quad 2
            else
            {
                y = (3 * n) - oldY - 1;
                x = oldX + n;
                facing = 2;
            }
        }
        // quadrant 5
        else
        {
            // west of quad 5 -> north of quad 0
            if (x < 0)
            {
                y = oldX;
                x = oldY - (2 * n);
                facing = 1;
            }
            // south of quad 5 -> north of quad 1
            else if (y == 4 * n)
            {
                y = 0;
                x = oldX + (2 * n);
                facing = 1; // still going south
            }
            // east of quad 5 -> south of quad 4
            else
            {
                y = oldX + (2 * n);
                x = oldY - (2 * n);
                facing = 3;
            }
        }

        return dict[y][x] == '.' ? (y, x, facing) : (oldY, oldX, facing);
    }

    public int Solve(string input, Func<Dictionary<int, Dictionary<int, char>>, int, int, int, int, int, int, (int, int, int)> mover)
    {
        Dictionary<int, Dictionary<int, char>> map;
        (int m, char d)[] directions;
        int n = UnitTestsP2.ContainsKey(input) ? 4 : 50;
        (map, directions) = ParseInput(input);

        int y = 0;
        int x = map[0].Min(k => k.Key);
        int direction = 0;

        foreach ((int mag, char dir) in directions)
        {
            for (int i = 0; i < mag; i++)
            {
                int newX = x, newY = y;
                switch (direction)
                {
                    case 0: newX++; break; // east
                    case 1: newY++; break; // south
                    case 2: newX--; break; // west
                    case 3: newY--; break; // north
                }

                (newY, newX, direction) = mover(map, newY, newX, y, x, direction, n);

                if (newY == y && newX == x) break;

                y = newY;
                x = newX;
            }

            direction = Mod(direction + (dir == 'R' ? 1 : -1), 4);
        }

        // on last direction added a fake right, so now remove it (direction - 1)
        return (1000 * (y + 1)) + (4 * (x + 1)) + Mod(direction - 1, 4);
    }

    public string SolvePart1(string input) => $"{Solve(input, MovePlayer2D)}";
    public string SolvePart2(string input) => $"{Solve(input, MovePlayer3D)}";

    [GeneratedRegex("(\\d+\\w)")]
    private static partial Regex InstructionParse();
}
