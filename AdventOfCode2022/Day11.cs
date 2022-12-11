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
using System.ComponentModel.DataAnnotations;

namespace AdventOfCode2022;
internal partial class Day11 : IDay
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

    public string SolvePart1(string input)
    {
        Monkey[] monkeys = ParseInput(input);

        return $"{Enumerable.Range(1, 20).Aggregate(monkeys, (acc, iteration) => DoRound(acc, n => n / 3))
            .Select(m => m.InspectionCount).OrderDescending().Take(2).Aggregate((a, b) => a * b)}";
    }

    public string SolvePart2(string input)
    {
        Monkey[] monkeys = ParseInput(input);
        int LCM = Lcm(monkeys);

        return $"{Enumerable.Range(1, 10000).Aggregate(monkeys, (acc, iteration) => DoRound(acc, n => n % LCM))
            .Select(m => m.InspectionCount).OrderDescending().Take(2).Aggregate((a, b) => a * b)}";
    }

    enum Operator { Times, Add, Square }
    class Monkey
    {
        public Queue<Int128> Items;
        public long InspectionCount;
        public readonly Operator Operator;
        public readonly int? Operand;
        public readonly int Test;
        public readonly int IfTrue;
        public readonly int IfFalse;

        public Monkey(IEnumerable<Int128> startingItems, int opcode, int? operand, int test, int ifTrue, int ifFalse)
        {
            this.Items = new(startingItems);
            this.Operator = (Operator)opcode;
            this.Operand = operand;
            this.Test = test;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
            this.InspectionCount = 0;
        }
    }

    static Monkey[] DoRound(Monkey[] monkeys, Func<Int128, Int128> worryReducer)
    {
        foreach (Monkey monkey in monkeys)
        {
            int repeat = monkey.Items.Count;
            for (int i = 0; i < repeat; i++, monkey.InspectionCount++)
            {
                Int128 item = monkey.Items.Dequeue();
                Int128 worry = monkey.Operator == Operator.Times ? item * monkey.Operand!.Value : monkey.Operator == Operator.Add ? item + monkey.Operand!.Value : item * item;
                worry = worryReducer(worry);
                monkeys[worry % monkey.Test == 0 ? monkey.IfTrue : monkey.IfFalse].Items.Enqueue(worry);
            }
        }

        return monkeys;
    }

    // Euclid's algorithm
    static int Gcd(int a, int b)
    {
        while (b != 0) { (a, b) = (b, a % b); }
        return a;
    }
    static int Lcm(Monkey[] m) => m.Select(m => m.Test).Aggregate((a, b) => a * b / Gcd(a, b));

    static Monkey[] ParseInput(string input)
    {
        string[] rawMonkeys = input.Split("\r\n\r\n");
        Monkey[] monkeys = new Monkey[rawMonkeys.Length];
        Regex r = FindDigits();

        for (int m = 0; m < rawMonkeys.Length; m++)
        {
            string[] line = rawMonkeys[m].Split("\r\n").Select(s => s.Trim(' ')).ToArray();
            
            IEnumerable<Int128> startingItems = r.Matches(line[1]).Select(m => Int128.Parse(m.Groups.Cast<Group>().First().Value));
            int opcode = line[2].Contains('*') ? 0 : 1;
            int? operand = null;
            try { operand = r.Matches(line[2]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First(); }
            catch (InvalidOperationException) { opcode = 2; }
            int test = r.Matches(line[3]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();
            int ifTrue = r.Matches(line[4]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();
            int ifFalse = r.Matches(line[5]).Select(m => int.Parse(m.Groups.Cast<Group>().First().Value)).First();

            monkeys[m] = new Monkey(startingItems, opcode, operand, test, ifTrue, ifFalse);
        }

        return monkeys;
    }

    [GeneratedRegex("(\\d+)")]
    private static partial Regex FindDigits();
}
