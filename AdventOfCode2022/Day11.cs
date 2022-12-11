using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2022;
internal class Day11 : IDay
{
    public int Day => 11;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1", "10605" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1", "2713310158" }
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

    static T?[] GetNeighbours<T>(T[,] grid, int y, int x, bool diagonals = false)
    {
        var coordsToCheck = new (int, int)[]
        {
            (y - 1, x), // Up
            (y, x + 1), // Right
            (y + 1, x), // Down
            (y, x - 1), // Left
        };

        if (diagonals)
        {
            coordsToCheck = coordsToCheck.Concat(new (int, int)[]
            {
                (y - 1, x + 1), // Up-Right
                (y + 1, x + 1), // Down-Right
                (y + 1, x - 1), // Down-Left
                (y - 1, x - 1), // Up-Left

            }).ToArray();
        }

        return coordsToCheck.Select(c => y < grid.GetLength(0) && x < grid.GetLength(1) ? grid[c.Item1, c.Item2] : default).ToArray();
    }

    class Monkey
    {
        public List<Int128> items;
        public int opcode; // 0 is * 1 is +
        public int? operand;
        public int test;
        public int ifTrue;
        public int ifFalse;
        public long inspectionCount = 0;

    }

    // Euclid's algorithm
    static int Gfc(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }
    static int Lcm(int a, int b) => a * b / Gfc(a, b);

    static Monkey[] ParseInput(string input)
    {
        string[] rawMonkeys = input.Split("\r\n\r\n").Where(n => n != string.Empty).ToArray();
        Monkey[] monkeys = new Monkey[rawMonkeys.Length];
        Regex r = new (@"(\d+)");

        for (int m = 0; m < rawMonkeys.Length; m++)
        {
            string[] line = rawMonkeys[m].Split("\r\n").Select(s => s.Trim(' ')).ToArray();

            
            var startingItems = r.Matches(line[1]).Select(m => Int128.Parse(m.Groups.Cast<Group>().First().Value)).ToList();
            int opcode = line[2].Contains("*") ? 0 : 1;

            int? operand = null;
            try { operand = r.Matches(line[2]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First(); }
            catch { }
            int test = r.Matches(line[3]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();
            int ifTrue = r.Matches(line[4]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();
            int ifFalse = r.Matches(line[5]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();

            monkeys[m] = new Monkey() { items = startingItems, opcode = opcode, operand = operand, test = test, ifTrue = ifTrue, ifFalse = ifFalse };
        }

        return monkeys;
    }

    public string SolvePart1(string input)
    {
        Monkey[] monkeys = ParseInput(input);

        for (int r = 0; r < 20; r++)
        {
            for (int m = 0; m < monkeys.Length; m++)
            {
                Monkey monkey = monkeys[m];
                int repeat = monkey.items.Count;
                for (int i = 0; i < repeat; i++)
                {
                    Int128 item = monkey.items[0];
                    monkey.inspectionCount++;

                    Int128 result = ( (monkey.opcode == 0 ? ((monkey.operand == null) ? item * item : item * monkey.operand!.Value) : item + monkey.operand!.Value) ) / 3;

                    int monkeyToPassTo;
                    if (result % (monkey.test) == 0)
                    {
                        monkeyToPassTo = monkey.ifTrue;
                    }
                    else
                    {
                        monkeyToPassTo = monkey.ifFalse;
                    }

                    //Console.WriteLine($"Item with worry level{result} is thrown to Monkeyu {monkeyToPassTo}");

                    monkey.items.RemoveAt(0);
                    monkeys[monkeyToPassTo].items.Add(result);
                }
            }
        }

        Monkey[] mostActive = monkeys.OrderByDescending(m => m.inspectionCount).ToArray();

        return $"{mostActive[0].inspectionCount * mostActive[1].inspectionCount}";
    }
   
    public string SolvePart2(string input)
    {
        Monkey[] monkeys = ParseInput(input);
        int LCM = monkeys.Select(m => m.test).Aggregate(Lcm);

        for (int r = 0; r < 10000; r++)
        {
            for (int m = 0; m < monkeys.Length; m++)
            {
                Monkey monkey = monkeys[m];

                int repeat = monkey.items.Count;
                for (int i = 0; i < repeat; i++)
                {
                    Int128 item = monkey.items[0];
                    monkey.inspectionCount++;

                    Int128 result = (monkey.opcode == 0 ? ((monkey.operand == null) ? item * item : item * monkey.operand!.Value) : item + monkey.operand!.Value ) % LCM;

                    int monkeyToPassTo;
                    if (result % monkey.test == 0)
                    {
                        monkeyToPassTo = monkey.ifTrue;
                    }
                    else
                    {
                        monkeyToPassTo = monkey.ifFalse;
                    }

                    monkey.items.RemoveAt(0);
                    monkeys[monkeyToPassTo].items.Add(result);
                }
            }
        }

        Monkey[] mostActive = monkeys.OrderByDescending(m => m.inspectionCount).ToArray();

        return $"{mostActive[0].inspectionCount * mostActive[1].inspectionCount}";
    }
}
