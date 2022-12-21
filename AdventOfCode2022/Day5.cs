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

    static (Stack<char>[], int, string[]) ParseInput(string[] lines)
    {
        int initialCrates = lines.Where(l => l.Length != 0 && l[0] != 'm').Count() - 1;
        int crateCount = lines[initialCrates].Where(c => c != '[' && c != ' ' && c != ']').Count();
        Stack<char>[] crates = Enumerable.Repeat(new string(' ', lines[0].Length), crateCount - initialCrates).Concat(lines).Reverse().Skip(lines.Length - initialCrates)
            .Select(l => l.Where((_, i) => (i - 1) % 4 == 0).ToArray()).ToArray()
            .Rotate().Select(r => new Stack<char>(r.Reverse().Where(c => c != ' '))).ToArray();

        return (crates, initialCrates + 2, lines);
    }

    public string SolvePart1(string input)
    {
        (Stack<char>[] crates, int i, string[] lines) = ParseInput(input.Split("\r\n"));

        for (; i < lines.Length; i++)
        {
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            Enumerable.Range(0, nums[0]).Select(n => crates[nums[1] - 1].Pop()).ToList().ForEach(c => crates[nums[2] - 1].Push(c));
        }

        return new string(crates.Select(s => s.Peek()).ToArray());
    }
    public string SolvePart2(string input)
    {
        (Stack<char>[] crates, int i, string[] lines) = ParseInput(input.Split("\r\n"));

        for (; i < lines.Length; i++)
        {
            int[] nums = MoveFromTo().Matches(lines[i])[0].Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            Enumerable.Range(0, nums[0]).Select(n => crates[nums[1] - 1].Pop()).Reverse().ToList().ForEach(c => crates[nums[2] - 1].Push(c));
        }

        return new string(crates.Select(s => s.Peek()).ToArray());
    }

    [GeneratedRegex("move (\\d+) from (\\d+) to (\\d+)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MoveFromTo();
}
static class MyExtensions
{
    public static T[][] Rotate<T>(this T[][] matrix)
    {
        int n = matrix.GetLength(0);
        T[][] ret = new T[n][].Select(i => new T[n]).ToArray();

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i][j] = matrix[n - j - 1][i];
            }
        }

        return ret;
    }
}