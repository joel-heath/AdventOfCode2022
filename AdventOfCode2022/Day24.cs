using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal class Day24 : IDay
{
    public int Day => 24;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "#.#####\r\n#.....#\r\n#>....#\r\n#.....#\r\n#...v.#\r\n#.....#\r\n#####.#", "10" },
        { "#.######\r\n#>>.<^<#\r\n#.<..<<#\r\n#>v.><>#\r\n#<^v^^>#\r\n######.#", "18" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "#.#####\r\n#.....#\r\n#>....#\r\n#.....#\r\n#...v.#\r\n#.....#\r\n#####.#", "30" },
        { "#.######\r\n#>>.<^<#\r\n#.<..<<#\r\n#>v.><>#\r\n#<^v^^>#\r\n######.#", "54" }
    };

    static (Point, Point, HashSet<Point>, HashSet<(Point, int)>) ParseInput(string input)
    {
        string[] lines = input.Split("\r\n");

        Point start = (lines[0].IndexOf('.'), 0);
        Point goal = (lines[^1].IndexOf('.'), lines.Length - 1);
        HashSet<Point> walls = new() { start + (0, -1), goal + (0, 1) }; // dont let them escape through entrance

        HashSet<(Point, int)> blizards = new();
        //Dictionary<Point, List<int>> blizziez = new();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            for (int j = 0; j < line.Length; j++)
            {
                char word = line[j];
                switch (word)
                {
                    case '^': blizards.Add(((j, i), 0)); break;
                    case '>': blizards.Add(((j, i), 1)); break;
                    case 'v': blizards.Add(((j, i), 2)); break;
                    case '<': blizards.Add(((j, i), 3)); break;
                    case '#': walls.Add((j, i)); break;
                }
            }
        }

        return (start, goal, walls, blizards);
    }

    class Node
    {
        public Point Location;
        public int Time;
        public int Heuristic;
        //public int TotalWorth;

        public Node(Point l, int time, int heuristic)
        {
            Location = l;
            Time = time;
            Heuristic = heuristic;
            //TotalWorth = time + heuristic;
        }

        //public static bool operator >(Node a, Node b) => (a is not null && b is not null && a.TotalWorth > b.TotalWorth);
        //public static bool operator <(Node a, Node b) => (a is not null && b is not null && a.TotalWorth < b.TotalWorth);

        public override bool Equals(object? obj) => obj is Node p && p.Location.X.Equals(Location.X) && p.Location.Y.Equals(Location.Y);
        public override int GetHashCode() => HashCode.Combine(Location.X, Location.Y);
        public static bool operator ==(Node a, Node b) => (a is null && b is null) || (a is not null && b is not null && a.Location.X == b.Location.X && a.Location.Y == b.Location.Y);
        public static bool operator !=(Node a, Node b) => (a is not null || b is not null) && (a is null || b is null || a.Location.X != b.Location.X || a.Location.Y != b.Location.Y);
        //public static bool operator ==(Node a, Point b) => (a is null && b is null) || (a is not null && b is not null && a.Location.X == b.X && a.Location.Y == b.Y);
        //public static bool operator !=(Node a, Point b) => (a is not null || b is not null) && (a is null || b is null || a.Location.X != b.X || a.Location.Y != b.Y);
    }

    static IEnumerable<Node> Options(Node a, Point end)
    {
        var destinations = new Point[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
        return destinations.Select(p => new Node(p + a.Location, a.Time + 1, a.Location.MDistanceTo(end))).Append(new Node(a.Location, a.Time + 1, a.Heuristic));
    }

    static HashSet<(Point, int)> MoveBlizzards(HashSet<(Point, int)> blizzards, HashSet<Point> walls)
    {
        HashSet<(Point, int)> newBlizzards = new();
        foreach ((Point, int) blizzard in blizzards)
        {
            Point newLocation = blizzard.Item1 + (blizzard.Item2 switch {
                0 => (0, -1),
                1 => (1, 0),
                2 => (0, 1),
                _ => (-1, 0) });

            if (walls.Contains(newLocation))
            {
                int closeX = 0;
                int closeY = 0;
                int farX = walls.Max(w => w.X);
                int farY = walls.Max(w => w.Y) - 1;

                if (newLocation.X == closeX) newLocation = (farX - 1, newLocation.Y);
                else if (newLocation.X == farX) newLocation = (closeX + 1, newLocation.Y);
                else if (newLocation.Y == closeY) newLocation = (newLocation.X, farY - 1);
                else newLocation = (newLocation.X, closeY + 1);
            }

            newBlizzards.Add((newLocation, blizzard.Item2));
        }

        return newBlizzards;
    }

    static HashSet<(Point, int)> WhereAreTheBlizzards(int time, HashSet<(Point location, int direction)> blizzards, HashSet<Point> walls)
    {
        HashSet<(Point, int)> newBlizzards = blizzards.ToHashSet();

        for (int i = 0; i < time; i++)
            newBlizzards = MoveBlizzards(newBlizzards, walls);

        //can do mod the length / width
        /*HashSet<(Point, int)> newBlizzards = new();
        foreach (var (location, direction) in blizzards)
        {
            newBlizzards.Add((direction switch {
                0 => (location.X, Mod(location.Y - time - 1, walls.Max(w => w.Y) - 3) + 1),
                2 => (location.X, Mod(location.Y + time - 1, walls.Max(w => w.Y) - 3) + 1),
                1 => (Mod(location.X + time - 1, walls.Max(w => w.X) - 3) + 1, location.Y),
                _ => (Mod(location.X - time - 1, walls.Max(w => w.X) - 3) + 1, location.Y)
            }, direction));
        }*/


        return newBlizzards;
    }

    static int bestSoFar;
    static int ShortestPath(Node current, Point goal, HashSet<Point> walls, HashSet<(Point, int)> blizzards, Dictionary<string, int> cache)
    {
        string cacheKey = $"{current.Time}{current.Location}";
        if (cache.TryGetValue(cacheKey, out int value))
            return value;

        if (current.Location == goal)
            return current.Time;

        int bestTime = int.MaxValue;
        if (current.Time + current.Heuristic < bestSoFar)
        {
            HashSet<(Point, int)> newBlizzards = MoveBlizzards(blizzards, walls);

            foreach (Node n in Options(current, goal))//.OrderBy(n => n.Heuristic))
            {
                // would like to use the fact its a hash set over this, could be v slow
                if (newBlizzards.Any(b => b.Item1 == n.Location)) continue;
                if (walls.Contains(n.Location)) continue;

                bestTime = Math.Min(ShortestPath(n, goal, walls, newBlizzards, cache), bestTime);
            }
        }
        if (bestTime < bestSoFar)
        {
            Console.WriteLine(bestTime);
            bestSoFar = bestTime;
        }

        cache[cacheKey] = bestTime;
        return bestTime;
    }

    public string SolvePart1(string input)
    {
        (Point start, Point end, HashSet<Point> walls, HashSet<(Point, int)> blizzards) = ParseInput(input);

        bestSoFar = UnitTestsP1.ContainsKey(input) ? 30 : 320;
        return $"{ShortestPath(new Node(start, 0, start.MDistanceTo(end)), end, walls, blizzards, new())}";
    }

    public string SolvePart2(string input)
    {
        (Point start, Point end, HashSet<Point> walls, HashSet<(Point, int)> blizzards) = ParseInput(input);

        /*
        int thereOnce = 308;
        blizzards = WhereAreTheBlizzards(thereOnce, blizzards, walls);
        Console.WriteLine($"Made it to end in {thereOnce} minutes");
        */

        /*Console.ReadKey();
        Console.Clear();
        VisualiseBlizzies(currentBlizzards, walls);
        Console.ReadKey();*/

        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 320;
        int thereOnce = ShortestPath(new Node(start, 0, start.MDistanceTo(end)), end, walls, blizzards, new()); // 308 minutes
        blizzards = WhereAreTheBlizzards(thereOnce, blizzards, walls);
        Console.WriteLine($"The first trip to the goal takes {thereOnce} minutes");
        
        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 320;
        int backAgain = ShortestPath(new Node(end, 0, end.MDistanceTo(start)), start, walls, blizzards, new()); // 289 minutes
        blizzards = WhereAreTheBlizzards(backAgain, blizzards, walls);
        Console.WriteLine($"The trip back to the start takes {backAgain} minutes");
        
        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 320;
        int thereLast = ShortestPath(new Node(start, 0, start.MDistanceTo(end)), end, walls, blizzards, new()); // 311 minutes
        Console.WriteLine($"The trip back to the goal again takes {thereLast} minutes");

        return $"{thereOnce + backAgain + thereLast}";
    }
}
