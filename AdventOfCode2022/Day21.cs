using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2022;
internal class Day21 : IDay
{
    public int Day => 21;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "root: pppw + sjmn\r\ndbpl: 5\r\ncczh: sllz + lgvd\r\nzczc: 2\r\nptdq: humn - dvpt\r\ndvpt: 3\r\nlfqf: 4\r\nhumn: 5\r\nljgn: 2\r\nsjmn: drzm * dbpl\r\nsllz: 4\r\npppw: cczh / lfqf\r\nlgvd: ljgn * ptdq\r\ndrzm: hmdt - zczc\r\nhmdt: 32", "152" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "root: pppw + sjmn\r\ndbpl: 5\r\ncczh: sllz + lgvd\r\nzczc: 2\r\nptdq: humn - dvpt\r\ndvpt: 3\r\nlfqf: 4\r\nhumn: 5\r\nljgn: 2\r\nsjmn: drzm * dbpl\r\nsllz: 4\r\npppw: cczh / lfqf\r\nlgvd: ljgn * ptdq\r\ndrzm: hmdt - zczc\r\nhmdt: 32", "301" }
    };

    class Monkey
    {
        public string Name { get; }
        public long? Value { get; set; }
        public string Operand1 { get; set; }
        public string Operand2 { get; set; }
        public char Opcode { get; set; }

        public Monkey(string name, long value)
        {
            this.Name = name;
            this.Value = value;
            this.Operand1 = string.Empty;
            this.Operand2 = string.Empty;
            this.Opcode = (char)0;
        }
        public Monkey(string name, string operand1, string operand2, string opcode)
        {
            this.Name = name;
            this.Value = null;
            this.Operand1 = operand1;
            this.Operand2 = operand2;
            this.Opcode = opcode[0];
        }
    }
    static Dictionary<string, Monkey> ParseInput(string input)
    {
        string[] lines = input.Split("\r\n");
        Dictionary<string, Monkey> monkeys = new();

        for (long i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(' ').Select(x => x.Trim()).ToArray();
            string name = line[0][..4];
            string operand1 = line[1];
            
            if (line.Length == 2) monkeys[name] = new Monkey(name, long.Parse(operand1));
            else
            {
                string opcode = line[2];
                string operand2 = line[3];
                monkeys[name] = new Monkey(name, operand1, operand2, opcode);
            }
        }

        return monkeys;
    }

    static long WhatWillHeYell(string monkeyName, Dictionary<string, Monkey> monkeys)
    {
        Monkey m = monkeys[monkeyName];
        if (m.Value != null) return m.Value.Value;

        long val1 = WhatWillHeYell(m.Operand1, monkeys);
        long val2 = WhatWillHeYell(m.Operand2, monkeys);

        return m.Opcode switch
        {
            '+' => val1 + val2,
            '-' => val1 - val2,
            '*' => val1 * val2,
            '/' => val1 / val2,
            _ => -1
        };
    }

    static bool FindHumn(string monkeyName, Dictionary<string, Monkey> monkeys)
    {
        if (monkeyName == "humn") return true;
        Monkey m = monkeys[monkeyName];
        if (m.Value != null) return false;
        return FindHumn(m.Operand1, monkeys) || FindHumn(m.Operand2, monkeys);
    }

    static long WhatWillYouYell(string monkeyName, Dictionary<string, Monkey> monkeys, long above)
    {
        Monkey m = monkeys[monkeyName];

        if (monkeyName == "root")
            return FindHumn(m.Operand1, monkeys) ? WhatWillYouYell(m.Operand1, monkeys, WhatWillHeYell(m.Operand2, monkeys)) : WhatWillYouYell(m.Operand2, monkeys, WhatWillHeYell(m.Operand1, monkeys));

        if (FindHumn(m.Operand1, monkeys))
        {
            long value = WhatWillHeYell(m.Operand2, monkeys);
            long answer = m.Opcode switch
            {
                '+' => above - value,
                '-' => above + value,
                '*' => above / value,
                '/' => above * value,
                _ => -1
            };
            return m.Operand1 == "humn" ? answer : WhatWillYouYell(m.Operand1, monkeys, answer);
        }
        else
        {
            long value = WhatWillHeYell(m.Operand1, monkeys);
            long answer = m.Opcode switch
            {
                '+' => above - value,
                '-' => value - above,
                '*' => above / value,
                '/' => value / above,
                _ => -1
            };
            return m.Operand2 == "humn" ? answer : WhatWillYouYell(m.Operand2, monkeys, answer);
        }
    }

    public string SolvePart1(string input) => $"{WhatWillHeYell("root", ParseInput(input))}";
    public string SolvePart2(string input) => $"{WhatWillYouYell("root", ParseInput(input), 0)}";
}