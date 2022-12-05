using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal partial class Day5 : IDay
{
    public int Day => 5;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "    [D]    \r\n[N] [C]    \r\n[Z] [M] [P]\r\n 1   2   3 \r\n\r\nmove 1 from 2 to 1\r\nmove 3 from 1 to 3\r\nmove 2 from 2 to 1\r\nmove 1 from 1 to 2", "CMZ" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "    [D]    \r\n[N] [C]    \r\n[Z] [M] [P]\r\n 1   2   3 \r\n\r\nmove 1 from 2 to 1\r\nmove 3 from 1 to 3\r\nmove 2 from 2 to 1\r\nmove 1 from 1 to 2", "MCD" }
    };

    static string[] GetMatches(string input, string regex)
    {
        return new Regex(regex, RegexOptions.IgnoreCase).Matches(input).Skip(1).Select(m => m.Value).ToArray();
    }


    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        int intialCrates = -1;
        for (int i = 0; i < lines.Length && intialCrates == -1; i++)
        {
            if (lines[i].Length < 2 || lines[i][0] == '[') continue;
            if (lines[i][1] == '1') intialCrates = i;
        }

        int crateCount = 0;
        for (int i = 0; i < lines[intialCrates].Length; i++)
        {
            char c = lines[intialCrates][i];
            if (c != '[' && c != ' ' && c != ']') crateCount++;
        }

        Stack<char>[] parts = new Stack<char>[crateCount].Select(s => new Stack<char>()).ToArray();

        for (int i = intialCrates-1; i >= 0; i--)
        {
            string line = lines[i];

            for (int j = 0, k = 1; j < crateCount; j++, k += 4)
            {
                if (line[k] == ' ') continue;
                parts[j].Push(line[k]);
            }
        }


        for (int i = intialCrates+2; i < lines.Length; i++)
        {
            string line = lines[i];
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();

            for (int j = 0; j < nums[0]; j++)
            {
                parts[nums[2]-1].Push( parts[nums[1]-1].Pop() );
            }
        }

        string result = string.Empty;

        foreach (Stack<char> stack in parts)
        {
            result += stack.Pop();
        }

        return $"{result}";
    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");

        int intialCrates = -1;
        for (int i = 0; i < lines.Length && intialCrates == -1; i++)
        {
            if (lines[i].Length < 2 || lines[i][0] == '[') continue;
            if (lines[i][1] == '1') intialCrates = i;
        }

        int crateCount = 0;
        for (int i = 0; i < lines[intialCrates].Length; i++)
        {
            char c = lines[intialCrates][i];
            if (c != '[' && c != ' ' && c != ']') crateCount++;
        }

        Stack<char>[] parts = new Stack<char>[crateCount].Select(s => new Stack<char>()).ToArray();

        for (int i = intialCrates - 1; i >= 0; i--)
        {
            string line = lines[i];

            for (int j = 0, k = 1; j < crateCount; j++, k += 4)
            {
                if (line[k] == ' ') continue;
                parts[j].Push(line[k]);
            }
        }


        for (int i = intialCrates + 2; i < lines.Length; i++)
        {
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();

            List<char> temp = new();

            for (int j = 0; j < nums[0]; j++)
            {
                temp.Add(parts[nums[1] - 1].Pop());
            }

            for (int j = temp.Count - 1; j >= 0; j--)
            {
                parts[nums[2] - 1].Push(temp[j]);
            }
        }

        string result = string.Empty;

        foreach (Stack<char> stack in parts)
        {
            result += stack.Pop();
        }

        return $"{result}";
    }

    [GeneratedRegex("move (\\d+) from (\\d+) to (\\d+)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MoveFromTo();
}
