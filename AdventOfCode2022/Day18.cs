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
        { "2,2,2\r\n1,2,2\r\n3,2,2\r\n2,1,2\r\n2,3,2\r\n2,2,1\r\n2,2,3\r\n2,2,4\r\n2,2,6\r\n1,2,5\r\n3,2,5\r\n2,1,5\r\n2,3,5", "58" }
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

    /*
    static List<Coord> AllContained(List<Coord> coords)
    {
        HashSet<Coord> newCoords = coords.ToHashSet();

        foreach (Coord coord in coords)
        {
            foreach (Coord a in coord.Adjacents())
            {
                try
                {
                    if (a.Z >= newCoords.Where(c => c.X == a.X && c.Y == a.Y).Max(c => c.Z) ||
                        a.Z <= newCoords.Where(c => c.X == a.X && c.Y == a.Y).Min(c => c.Z) ||
                        a.Y >= newCoords.Where(c => c.Z == a.Z && c.X == a.X).Max(c => c.Y) ||
                        a.Y <= newCoords.Where(c => c.Z == a.Z && c.X == a.X).Min(c => c.Y) ||
                        a.X >= newCoords.Where(c => c.Z == a.Z && c.Y == a.Y).Max(c => c.X) ||
                        a.X <= newCoords.Where(c => c.Z == a.Z && c.Y == a.Y).Min(c => c.X) ||
                        a.X >= newCoords.Max(c => c.X) || a.Y >= newCoords.Max(c => c.Y) || a.Z >= newCoords.Max(c => c.Z) ||
                        a.X <= newCoords.Min(c => c.X) || a.Y <= newCoords.Min(c => c.Y) || a.Z <= newCoords.Min(c => c.Z)
                        || newCoords.Contains(a)) continue;
                }
                catch { continue; }

                try
                {
                    newCoords.Add(a);
                    Console.WriteLine($"Added missing co-ordinate {a}");
                }
                catch { continue; }
            }
        }

        return newCoords.ToList();
    }
    */

    static List<Coord> AllContained(List<Coord> coords)
    {
        List<Coord> newCoords = coords.ToList();

        Coord upperBound = (newCoords.Max(c => c.X), newCoords.Max(c => c.Y), newCoords.Max(c => c.Z));
        Coord lowerBound = (newCoords.Min(c => c.X), newCoords.Min(c => c.Y), newCoords.Min(c => c.Z));

        for (int i = 0; i < newCoords.Count; i++)
        {
            Coord p = newCoords[i];

            //if (p.Z > newCoords.Where(c => c.X == p.X && c.Y == p.Y).DefaultIfEmpty(new Coord(0, 0, int.MinValue)).Max(c => c.Z)) continue;
            //if (p.Z < newCoords.Where(c => c.X == p.X && c.Y == p.Y).DefaultIfEmpty(new Coord(0, 0, int.MaxValue)).Min(c => c.Z)) continue;

            //if (p.Y > newCoords.Where(c => c.Z == p.Z && c.X == p.X).DefaultIfEmpty(new Coord(0, int.MinValue, 0)).Max(c => c.Y)) continue;
            //if (p.Y < newCoords.Where(c => c.Z == p.Z && c.X == p.X).DefaultIfEmpty(new Coord(0, int.MaxValue, 0)).Min(c => c.Y)) continue;

            //if (p.X > newCoords.Where(c => c.Z == p.Z && c.Y == p.Y).DefaultIfEmpty(new Coord(int.MinValue, 0, 0)).Max(c => c.X)) continue;
            //if (p.X < newCoords.Where(c => c.Z == p.Z && c.Y == p.Y).DefaultIfEmpty(new Coord(int.MaxValue, 0, 0)).Min(c => c.X)) continue;

            //if (p.X > upperBound.X || p.Y > upperBound.Y || p.Z > upperBound.Z) continue;
            //if (p.X < lowerBound.X || p.Y < lowerBound.Y || p.Z < lowerBound.Z) continue;


            foreach (Coord a in p.Adjacents())
            {
                if (newCoords.Contains(a)) continue;

                /*
                if (a.Z > newCoords.Where(c => c.X == a.X && c.Y == a.Y).DefaultIfEmpty(new Coord(0, 0, int.MaxValue)).Max(c => c.Z)) continue;
                if (a.Z < newCoords.Where(c => c.X == a.X && c.Y == a.Y).DefaultIfEmpty(new Coord(0, 0, int.MinValue)).Min(c => c.Z)) continue;

                if (a.Y > newCoords.Where(c => c.Z == a.Z && c.X == a.X).DefaultIfEmpty(new Coord(0, int.MaxValue, 0)).Max(c => c.Y)) continue;
                if (a.Y < newCoords.Where(c => c.Z == a.Z && c.X == a.X).DefaultIfEmpty(new Coord(0, int.MinValue, 0)).Min(c => c.Y)) continue;

                if (a.X > newCoords.Where(c => c.Z == a.Z && c.Y == a.Y).DefaultIfEmpty(new Coord(int.MaxValue, 0, 0)).Max(c => c.X)) continue;
                if (a.X < newCoords.Where(c => c.Z == a.Z && c.Y == a.Y).DefaultIfEmpty(new Coord(int.MinValue, 0, 0)).Min(c => c.X)) continue;

                if (a.X > upperBound.X || a.Y > upperBound.Y || a.Z > upperBound.Z) continue;
                if (a.X < lowerBound.X || a.Y < lowerBound.Y || a.Z < lowerBound.Z) continue;
                */


                IEnumerable<Coord> sameX = newCoords.Where(c => c.X == a.X);
                IEnumerable<Coord> sameY = newCoords.Where(c => c.Y == a.Y);
                IEnumerable<Coord> sameZ = newCoords.Where(c => c.Z == a.Z);

                IEnumerable<int> similarX = sameY.Intersect(sameZ).Select(c => c.X).DefaultIfEmpty(int.MaxValue);
                IEnumerable<int> similarY = sameX.Intersect(sameZ).Select(c => c.Y).DefaultIfEmpty(int.MaxValue);
                IEnumerable<int> similarZ = sameX.Intersect(sameY).Select(c => c.Z).DefaultIfEmpty(int.MaxValue);


                if (a.X < similarX.Max() && a.X > similarX.Min() &&
                    a.Y < similarY.Max() && a.Y > similarY.Min() &&
                    a.Z < similarZ.Max() && a.Z > similarZ.Min())
                {
                    newCoords.Add(a);
                    Console.WriteLine($"Added missing co-ordinate {a}");
                }
            }
        }

        return newCoords;
    }

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
        
        /*
        int surfaceArea = coords.Count * 6;
        for (int i = 0; i < coords.Count; i++)
        {
            for (int j = 0; j < coords.Count; j++)
            {
                if (i == j) continue;
                surfaceArea -= Intersect(coords[i], coords[j]);
            }
        }
        */


        return surfaceArea;
    }

    static List<Coord> SuperContained(List<Coord> coords)
    {
        List<Coord> oldCoords = new();

        while (oldCoords.Count != coords.Count)
        {
            oldCoords = coords.ToList();
            coords = AllContained(coords);
        }

        return oldCoords;
    }

    public string SolvePart2(string input)
        => $"{SurfaceArea(AllContained(ParseInput(input)))}";

    
}
