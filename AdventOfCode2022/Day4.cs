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
        { "TestInput", "Output" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
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

        /*
        Regex r = new(regex, RegexOptions.IgnoreCase);
        MatchCollection matches = r.Matches(input);
        string[] result = matches.Skip(1).Select(m => m.Value).ToArray();
        string[] result = new string[matches.Count - 1];

        foreach (Match m in matches.Cast<Match>())
        {
            GroupCollection g = m.Groups;

            //g[0] is the overarching capture of the whole thing
            // skip it with index at 1

            for (int gi = 1; gi < g.Count; gi++)
            {
                result[gi-1] = g[gi].Value;
            }
        }

        return result;
        */
    }


    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        Dictionary<string, int> result = new();
        //int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ").Select(s => s.Trim(' ')).ToArray();



            for (int j = 0; j < words.Length; j++)
            {
                string word = words[j];
                if (word == "") continue;


            }
        }



        // OR REGEX ROUTE
        Regex r = new("(\\d+),(\\d+) => (\\d+),(\\d+)", RegexOptions.IgnoreCase);
        MatchCollection matches = r.Matches(input);

        foreach (Match m in matches)
        {
            GroupCollection g = m.Groups;

            //g[0] is the overarching capture of the whole thing
            // skip it with index at 1
            for (int gi = 1; gi < g.Count; gi++)
            {
                Console.Write($"{g[gi].Value} ");
            }

            Console.WriteLine();
        }



        return $"{string.Empty}";
    }

    public string SolvePart2(string input)
    {





        return $"{string.Empty}";
    }
}
