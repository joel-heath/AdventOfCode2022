using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2022;
internal class Day25 : IDay
{
    public int Day => 25;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1=-0-2\r\n12111\r\n2=0=\r\n21\r\n2=01\r\n111\r\n20012\r\n112\r\n1=-1=\r\n1-12\r\n12\r\n1=\r\n122", "2=-1=0" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    static long BiggestPowerOfFive(long num)
    {
        int i = 0;
        for (; i <= num; i++)
        {
            long n = (long)Math.Pow(5, i);
            if (n > num) break;
        }

        return (i - 1);
    }

    static int BiggestMultipleOfFive(long num, long biggestPower)
    {
        long n = biggestPower;
        int i = -2;
        for (; i <= 2; i++)
        {
            n = biggestPower * i;
            if (n > num) break;
        }

        return (i - 1);
    }
    /*
    static string DecToSnafu(long dec)
    {
        long length = BiggestPowerOfFive(dec);
        long power = (long)Math.Pow(5, length);
        string snafu = string.Empty;

        for (int i = 0; i < length; i++)
        {
            int placeValue = BiggestMultipleOfFive(dec, i);
            long quantity = placeValue * power;
            dec -= quantity;

            snafu += placeValue switch
            {
                -2 => '=',
                -1 => '-',
                0 => '0',
                1 => '1',
                _ => '2'
            };
        }

        return snafu;
    }*/

    static string DecToSnafu(long dec)
    {
        if (dec == 0) return "";
        if (dec % 5 == 0) return DecToSnafu(dec / 5) + "0";
        if (dec % 5 == 1) return DecToSnafu(dec / 5) + "1";
        if (dec % 5 == 2) return DecToSnafu(dec / 5) + "2";
        if (dec % 5 == 3) return DecToSnafu((dec + 2) / 5) + "=";
        if (dec % 5 == 4) return DecToSnafu((dec + 1) / 5) + "-";
        throw new Exception("Thats a weird number");
    }

    public string SolvePart1(string input)
    {
        string[] lines = input.Split("\r\n");

        long total = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            long num = 0;
            
            for (int j = 0; j < line.Length; j++)
            {
                char word = line[j];

                int value = word switch
                {
                    '=' => -2,
                    '-' => -1,
                    '0' => 0,
                    '1' => 1,
                     _ => 2
                };

                num += value * (long)Math.Pow(5, line.Length-j-1);
            }

            //Console.WriteLine($"{line}   {num}");

            total += num;
        }

        Console.WriteLine($"Decimal: {total}"); // 32005641587247

        string snafu = DecToSnafu(total);
        Console.WriteLine($"SNAFU: {snafu}");

        return $"{snafu}";
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }
}
