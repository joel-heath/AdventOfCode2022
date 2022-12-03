using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace AdventOfCode2022;
internal class Day2 : IDay
{
    public int Day => 2;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "A Y\r\nB X\r\nC Z", "6" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "A Y\r\nB X\r\nC Z", "12" },
    };

    public string SolvePart1(string input)
    {
        /*// each line       foreach   split into words  foreach      turn A,B,C & X,Y,Z into 0, 1, 2
        input.Split("\r\n").Select(l => l.Split(" ").Select(w => w[0] - 65 > 3 ? w[0] - 65 : w[0] - 88))*/

        string[] lines = input.Split("\r\n");
        int total = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ");
            int pivot = words[0].Trim(' ') switch
            {
                "A" => 1,
                "B" => 0,
                 _  => 2   // "C"
            };

            total += ((words[1][0] - 88 + pivot) % 3 * 3) + words[1][0] - 88;
        }

        return $"{total}";
    }

    public string SolvePart2(string input)
    {
        string[] lines = input.Split("\r\n");
        int total = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(" ");


            switch (words[0].Trim(' '))
            {
                case "A": //rock
                    {
                        switch (words[1])
                        {
                            case "X": // loose : sci
                                total += 3;
                                total += 0;
                                break;
                            case "Y": // draw : rock
                                total += 1;
                                total += 3;
                                break;
                            case "Z": // win : paper
                                total += 2;
                                total += 6;
                                break;
                        }

                    }
                    break;

                case "B": // paper
                    switch (words[1].Trim(' '))
                    {
                        case "X": // loose : rock
                            total += 1;
                            total += 0;
                            break;
                        case "Y": // draw
                            total += 2;
                            total += 3;
                            break;
                        case "Z": // win: sci
                            total += 3;
                            total += 6;
                            break;
                    }
                    break;

                case "C": // sci
                    switch (words[1].Trim(' '))
                    {
                        case "X": //loose: paper
                            total += 2;
                            total += 0;
                            break;
                        case "Y": // draw: sci
                            total += 3;
                            total += 3;
                            break;
                        case "Z":// win: rock 
                            total += 1;
                            total += 6;
                            break;
                    }
                    break;
            }
        }



        return $"{total}";
    }
}
