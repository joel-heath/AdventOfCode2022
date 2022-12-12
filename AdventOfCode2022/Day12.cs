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
        { "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi", "29" }
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
    static T[,] To2D<T>(T[][] source)
    {
        var result = new T[source.Length, source[0].Length];
        Enumerable.Range(0, source.Length).ToList().ForEach(r => Enumerable.Range(0, source[0].Length).ToList().ForEach(c => result[r, c] = source[r][c]));
        return result;
    }

    public class Node
    {
        public Node? p; // parent
        public Point s; // self
        public int g; // distance from origin

        public Node(Point self, Node? parent)
        {
            this.p = parent;
            this.s = self;
            this.g = Equals(parent, null) ? 0 : p.g + 1;
        }

        public Node Add(Point b) => new ((s.y + b.y, s.x + b.x), this);
        public static bool operator ==(Node a, Point? b) => b == null ? false : a.s.y == b.y && a.s.x == b.x;
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
        
        public static implicit operator Point(ValueTuple<int, int> coords) => new(coords.Item1, coords.Item2);
        public static explicit operator Point(Node n) => n.s;
    }

    static bool IsValid(Point s, Point d, int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        if (d.y < 0 || d.y >= rows || d.x < 0 || d.x >= cols || s.y < 0 || s.y >= rows || s.x < 0 || s.x >= cols) return false;

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
    
    static (List<Node>, Point) PathFind(Point start, int[,] map, Point? end = null)
    {
        int ROW = map.GetLength(0);
        int COL = map.GetLength(1);

        List<Node> open = new () { new (start, null) };
        List<Node> closed = new ();

        Node currentNode = new (start, null);

        while (open.Count > 0)
        {
            Point[] vectors = { (1, 0), (-1, 0), (0, 1), (0, -1) };

            currentNode = open.MinBy(n => n.g)!;
            open.Remove(currentNode);
            closed.Add(currentNode);

            if (end == null && map[currentNode.s.y, currentNode.s.x] == 0) { break; }
            else if (currentNode == end) { break; }
            
            foreach (Point vector in vectors)
            {
                Node newNode = currentNode.Add(vector);

                if (IndexOf(closed, (Point)newNode) != -1) { continue; }

                if (end != null) { if (!IsValid((Point)currentNode, (Point)newNode, map)) { continue; } }
                else             { if (!IsValid((Point)newNode, (Point)currentNode, map)) { continue; } }

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

        return (closed, currentNode.s);
    }

    public string SolvePart1(string input)
    {
        (int[,] map, Point start, Point end) = ParseInput(input);
        (List<Node> closed, _)= PathFind(start, map, end);

        return $"{closed[IndexOf(closed, end)].g}";
    }

    public string SolvePart2(string input)
    {
        (int[,] map, _, Point end) = ParseInput(input);
        (List<Node> closed, Point start) = PathFind(end, map);

        return $"{closed[IndexOf(closed, start)].g}";
    }
}
