using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace AdventOfCode2022;
internal class Day18 : IDay
{
    public int Day => 18;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1,1,1\r\n2,1,1", "10" },
        { "2,2,2\r\n1,2,2\r\n3,2,2\r\n2,1,2\r\n2,3,2\r\n2,2,1\r\n2,2,3\r\n2,2,4\r\n2,2,6\r\n1,2,5\r\n3,2,5\r\n2,1,5\r\n2,3,5", "64" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "1,1,1\r\n2,1,1", "10" },
        { "2,2,2\r\n1,2,2\r\n3,2,2\r\n2,1,2\r\n2,3,2\r\n2,2,1\r\n2,2,3\r\n2,2,4\r\n2,2,6\r\n1,2,5\r\n3,2,5\r\n2,1,5\r\n2,3,5", "58" },
    };

    static HashSet<Coord> ParseInput(string input)
        => input.Split("\r\n").Select(l => l.Split(',').Select(int.Parse).ToArray()).Select(l => new Coord(l[0], l[1], l[2])).ToHashSet();
    static int SurfaceArea(HashSet<Coord> coords)
        => coords.Select(c => c.Adjacents().Where(a => !coords.Contains(a)).Count()).Sum();
    public string SolvePart1(string input)
        => $"{SurfaceArea(ParseInput(input))}";

    static int FlowWater(HashSet<Coord> lava)
    {
        int area = 0;
        Coord pond = (lava.Max(c => c.X) + 1, lava.Max(c => c.Y) + 1, lava.Max(c => c.Z) + 1);
        Queue<Coord> queue = new(new Coord[] {(-1, -1, -1)});
        HashSet<Coord> water = new(new Coord[] {(-1, -1, -1)});

        while (queue.TryDequeue(out var current))
        {
            foreach (var point in current.Adjacents().Where(p => p <= pond && p >= (-1, -1, -1) && !water.Contains(p)))
            {
                if (lava.Contains(point)) area++;
                else { queue.Enqueue(point); water.Add(point); }
            }
        }

        return area;
    }

    public string SolvePart2(string input)
        => $"{FlowWater(ParseInput(input))}";
}
