using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        { "TestInput", "Output" }
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
        if (dict.TryGetValue(y, out var row))
        {
            if (row.TryGetValue(x, out var position))
            {
                return position == '.' ? (y, x) : (oldY, oldX);
            }

            var closeX = row.Min(x => x.Key);
            int farX = row.Max(x => x.Key);

            if (oldY == y)
            {
                // fallen off right side 
                if (farX == oldX && oldX + 1 == x)
                {
                    return (row[closeX] == '.') ? (y, closeX) : (oldY, oldX);
                }
                // off left side
                if (closeX == oldX && oldX - 1 == x)
                {
                    return (row[farX] == '.') ? (y, farX) : (oldY, oldX);
                }
                throw new Exception("Where the heck did you go?");
            }

            var column = dict.Where(r => r.Value.ContainsKey(x));
            int closeY = column.Min(k => k.Key);
            int farY = column.Max(k => k.Key);

            // off top
            if (closeY == oldY && oldY - 1 == y)
            {
                return dict[farY].TryGetValue(x, out char pos) && pos == '.' ? (farY, x) : (oldY, oldX);
            }
            // off bottom
            if (farY == oldY && oldY + 1 == y)
            {
                return dict[closeY].TryGetValue(x, out char pos) && pos == '.' ? (closeY, x) : (oldY, oldX);
            }
        }
        else
        {
            var column = dict.Where(r => r.Value.ContainsKey(x));
            int closeY = column.Min(y => y.Key);
            int farY = column.Max(y => y.Key);

            if (closeY == oldY && oldY - 1 == y)
            {
                return dict[farY].TryGetValue(x, out char position) && position == '.' ? (farY, x) : (oldY, oldX);
            }
            else if (farY == oldY && oldY + 1 == y)
            {
                return dict[closeY].TryGetValue(x, out char position) && position == '.' ? (closeY, x) : (oldY, oldX);
            }
        }
        return (oldY, oldX);
    }

    
    public string SolvePart1(string input)
    {
        Dictionary<int, Dictionary<int, char>> map;
        (int m, char d)[] directions;
        (map, directions) = ParseInput(input);

        int y = 0;
        int x = map[0].Min(k => k.Key);
        int direction = 0;

        //Console.ReadKey(true);
        //Console.Clear();

        //Console.SetCursorPosition(x, y);
        //Console.Write(direction == 0 ? '>' : direction == 1 ? 'v' : direction == 2 ? '<' : '^');

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

                if (newY == y && newX == x)
                {
                    break;
                }
                else
                {
                    y = newY;
                    x = newX;
                    //Console.SetCursorPosition(x, y);
                    //Console.Write(direction == 0 ? '>' : direction == 1 ? 'v' : direction == 2 ? '<' : '^');
                }
            }

            direction = mod(direction + (dir == 'R' ? 1 : -1), 4);
            //Console.SetCursorPosition(x, y);
            //Console.Write(direction == 0 ? '>' : direction == 1 ? 'v' : direction == 2 ? '<' : '^');
        }


        //Console.ReadKey(true);

        // on last direction added a fake right, so now remove it (direction - 1)
        return $"{(1000 * (y + 1)) + (4 * (x + 1)) + (direction - 1)}";



        // 115184 too high
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }

    [GeneratedRegex("(\\d+\\w)")]
    private static partial Regex InstructionParse();
}
