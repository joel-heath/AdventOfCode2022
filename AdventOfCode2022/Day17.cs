using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal class Day17 : IDay
{
    public int Day => 17;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>", "3068" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>", "1514285714288" }
    };


    static void PushRock(Dictionary<int, HashSet<int>> grid, ref Point[] rock, bool right, int WIDTH=6)
    {
        if (right)
        {
            //if (rock.Where(p => p.X == WIDTH || GridContains(grid, p)).Any()) return;
            Point[] newRock = rock.Select(r => r + (1, 0)).ToArray();
            if (newRock.Where(p => p.X > WIDTH || GridContains(grid, p)).Any()) return;

            rock = newRock;
        }
        else
        {
            Point[] newRock = rock.Select(r => r + (-1, 0)).ToArray();
            if (newRock.Where(p => p.X < 0 || GridContains(grid, p)).Any()) return;

            rock = newRock;
        }
    }

    static bool GridContains(Dictionary<int, HashSet<int>> grid, Point p)
    {
        if (grid.TryGetValue(p.Y, out HashSet<int>? xCoords))
        {
            if (xCoords.Contains(p.X)) return true;
            return false;
        }
        else
        {
            return false;
        }
    }
    static void AddToGrid(Dictionary<int, HashSet<int>> grid, Point p)
    {
        if (grid.TryGetValue(p.Y, out HashSet<int>? xCoords))
        {
            xCoords.Add(p.X);
        }
        else
        {
            grid.Add(p.Y, new HashSet<int>() { p.X });
        }
    }

    static bool DropRock(Dictionary<int, HashSet<int>> grid, ref Point[] rock)
    {
        Point[] newRock = rock.Select(r => r + (0, -1)).ToArray();

        bool positionOccupied = false;
        foreach (Point p in newRock)
        {
            if (GridContains(grid, p) || p.Y == -1)
                positionOccupied = true;
        }

        if (!positionOccupied)
        {
            rock = newRock;
            return false; // rock is still falling
        }
        else
        {
            foreach (Point p in rock)
            {
                AddToGrid(grid, p);
            }
            return true; // rock has settled
        }
    }

    public string SolvePart1(string input)
    {
        int WIDTH = 6;
        /* =====================================
         * does it include final floating rock?
         * =====================================
         * no :)
         */

        // x positions embedded, just add correct y ordinate
        var rockShapes = new Point[][] {
        new Point[] { (2,0), (3,0), (4,0), (5,0) },
        new Point[] { (2,1), (3,0), (3,1), (3,2), (4,1) },
        new Point[] { (2,0), (3,0), (4,0), (4,1), (4,2) },
        new Point[] { (2,0), (2,1), (2,2), (2,3) },
        new Point[] { (2,0), (2,1), (3,0), (3,1) },
        };
        

        //     rock y     rock x
        Dictionary<int, HashSet<int>> grid = new() { { -1, new() { 0, 1, 2, 3, 4, 5, 6 } } };

        Point[] rock = Array.Empty<Point>();
        bool newRock = true;
        for (int i = 0, s = 0; s <= 5009; i++)
        {
            int yLevel = grid.Keys.Max();
            if (newRock)
            {
                //Console.WriteLine($"Rock {s}");
                rock = RockShapes[s++ % RockShapes.Length].Select(p => p + (0, yLevel + 4)).ToArray();
                newRock = false;
            }

            PushRock(grid, ref rock, input[i % input.Length] == '>', WIDTH);
            newRock = DropRock(grid, ref rock);
        }

        // row 0 is row 1, +1 at end
        return $"{grid.Keys.Max() + 1}";
    }

    static readonly Point[][] RockShapes = new Point[][] {
            new Point[] { (2,0), (3,0), (4,0), (5,0) },
            new Point[] { (2,1), (3,0), (3,1), (3,2), (4,1) },
            new Point[] { (2,0), (3,0), (4,0), (4,1), (4,2) },
            new Point[] { (2,0), (2,1), (2,2), (2,3) },
            new Point[] { (2,0), (2,1), (3,0), (3,1) },
        };

    static (int rows, int rocks) DropUntilIts(long iterations, Dictionary<int, HashSet<int>> grid, string input, ref Point[] rock, ref bool newRock, ref long inputPos, ref long shapeIndex)
    {
        var droppedRocks = 0;
        for (long i = 0; i < iterations; i++)
        {
            int yLevel = grid.Keys.Max();
            if (newRock)
                rock = RockShapes[shapeIndex++ % RockShapes.Length].Select(p => p + (0, yLevel + 4)).ToArray();
            //newRock = false;

            PushRock(grid, ref rock, input[(int)((i + inputPos) % input.Length)] == '>');
            newRock = DropRock(grid, ref rock);
            if (newRock) droppedRocks++;
        }

        inputPos = (inputPos + iterations) % input.Length;
        return (grid.Keys.Max() + 1, droppedRocks);
    }

    static long DropUntilRocks(long rocksToDrop, Dictionary<int, HashSet<int>> grid, string input, ref Point[] rock, ref bool newRock, ref long shapeIndex)
    {
        int droppedRocks = 0;
        for (long i = 0; droppedRocks < rocksToDrop; i++)
        {
            int yLevel = grid.Keys.Max();
            if (newRock)
            {
                rock = RockShapes[shapeIndex++ % RockShapes.Length].Select(p => p + (0, yLevel + 4)).ToArray();
            }
            //newRock = false;

            PushRock(grid, ref rock, input[(int)((i) % input.Length)] == '>');
            newRock = DropRock(grid, ref rock);
            if (newRock) droppedRocks++;
        }

        //inputPos = (inputPos + rocksToDrop) % input.Length;
        return grid.Keys.Max() + 1;
    }

    public string SolvePart2(string input)
    {
        // x positions embedded, just add correct y ordinate
        var rockShapes = new Point[][] {
            new Point[] { (2,0), (3,0), (4,0), (5,0) },
            new Point[] { (2,1), (3,0), (3,1), (3,2), (4,1) },
            new Point[] { (2,0), (3,0), (4,0), (4,1), (4,2) },
            new Point[] { (2,0), (2,1), (2,2), (2,3) },
            new Point[] { (2,0), (2,1), (3,0), (3,1) },
        };

        int repeat = input.Length * rockShapes.Length;

        Dictionary<int, HashSet<int>> grid = new() { { -1, new() { 0, 1, 2, 3, 4, 5, 6 } } };

        Point[] rock = Array.Empty<Point>();
        bool newRock = true;
        long inputPos = 0;
        long shapeIndex = 0;

        long rocksToDropRemaining = 1000000000000;

        // FIRST RUN THROUGH
        (long initToalRows, long rocksAdded) = DropUntilIts(repeat, grid, input, ref rock, ref newRock, ref inputPos, ref shapeIndex);

        rocksToDropRemaining -= rocksAdded;

        Console.WriteLine($"Drops {rocksAdded} and adds {initToalRows} rows");


        // Second time and after are all the same
        (int newTotalRows, rocksAdded) = DropUntilIts(repeat, grid, input, ref rock, ref newRock, ref inputPos, ref shapeIndex);

        long rowsAdded = newTotalRows - initToalRows;
        long timesToRepeat = rocksToDropRemaining / rocksAdded;
        long totalRows = initToalRows + timesToRepeat * rowsAdded;

        Console.WriteLine($"Every period of {repeat} drops {rocksAdded} and adds {rowsAdded} rows");
        Console.WriteLine($"Need {timesToRepeat} period");

        // now just remaining left to do
        long finalRocksToAdd = rocksToDropRemaining % rocksAdded;

        long finalTotalRows = DropUntilRocks(finalRocksToAdd, grid, input, ref rock, ref newRock, ref shapeIndex);

        finalTotalRows -= newTotalRows;

        Console.WriteLine($"Need to drop {finalRocksToAdd} rocks after the periods");
        Console.WriteLine($"Final total rows: {finalTotalRows}. (Added to the old total, {totalRows})");

        return $"{totalRows + finalTotalRows}";
    }
}
