using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using System.Security;
using System.Numerics;
using System.Security.Cryptography;

namespace AdventOfCode2022;
internal class Day14 : IDay
{
    public int Day => 14;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "498,4 -> 498,6 -> 496,6\r\n503,4 -> 502,4 -> 502,9 -> 494,9", "24" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "498,4 -> 498,6 -> 496,6\r\n503,4 -> 502,4 -> 502,9 -> 494,9", "93" }
    };

    static void AddToDict<TKey>(Dictionary<TKey, int> dict, TKey key, int value)
    {
        if (dict.TryGetValue(key, out int oldValue))
        {
            dict[key] = oldValue + value;
        }
        else
        {
            dict[key] = value;
        }
    }
    static void AddToDict<TKey>(Dictionary<TKey, string> dict, TKey key, string value)
    {
        if (dict.TryGetValue(key, out string? oldValue))
        {
            dict[key] = oldValue + value;
        }
        else
        {
            dict[key] = value;
        }
    }

    

    enum Material { Air, Rock, RestedSand, Sand }

    static (Dictionary<Point, Material>, int) ParseInput(string input)
    {
        int abyssLevel = 0;
        string[] lines = input.Split("\r\n");

        Dictionary<Point, Material> grid = new Dictionary<Point, Material>();//new PointComparer());

        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(" -> ");
            
            Point? lastCoord = null;
            for (int j = 0; j < line.Length; j++)
            {
                IEnumerable<int> coord = line[j].Split(',').Select(int.Parse);
                Point here = (coord.First(), coord.Last());

                if (coord.Last() > abyssLevel) abyssLevel = coord.Last();

                if (null == lastCoord)
                {
                    grid.Add(here, Material.Rock);
                }
                else
                {
                    lastCoord.PointsTo(here, true).ToList().ForEach(p => grid.Add(p, Material.Rock));
                }

                lastCoord = here;
            }
        }

        return (grid, abyssLevel + 2);
    }

    static bool PositionBlocked(Dictionary<Point, Material> grid, Point point)
    {
        foreach (KeyValuePair<Point, Material> pair in grid)
        {
            if (pair.Key.X == point.X && pair.Key.Y == point.Y) return true;
        }
        return false;
    }

    static Point? FlowSand(Dictionary<Point, Material> grid, Point p)
    {
        var vectors = new Point[]
        {
            ( 0, 1), // Down
            (-1, 1), // Down-Left
            ( 1, 1), // Down-Right
        };

        foreach (var coord in vectors)
        {
            Point newPoint = (p.X + coord.X, p.Y + coord.Y);

            if (grid.ContainsKey(p)) //PositionBlocked(grid, newPoint))
            {
                continue;
            }
            else
            {
                return newPoint;
            }
        }

        return null;
    }

    static void DrawGrid(Dictionary<Point, Material> grid)
    {
        foreach (Point p in grid.Keys)
        {
            Console.SetCursorPosition(p.X - 450, p.Y);
            
            if (grid[p] == Material.Rock)
            {
                Console.Write('#');
            }
            else if (grid[p] == Material.RestedSand)
            {
                Console.Write('o');
            }
        }

    }

    public string SolvePart1(string input)
    {
        (var grid, int abyssLevel) = ParseInput(input);
        
        int count = 0;
        bool reachedAbyss = false;
        while (!reachedAbyss)
        {
            bool rested = false;
            Point pos = (500, 1);
            while (!rested)
            {
                Point? newPos = FlowSand(grid, pos);

                //Console.WriteLine(newPos);

                if (newPos == null)
                {
                    rested = true;
                    count++;
                    grid.Add(pos, Material.RestedSand);
                }
                else
                {
                    pos = newPos;
                    if (newPos.Y > abyssLevel)
                    {
                        reachedAbyss = true;
                        //count--;
                        break;
                    }
                }
            }
        }

        return $"{count}";
    }



    class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point[] PointsTo(Point p, bool inclusive = false)
        {
            if (X == p.X)
            {
                int big = Math.Max(Y, p.Y);
                int small = Math.Min(Y, p.Y);
                Point[] points;

                if (!inclusive)
                {
                    points = new Point[big - small];
                    for (int i = 0; i < big - small; i++)
                    {
                        points[i] = (X, small + i);
                    }
                }
                else
                {
                    points = new Point[big - small + 1];
                    for (int i = 0; i <= big - small; i++)
                    {
                        points[i] = (X, small + i);
                    }
                }
                return points;
            }
            else if (Y == p.Y)
            {
                int big = Math.Max(X, p.X);
                int small = Math.Min(X, p.X);
                Point[] points;

                if (!inclusive)
                {
                    points = new Point[big - small];
                    for (int i = 0; i < big - small; i++)
                    {
                        points[i] = (small + i, Y);
                    }
                }
                else
                {
                    points = new Point[big - small + 1];
                    for (int i = 0; i <= big - small; i++)
                    {
                        points[i] = (small + i, Y);
                    }
                }
                return points;
            }
            else { throw new Exception("Cannot build straight line between points"); }
        }

        public override string ToString() => $"({X}, {Y})";
        public static implicit operator Point(ValueTuple<int, int> tuple) => new(tuple.Item1, tuple.Item2);

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        //public static bool operator ==(Point? a, Point? b) => (a.Equals(null) && b.Equals(null)) || ( !b.Equals(null) && a.X == b.X && a.Y == b.Y);
        //public static bool operator !=(Point? a, Point? b) => a.X != b.X || a.Y != b.Y;

        /*
        public bool Equals(Point? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Point)) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }*/
        
    }
    /*
    class PointComparer : IEqualityComparer<Point>
    {
        public static bool Equals(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public int GetHashCode(Point a)
        {
            return a.X.GetHashCode() + a.Y.GetHashCode();
        }
    }
    */

    static Point FlowSand2(Dictionary<Point, Material> grid, Point p, int abyssLevel)
    {
        var vectors = new Point[]
        {
            ( 0, 1), // Down
            (-1, 1), // Down-Left
            ( 1, 1), // Down-Right
        };

        foreach (var coord in vectors)
        {
            Point newPoint = (p.X + coord.X, p.Y + coord.Y);

            if (newPoint.Y == abyssLevel || PositionBlocked(grid, newPoint)) // grid.ContainsKey(p))
            {
                continue;
            }
            else 
            {
                return newPoint;
            }
        }

        return p;
    }

    public string SolvePart2(string input)
    {
        (var grid, int abyssLevel) = ParseInput(input);
        Console.WriteLine(abyssLevel);
        int count = 0;
        bool completelyFilled = false;
        while (!completelyFilled)
        {
            bool rested = false;
            Point pos = (500, 0);
            while (!rested)
            {
                Point newPos = FlowSand2(grid, pos, abyssLevel);

                if (newPos.X == pos.X && newPos.Y == pos.Y)
                {
                    if (newPos.X == 500 && newPos.Y == 0)
                    {
                        completelyFilled = true;
                    }
                    rested = true;
                    count++;
                    grid.Add(pos, Material.RestedSand);
                }
                else
                {
                    pos = newPos;
                }
            }
            Console.WriteLine($"Sand rested at {pos}");
        }

        return $"{count}";
    }
}
