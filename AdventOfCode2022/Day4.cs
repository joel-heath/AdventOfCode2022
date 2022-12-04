using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022;
internal class Day4 : IDay
{
    public int Day => 4;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", "2" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", "4" }
    };

    static void AddToDict<TKey>(Dictionary<TKey,int> dict, TKey key, int value)
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

    static string[] GetMatches(string input, string regex)
    {
        return new Regex(regex, RegexOptions.IgnoreCase).Matches(input).Skip(1).Select(m => m.Value).ToArray();
    }


    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");
        int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] ranges = lines[i].Split(",");
            int[] range1 = ranges[0].Split("-").Select(int.Parse).ToArray();
            int[] range2 = ranges[1].Split("-").Select(int.Parse).ToArray();

            if ((range2[1] >= range1[1] && range2[0] <= range1[0]) || (range1[1] >= range2[1] && range1[0] <= range2[0]))
            {
                count++;
            }

        }

        return $"{count}";

    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");

        int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] ranges = lines[i].Split(",");
            int[] bounds1 = ranges[0].Split("-").Select(int.Parse).ToArray();
            int[] bounds2 = ranges[1].Split("-").Select(int.Parse).ToArray();

            int[] range1 = new int[bounds1[1] - bounds1[0] + 1];
            int[] range2 = new int[bounds2[1] - bounds2[0] + 1];

            for (int j = bounds1[0]; j <= bounds1[1]; j++)
            {
                
                range1[j - bounds1[0]] = j;
            }

            for (int j = bounds2[0]; j <= bounds2[1]; j++)
            {
                range2[j - bounds2[0]] = j;
            }

            if (range1.Intersect(range2).ToArray().Length > 0) count++;

        }

        return $"{count}";
    }
}
