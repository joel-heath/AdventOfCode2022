using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;

namespace AdventOfCode2022;
internal class Day13 : IDay
{
    public int Day => 13;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "[1,1,3,1,1]\r\n[1,1,5,1,1]\r\n\r\n[[1],[2,3,4]]\r\n[[1],4]\r\n\r\n[9]\r\n[[8,7,6]]\r\n\r\n[[4,4],4,4]\r\n[[4,4],4,4,4]\r\n\r\n[7,7,7,7]\r\n[7,7,7]\r\n\r\n[]\r\n[3]\r\n\r\n[[[]]]\r\n[[]]\r\n\r\n[1,[2,[3,[4,[5,6,7]]]],8,9]\r\n[1,[2,[3,[4,[5,6,0]]]],8,9]", "13" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static void Insert(List<object> list, List<int> indices, int itemToInsert)
    {
        if (indices.Count > 1)
        {
            List<object> child;
            try { child = (List<object>)list[indices[0]]; }
            catch (ArgumentOutOfRangeException) { child = new List<object>(); list.Add(child); }

            Insert(child, indices.Skip(1).ToList(), itemToInsert);
        }
        else
        {
            list.Add(itemToInsert);
        }
    }

    static List<object> ParsePacket(string input)
    {
        var packet = new List<object>();
        var indices = new List<int>() { -1 };

        var r = new Regex(@"(\d+|\[|]|,)+");
        IEnumerable<string> characters = r.Matches(input).First().Groups[1].Captures.Select(c => c.Value);

        foreach (string l in characters)
        {
            if (l[0] == '[')
            {
                packet.Add(new List<object>());
                indices[^1]++;
                indices.Add(-1);

            }
            else if (l[0] == ']')
            {
                indices.RemoveAt(indices.Count - 1);
            }
            else if (l[0] == ',')
            {
                continue;
            }
            else
            {
                indices[^1]++;
                Insert(packet, indices, int.Parse(l));
            }
        }

        return ((List<object>)packet[0]).Where(p => p is int || ((List<object>)p).Count > 0).ToList();
    }

    public static List<(List<object>, List<object>)> ParseInput(string input)
    {
        string[] groups = input.Split("\r\n\r\n");

        List<(List<object>, List<object>)> allPairs = new();

        for (int i = 0; i < groups.Length; i++)
        {
            string[] lines = groups[i].Split("\r\n");

            allPairs.Add((ParsePacket(lines[0]), ParsePacket(lines[1])));
        }

        return allPairs;
    }

    enum Result { Equal, Ordered, Unordered }

    static Result ComparePair(object left, object right)
    {
        if ((left is List<object>) || (right is List<object>))
        {
            List<object> leftList;
            List<object> rightList;

            if (right is not List<object>)
                rightList = new List<object>() { (int)right };
            else
                rightList = (List<object>)right;

            if (left is not List<object>)
                leftList = new List<object>() { (int)left };
            else
                leftList = (List<object>)left;

            for (int j = 0; j < Math.Max(leftList.Count, rightList.Count); j++)
            {
                if (j >= rightList.Count) return Result.Unordered;
                if (j >= leftList.Count) return Result.Ordered;

                Result r = ComparePair(leftList[j], rightList[j]);

                if (r == Result.Ordered) return Result.Ordered;
                if (r == Result.Unordered) return Result.Unordered;
            }

            return Result.Equal;
        }
        else
        {
            int leftInt = (int)left;
            int rightInt = (int)right;

            //Console.WriteLine($"Comparing {leftInt} vs {rightInt}");

            if (rightInt < leftInt)
            {
                return Result.Unordered;
            }
            if (leftInt < rightInt)
            {
                return Result.Ordered;
            }
            
            return Result.Equal;
        }
    }

    public string SolvePart1(string input)
    {
        List<(List<object>, List<object>)> pairs = ParseInput(input);
        Console.WriteLine("Parsed!");
        int count = 0;

        for (int i = 0; i < pairs.Count; i++)
        {
            List<object> left = pairs[i].Item1;
            List<object> right = pairs[i].Item2;

            Result result = ComparePair(left, right);

            if (result == Result.Ordered)
            {
                //Console.WriteLine($"Pair {i+1} is good!");
                count += i + 1;
            }
        }

        // 1163 not correct
        // 3536 too low
        // 5487 too low

        // 5775 INCREDIBLY SLOW

        return $"{count}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }
}
