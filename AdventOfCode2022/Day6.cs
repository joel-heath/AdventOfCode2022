using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal class Day6 : IDay
{
    public int Day => 6;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" },
        { "Input2", "Output" },
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

    static string[] GetMatches(string input, string regex)
    {
        return new Regex(regex, RegexOptions.IgnoreCase).Matches(input).Skip(1).Select(m => m.Value).ToArray();
    }


    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        Dictionary<string, int> result = new Dictionary<string, int>();
        int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ").Select(s => s.Trim(' ')).ToArray();



            for (int j = 0; j < words.Length; j++)
            {
                string word = words[j];
                if (word == "") continue;


            }
        }





        return $"{string.Empty}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }
}
