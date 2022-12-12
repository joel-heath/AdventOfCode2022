using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static AdventOfCode2022.Day12;

namespace AdventOfCode2022;
internal class Day12 : IDay
{
    public int Day => 12;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi", "31" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static (int[,], Point, Point) ParseInput(string input)
    {
        int[][] map = input.Split("\r\n").Select(l => l.Select(c => c - 'a').ToArray()).ToArray();
        int startY = map.Select((l, i) => (l, i)).Where(l => l.l.Contains('S' - 'a')).First().i;
        int startX = map[startY].Select((c, i) => (c, i)).Where(c => c.c < 0).First().i;
        map[startY][startX] = 0; // elevation 'a'

        int endY = map.Select((l, i) => (l, i)).Where(l => l.l.Contains('E' - 'a')).First().i;
        int endX = map[endY].Select((c, i) => (c, i)).Where(c => c.c < 0).First().i;
        map[endY][endX] = 25; // elevation 'z'

        return (To2D(map), (startY, startX), (endY, endX));
    }

    public class Node
    {
        public Node? p; // parent
        public Point s;
        public int g; // distance from origin
        public int h; // heuristic
        public int f; // total
        public Point end;

        public Node(Point self, Node? parent, Point end)
        {
            this.p = parent;
            this.s = self;
            this.g = Equals(parent, null) ? 0 : p.g + 1;
            this.h = self.DistanceFrom(end);
            this.f = this.g + this.h;
            this.end = end;
        }

        public Node Add(Point b) => new ((s.y + b.y, s.x + b.x), this, end);
        public static bool operator ==(Node a, Point b) => a.s.y == b.y && a.s.x == b.x;
        public static bool operator !=(Node a, Point b) => a.s.y != b.y || a.s.x != b.x;
    }

    public class Point
    {
        public int y;
        public int x;

        public Point(int y, int x)
        {
            this.y = y;
            this.x = x;
        }

        public int DistanceFrom(Point p)
        => Math.Abs(p.y - this.y) + Math.Abs(p.x - this.x);

        public int DistanceFromO()
        => this.y + this.x;

        public static Point operator +(Point a, Point b) => (a.y + b.y, a.x + b.x);
        
        public static implicit operator int(Point a) => a.y + a.x;

        public static implicit operator Point(Tuple<int,int> coords) => new(coords.Item1,coords.Item2);
        public static implicit operator Point(ValueTuple<int, int> coords) => new(coords.Item1, coords.Item2);
        public static explicit operator Point(Node n) => n.s;
    }

    static bool IsValid(Point s, Point d, int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        if (d.y < 0 || d.y >= rows || d.x < 0 || d.x >= cols) return false;

        return map[d.y, d.x] - map[s.y, s.x] <= 1;
    }

    static int IndexOf(List<Node> list, Point p)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Node node = list[i];
            if (node.s.y == p.y && node.s.x == p.x) return i;
        }
        return -1;
    }

    static int PathFind(Point start, Point end, int[,] map)
    {
        Console.WriteLine();
        int ROW = map.GetLength(0);
        int COL = map.GetLength(1);

        List<Node> open = new () { new Node(start, null, end) };
        List<Node> closed = new ();

        while (IndexOf(closed, end) == -1 && open.Count > 0)
        {
            Point[] vectors = { (1, 0), (-1, 0), (0, 1), (0, -1) };

            Node currentNode = open.MinBy(n => n.f)!;
            open.Remove(currentNode);
            closed.Add(currentNode);

            if (currentNode == end) { break; }

            foreach (Point vector in vectors)
            {
                Node newNode = currentNode.Add(vector);

                if (IndexOf(closed, (Point)newNode) != -1) { continue; }
                
                if (!IsValid((Point)currentNode, (Point)newNode, map)) { continue; }

                if (IndexOf(open, (Point)newNode) == -1)
                {
                    open.Add(newNode);
                }
                else
                {
                    if (newNode.g < open[IndexOf(open, (Point)newNode)].g)
                    {
                        open[IndexOf(open, (Point)newNode)] = newNode;
                    }
                }
            }
        }

        if (IndexOf(closed, end) == -1) return -1;

        Node curr = closed[IndexOf(closed, end)];
        int count = 0;

        while (!Equals(curr.p, null))
        {
            curr = curr.p; count++;
        }

        return count;
    }

    static T[,] To2D<T>(T[][] source)
    {
        try
        {
            int FirstDim = source.Length;
            int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

            var result = new T[FirstDim, SecondDim];
            for (int i = 0; i < FirstDim; ++i)
                for (int j = 0; j < SecondDim; ++j)
                    result[i, j] = source[i][j];

            return result;
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("The given jagged array is not rectangular.");
        }
    }

    public string SolvePart1(string input)
    {
        (int[,] map, Point start, Point end) = ParseInput(input);
        Console.WriteLine($"Start: ({start.y}, {start.x})");
        Console.WriteLine($"Goal: ({end.y}, {end.x})");

        int distance = PathFind(start, end, map);

        return $"{distance}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }
}
