using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

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

    static (Dictionary<int, Dictionary<int, char>>, (int m, char d)[]) ParseInput(string input)
    {
        string[] sectors = input.Split("\r\n\r\n");
        string[] mapLines = sectors[0].Split("\r\n");
        string directions = sectors[1];

        //         y              x     wall or space
        Dictionary<int, Dictionary<int, char>> map = new();
        Regex r = InstructionParse();

        for (int i = 0; i < mapLines.Length; i++)
        {
            (char w, int i)[] line = mapLines[i].Select((w, i) => (w, i)).Where(t => t.w != ' ').ToArray();

            for (int j = 0; j < line.Length; j++)
            {
                (char w, int i) word = line[j];

                AddToDict(map, i, word.i, word.w);
            }
        }

        return (map, r.Matches(directions).Cast<Match>().Select(m => (int.Parse(m.Value[..^1]), m.Value[^1])).Append((int.Parse($"{directions[^1]}"), 'R')).ToArray());
    }

    static int mod(int x, int m) => (x % m + m) % m;

    static void AddToDict(Dictionary<int, Dictionary<int, char>> dict, int y, int x, char s)
    {
        if (dict.TryGetValue(y, out var rows))
        {
            rows[x] = s;
        }
        else
        {
            dict[y] = new Dictionary<int, char>() { { x, s } };
        }
    }

    static (int, int) TryMovePlayer(Dictionary<int, Dictionary<int, char>> dict, int y, int x, int oldY, int oldX)
    {
        if (dict.TryGetValue(y, out var row) && row.TryGetValue(x, out var position))
            return position == '.' ? (y, x) : (oldY, oldX);


        // fallen off x-axis
        if (oldY == y)
        {
            var closeX = row!.Min(x => x.Key);
            int farX = row!.Max(x => x.Key);

            // fallen off right side 
            if (farX == oldX && oldX + 1 == x)
                return row![closeX] == '.' ? (y, closeX) : (oldY, oldX);

            // fallen off left side
            return row![farX] == '.' ? (y, farX) : (oldY, oldX);
        }

        // fallen off y-axis
        var column = dict.Where(r => r.Value.ContainsKey(x));
        int closeY = column.Min(k => k.Key);
        int farY = column.Max(k => k.Key);

        // off top
        if (closeY == oldY && oldY - 1 == y)
            return dict[farY].TryGetValue(x, out char posit) && posit == '.' ? (farY, x) : (oldY, oldX);

        // off bottom
        return dict[closeY].TryGetValue(x, out char pos) && pos == '.' ? (closeY, x) : (oldY, oldX);
    }

    
    public string SolvePart1(string input)
    {
        Dictionary<int, Dictionary<int, char>> map;
        (int m, char d)[] directions;
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

                (newY, newX) = TryMovePlayer(map, newY, newX, y, x);

                if (newY == y && newX == x) break;

                y = newY;
                x = newX;
            }

            direction = mod(direction + (dir == 'R' ? 1 : -1), 4);
        }

        // on last direction added a fake right, so now remove it (direction - 1)
        return $"{(1000 * (y + 1)) + (4 * (x + 1)) + mod(direction - 1, 4)}";
    }

    static (int, int) MovePlayerCube(Dictionary<int, Dictionary<int, char>> dict, int y, int x, int oldY, int oldX, int n, ref int facing)
    {
        if (dict.TryGetValue(y, out var row) && row.TryGetValue(x, out var position))
            return position == '.' ? (y, x) : (oldY, oldX);

        int newY;
        int newX;
        // quadrants 0 and 1
        if (oldY < n)
        {
            // west of quadrant 0 -> west of quad 3
            if (x < n)
            {
                newY = (3 * n) - oldY - 1;
                newX = oldX - n;
                facing = 0;
            }
            // north of quadrant 0 -> west of quad 5
            else if (x < 2 * n)
            {
                newY = oldX + (2 * n);
                newX = oldY;
                facing = 0;
            }

            // top or bottom of quadrant 1
            else if (x < 3 * n)
            {
                // south of quadrant 1 -> east of quad 2
                if (y == n)
                {
                    newY = oldX - n;
                    newX = oldY + n;
                    facing = 2;
                }
                // north of quadrant 1 -> south of quad 5
                else
                {
                    newY = (4 * n) - 1;
                    newX = oldX - (2 * n);
                    facing = 3;
                }
            }
            // east of quad 1 -> east of quad 4
            else
            {
                newY = (3 * n) - oldY - 1;
                newX = oldX - n;
                facing = 2;
            }
        }
        // quadrant 2
        else if (oldY < 2 * n)
        {
            // west of quadrant 2 -> north of quad 3
            if (x < n)
            {
                newY = 2 * n;
                newX = oldY - n;
                facing = 1;
            }
            // east of quadrant 2 -> south of quad 1
            else
            {
                newY = oldX - n;
                newX = oldY + n;
                facing = 3;
            }
        }
        // quadrants 3 or 4
        else if (oldY < 3 * n)
        {
            // west of quad 3 -> west of quad 0
            if (x < 0)
            {
                newY = (3 * n) - oldY - 1;
                newX = oldX + n;
                facing = 0;
            }
            // north of quad 3 -> west of quad 2
            else if (y < 2 * n)
            {
                newY = oldX + n;
                newX = n;
                facing = 0;
            }
            // south of quad 4 -> east of quad 5
            else if (y == 3 * n)
            {
                newY = oldX + (2 * n);
                newX = oldY - (2 * n);
                facing = 2;
            }
            // east of quad 4 -> east of quad 2
            else
            {
                newY = (3 * n) - oldY - 1;
                newX = oldX + n;
                facing = 2;
            }
        }
        // quadrant 5
        else
        {
            // west of quad 5 -> north of quad 0
            if (x < 0)
            {
                newY = oldX;
                newX = oldY - (2 * n);
                facing = 1;
            }
            // south of quad 5 -> north of quad 1
            else if (y == 4 * n)
            {
                newY = 0;
                newX = oldX + (2 * n);
                facing = 1; // still going south
            }
            // east of quad 5 -> south of quad 4
            else
            {
                newY = oldX + (2 * n);
                newX = oldY - (2 * n);
                facing = 3;
            }
        }

        return dict[newY][newX] == '.' ? (newY, newX) : (oldY, oldX);
    }



    public string SolvePart2(string input)
    {
        int n = UnitTestsP2.ContainsKey(input) ? 4 : 50;

        Dictionary<int, Dictionary<int, char>> map;
        (int m, char d)[] directions;
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

                (newY, newX) = MovePlayerCube(map, newY, newX, y, x, n, ref direction);

                if (newY == y && newX == x) break;

                y = newY;
                x = newX;
            }

            direction = mod(direction + (dir == 'R' ? 1 : -1), 4);
        }


        // on last direction added a fake right, so now remove it (direction - 1)
        return $"{(1000 * (y + 1)) + (4 * (x + 1)) + mod(direction - 1, 4)}";
    }

    [GeneratedRegex("(\\d+\\w)")]
    private static partial Regex InstructionParse();
}
