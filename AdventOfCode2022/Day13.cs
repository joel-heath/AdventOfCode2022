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
using System.Diagnostics.CodeAnalysis;

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
        { "[1,1,3,1,1]\r\n[1,1,5,1,1]\r\n\r\n[[1],[2,3,4]]\r\n[[1],4]\r\n\r\n[9]\r\n[[8,7,6]]\r\n\r\n[[4,4],4,4]\r\n[[4,4],4,4,4]\r\n\r\n[7,7,7,7]\r\n[7,7,7]\r\n\r\n[]\r\n[3]\r\n\r\n[[[]]]\r\n[[]]\r\n\r\n[1,[2,[3,[4,[5,6,7]]]],8,9]\r\n[1,[2,[3,[4,[5,6,0]]]],8,9]", "140" }
    };

    static (List<object>, int) ParsePacket(string input, int index)
    {
        var packet = new List<object>();
        string currNum = string.Empty;

        for (; index < input.Length; index++)
        {
            char l = input[index];
            if (l == '[')
            {
                var child = ParsePacket(input, ++index);
                index = child.Item2;
                packet.Add(child.Item1);
            }
            else if (l == ',')
            {
                if (currNum != string.Empty) packet.Add(int.Parse(currNum));
                currNum = string.Empty;
            }
            else if (l == ']')
            {
                if (currNum != string.Empty) packet.Add(int.Parse(currNum));
                return (packet, index);
            }
            else
            {
                currNum += l;
            }
        }

        return (packet, index);
    }

    public static List<(List<object>, List<object>)> ParseInput(string input)
    {
        string[] groups = input.Split("\r\n\r\n");

        List<(List<object>, List<object>)> allPairs = new();

        for (int i = 0; i < groups.Length; i++)
        {
            string[] lines = groups[i].Split("\r\n");

            allPairs.Add((ParsePacket(lines[0], 0).Item1, ParsePacket(lines[1], 0).Item1));
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
        int count = 0;

        for (int i = 0; i < pairs.Count; i++)
        {
            List<object> left = pairs[i].Item1;
            List<object> right = pairs[i].Item2;

            Result result = ComparePair(left, right);

            if (result == Result.Ordered)
            {
                count += i + 1;
            }
        }

        return $"{count}";
    }

    static void Sort(List<List<object>> list)
    {
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (ComparePair(list[i], list[i+1]) == Result.Unordered)
                {
                    (list[i], list[i+1]) = (list[i+1], list[i]);
                    sorted = false;
                }
            }
        }
    }

    public string SolvePart2(string input)
    {
        List<List<object>> packets = ParseInput(input + "\r\n\r\n[[2]]\r\n[[6]]").SelectMany(s => new List<List<object>> { s.Item1, s.Item2 }).ToList();

        Sort(packets);

        int result = 1;
        int count = 0;
        for (int i = 0; i < packets.Count && count < 2; i++)
        {
            List<object> p = packets[i];
            try
            {
                if (p.Count == 1)
                {
                    var child = (List<object>)p[0];
                    if (child.Count == 1)
                    {
                        var grandchild = (List<object>)child[0];
                        if (grandchild.Count == 1)
                        {
                            var greatgrandchild = (int)grandchild[0];
                            if (greatgrandchild == 6 || greatgrandchild == 2)
                            {
                                result *= i + 1;
                                count++;
                            }
                        }
                    }
                }
            }
            catch { continue; }
        }

        return $"{result}";
    }
}
