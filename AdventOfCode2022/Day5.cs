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

    static (Stack<char>[], int) ParseInput(string[] lines)
    {
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

        Stack<char>[] crates = new Stack<char>[crateCount].Select(s => new Stack<char>()).ToArray();

        for (int i = intialCrates - 1; i >= 0; i--)
        {
            string line = lines[i];

            for (int j = 0, k = 1; j < crateCount; j++, k += 4)
            {
                if (line[k] == ' ') continue;
                crates[j].Push(line[k]);
            }
        }

        return (crates, intialCrates+2);
    }

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");
        (Stack<char>[] crates, int instructionLine) = ParseInput(lines);

        for (int i = instructionLine; i < lines.Length; i++)
        {
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            Enumerable.Range(0, nums[0]).Select(n => crates[nums[1] - 1].Pop()).ToList().ForEach(c => crates[nums[2] - 1].Push(c));
        }

        return new string(crates.Select(s => s.Peek()).ToArray());
    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");
        (Stack<char>[] crates, int instructionLine) = ParseInput(lines);

        for (int i = instructionLine; i < lines.Length; i++)
        {
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            Enumerable.Range(0, nums[0]).Select(n => crates[nums[1] - 1].Pop()).Reverse().ToList().ForEach(c => crates[nums[2] - 1].Push(c));
        }

        return new string(crates.Select(s => s.Peek()).ToArray());
    }

    [GeneratedRegex("move (\\d+) from (\\d+) to (\\d+)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MoveFromTo();
}
