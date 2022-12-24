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
        //{ "#.#####\r\n#.....#\r\n#>....#\r\n#.....#\r\n#...v.#\r\n#.....#\r\n#####.#", "10" },
        { "#.######\r\n#>>.<^<#\r\n#.<..<<#\r\n#>v.><>#\r\n#<^v^^>#\r\n######.#", "18" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "#.######\r\n#>>.<^<#\r\n#.<..<<#\r\n#>v.><>#\r\n#<^v^^>#\r\n######.#", "54" }
    };

    static void AddBlizzard(Dictionary<Point, List<int>> dict, Point point, int direction)
    {
        if (dict.TryGetValue(point, out var dirs))
            dirs.Add(direction);
        else
            dict[point] = new List<int> { direction };
    }

    static (Point, Point, HashSet<Point>, HashSet<(Point, int)>) ParseInput(string input)
    {
        string[] lines = input.Split("\r\n");

        Point start = (lines[0].IndexOf('.'), 0);
        Point goal = (lines[^1].IndexOf('.'), lines.Length - 1);
        HashSet<Point> walls = new() { start - (0, 1), goal + (0, 1) }; // dont let them escape through entrance

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
                var row = walls.Where(w => w.Y == newLocation.Y);
                int closeX = row.Min(w => w.X);
                int farX = row.Max(w => w.X);
                var col = walls.Where(w => w.X == newLocation.X);
                int closeY = col.Min(w => w.Y);
                int farY = col.Max(w => w.Y);

                if (newLocation.X == closeX) newLocation = (farX - 1, newLocation.Y);
                else if (newLocation.X == farX) newLocation = (closeX + 1, newLocation.Y);
                else if (newLocation.Y == closeY) newLocation = (newLocation.X, farY - 1);
                else newLocation = (newLocation.X, closeY + 1);
            }

            newBlizzards.Add((newLocation, blizzard.Item2));
        }

        return newBlizzards;
    }

    static HashSet<(Point, int)> WhereAreTheBlizzards(int time, HashSet<(Point, int)> blizzards, HashSet<Point> walls)
    {
        HashSet<(Point, int)> newBlizzards = blizzards.ToHashSet();
        for (int i = 0; i < time % newBlizzards.Count; i++)
            newBlizzards = MoveBlizzards(newBlizzards, walls);
        // could even speed this up a lot with modolo
        return newBlizzards;
    }

    static int bestSoFar;
    static int DistanceTo(Node current, Point goal, HashSet<Point> walls, HashSet<(Point, int)> blizzards, Dictionary<string, int> cache)
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

                bestTime = Math.Min(DistanceTo(n, goal, walls, newBlizzards, cache), bestTime);
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

        bestSoFar = UnitTestsP1.ContainsKey(input) ? 30 : 400;
        return $"{DistanceTo(new Node(start, 0, start.MDistanceTo(end)), end, walls, blizzards, new())}";
    }

    public string SolvePart2(string input)
    {
        (Point start, Point end, HashSet<Point> walls, HashSet<(Point, int)> initBlizzards) = ParseInput(input);

        /* SKIP for my input
        int thereOnce = 308;
        HashSet<(Point, int)> currentBlizzards = WhereAreTheBlizzards(thereOnce, initBlizzards, walls);
        Console.WriteLine($"Made it to end in {thereOnce} minutes");
        */

        HashSet<(Point, int)> currentBlizzards = new(initBlizzards);

        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 400;
        int thereOnce = DistanceTo(new Node(start, 0, start.MDistanceTo(end)), end, walls, currentBlizzards, new());
        currentBlizzards = WhereAreTheBlizzards(thereOnce, initBlizzards, walls);
        Console.WriteLine($"Made it to end in {thereOnce} minutes");

        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 400;
        int backAgain = DistanceTo(new Node(end, 0, end.MDistanceTo(start)), start, walls, currentBlizzards, new());
        currentBlizzards = WhereAreTheBlizzards(thereOnce + backAgain + 2, initBlizzards, walls);
        Console.WriteLine($"Made it back again in {backAgain} minutes");
        
        bestSoFar = UnitTestsP2.ContainsKey(input) ? 30 : 400;
        int thereLast = DistanceTo(new Node(start, 0, start.MDistanceTo(end)), end, walls, currentBlizzards, new());
        Console.WriteLine($"Made it to the finish in {thereLast} minutes");

        return $"{thereOnce + backAgain + thereLast}";
    }
}
