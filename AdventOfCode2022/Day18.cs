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

    class Coord
    {
        private readonly int x, y, z;
        public int X { get => x; }
        public int Y { get => y; }
        public int Z { get => z; }

        public Coord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Coord((int X, int Y, int Z) coords) => new(coords.X, coords.Y, coords.Z);

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = this.x;
            y = this.y;
            z = this.z;
        }

        public Coord[] Adjacents() => new Coord[] { this + (0, 0, 1), this - (0, 0, 1), this + (0, 1, 0), this - (0, 1, 0), this + (1, 0, 0), this - (1, 0, 0) };

        public static Coord operator +(Coord a, Coord b) => new(a.x + b.x, a.y + b.y, a.z+b.z);
        public static Coord operator -(Coord a, Coord b) => new(a.x - b.x, a.y - b.y, a.z-b.z);
        public int this[int index] { get => index == 0 ? x : index == 1 ? y : z; }
        public override bool Equals(object? obj) => obj is Coord p && p.x.Equals(x) && p.y.Equals(y) && p.Z.Equals(z);
        public override int GetHashCode() => HashCode.Combine(x, y, z);

        public static bool operator ==(Coord a, Coord b) => (a is null && b is null) || (a is not null && b is not null && a.x == b.x && a.y == b.y && a.z == b.z);
        public static bool operator !=(Coord a, Coord b) => (a is not null || b is not null) && (a is null || b is null || a.x != b.x || a.y != b.y || a.z != b.z);
        public override string ToString() => $"({x}, {y}, {z})";
    }

    static int Intersect(Coord a, Coord b)
    {
        if (a.X == b.X)
        {
            // just one ordinate equal doesnt mean they cross

            if (a.Y == b.Y)
            {
                // ACTUALLY CROSS
                if (a.Z + 1 == b.Z || a.Z - 1 == b.Z)
                    return 2;
            }
            if (a.Z == b.Z)
            {
                if (a.Y + 1 == b.Y || a.Y - 1 == b.Y)
                    return 2;
            }
        }
        if (a.Y == b.Y)
        {
            if (a.Z == b.Z)
            {
                if (a.X + 1 == b.X || a.X - 1 == b.X)
                    return 2;
            }
        }

        // probably miles apart
        return 0;
    }

    static List<Coord> ParseInput(string input)
    {
        string[] lines = input.Split("\r\n");

        List<Coord> coords = new();

        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(',');
            Coord coord = new(int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]));

            coords.Add(coord);
        }

        return coords;
    }

    public string SolvePart1(string input)
        => $"{SurfaceArea(ParseInput(input))}";

    static int SurfaceArea(List<Coord> coords)
    {
        int surfaceArea = coords.Count * 6;
        for (int i = 0; i < coords.Count - 1; i++)
        {
            for (int j = i + 1; j < coords.Count; j++)
            {
                surfaceArea -= Intersect(coords[i], coords[j]);
            }
        }

        return surfaceArea;
    }

    static int FlowWater(List<Coord> lava)
    {
        int area = 0; // +1s and -1s because we must go around the lava droplet
        var pond = new int[lava.Max(c => c.X) + 1, lava.Max(c => c.Y) + 1, lava.Max(c => c.Z) + 1];
        Queue<Coord> queue = new(new Coord[] { (-1, -1, -1) });
        HashSet<Coord> closed = new();

        while (queue.Count > 0)
        {
            Coord current = queue.Dequeue();
            closed.Add(current);

            Coord[] pointsToConsider = current.Adjacents();

            foreach (Coord point in pointsToConsider)
            {
                if (point.X <= pond.GetLength(0) && point.Y <= pond.GetLength(1) && point.Z <= pond.GetLength(2)
                    && point.X >= -1 && point.Y >= -1 && point.Z >= -1)
                {
                    if (lava.Contains(point))
                    {
                        area++;
                    }
                    else if (!closed.Contains(point) && !queue.Contains(point))
                    {
                        queue.Enqueue(point);
                    }
                }
            }
        }

        return area;
    }

    public string SolvePart2(string input)
        => $"{FlowWater(ParseInput(input))}";
}
