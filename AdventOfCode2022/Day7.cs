using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal class Day7 : IDay
{
    public int Day => 7;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "$ cd /\r\n$ ls\r\ndir a\r\n14848514 b.txt\r\n8504156 c.dat\r\ndir d\r\n$ cd a\r\n$ ls\r\ndir e\r\n29116 f\r\n2557 g\r\n62596 h.lst\r\n$ cd e\r\n$ ls\r\n584 i\r\n$ cd ..\r\n$ cd ..\r\n$ cd d\r\n$ ls\r\n4060174 j\r\n8033020 d.log\r\n5626152 d.ext\r\n7214296 k", "95437" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "$ cd /\r\n$ ls\r\ndir a\r\n14848514 b.txt\r\n8504156 c.dat\r\ndir d\r\n$ cd a\r\n$ ls\r\ndir e\r\n29116 f\r\n2557 g\r\n62596 h.lst\r\n$ cd e\r\n$ ls\r\n584 i\r\n$ cd ..\r\n$ cd ..\r\n$ cd d\r\n$ ls\r\n4060174 j\r\n8033020 d.log\r\n5626152 d.ext\r\n7214296 k", "24933642" }
    };

    static void AddToDict(Dictionary<string, Dictionary<string, int>> dict, string path, string fileName, int fileSize)
    {
        if (dict.TryGetValue(path, out Dictionary<string, int>? files))
        {
            files[fileName] = fileSize;
            dict[path] = files;
        }
        else
        {
            dict[path] = new Dictionary<string, int> { { fileName, fileSize } };
        }
    }

    static string ListToString(List<string> list)
    => new string(list.SelectMany(s => s += "/").ToArray()).Trim('/');

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

    public string SolvePart1(string input)
    {
        string[][] commands = input.Split("$ ").Select(c => c.Split("\r\n")).ToArray();

        List<string> currentDirectory = new();


        //            path       files     filename  size
        Dictionary<string, Dictionary<string, int>> directories = new();

        for (int i = 0; i < commands.Length; i++)
        {
            // 0th item is command, rest are computer response
            string[] command = commands[i][0].Split(" ");
            switch (command[0])
            {
                case "cd":
                    if (command[1] == "/")
                    {
                        currentDirectory = new List<string>();
                    }
                    else if (command[1] == "..")
                    {
                        currentDirectory.RemoveAt(currentDirectory.Count - 1);
                    }
                    else
                    {
                        currentDirectory.Add(command[1]);
                    }
                    break;

                case "ls":
                    for (int j = 1; j < commands[i].Length; j++)
                    {
                        string line = commands[i][j];

                        MatchCollection matches = new Regex(@"(\d+) (.+)").Matches(line);

                        if (matches.Count == 0)
                        {
                            // dir _: not useful
                            continue;
                        }
                        else
                        {
                            
                            string[] file = matches[0].Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();

                            //Console.WriteLine($"Adding {file[1]} with a size of {file[0]}");

                            for (int k = 1; k <= currentDirectory.Count; k++)
                            {
                                AddToDict(directories, ListToString(currentDirectory.Take(k).ToList()), file[1], int.Parse(file[0]));
                            }
                        }
                    }

                    break;
            }
        }

        int total = 0;

        foreach (var dir in directories.Keys)
        {
            int sum = 0;
            foreach (var kvp in directories[dir])
            {
                sum += kvp.Value;
            }

            if (sum <= 100000) total += sum;
        }

        return $"{total}";
    }

    public string SolvePart2(string input)
    {
        string[][] commands = input.Split("$ ").Select(c => c.Split("\r\n").Where(s => s != string.Empty).ToArray()).Where(s => s.Length != 0).ToArray();

        List<string> currentDirectory = new();


        //            path       files     filename  size
        Dictionary<string, Dictionary<string, int>> directories = new();

        for (int i = 0; i < commands.Length; i++)
        {
            // 0th item is command, rest are computer response
            string[] command = commands[i][0].Split(" ");
            switch (command[0])
            {
                case "cd":
                    if (command[1] == "/")
                    {
                        currentDirectory = new List<string>();
                    }
                    else if (command[1] == "..")
                    {
                        currentDirectory.RemoveAt(currentDirectory.Count - 1);
                    }
                    else
                    {
                        currentDirectory.Add(command[1]);
                    }
                    break;

                case "ls":
                    for (int j = 1; j < commands[i].Length; j++)
                    {
                        string line = commands[i][j];

                        MatchCollection matches = new Regex(@"(\d+) (.+)").Matches(line);

                        if (matches.Count == 0)
                        {
                            // dir _: not useful
                            continue;
                        }
                        else
                        {

                            string[] file = matches[0].Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();


                            for (int k = 0; k <= currentDirectory.Count; k++)
                            {
                                AddToDict(directories, ListToString(currentDirectory.Take(k).ToList()), file[1], int.Parse(file[0]));
                            }
                        }
                    }

                    break;
            }
        }

        Dictionary<string, int> dirSizes = new();

        foreach (string dir in directories.Keys)
        {
            int sum = 0;
            foreach (var kvp in directories[dir])
            {
                sum += kvp.Value;
            }

            dirSizes[dir] = sum;
        }

        var sizes = dirSizes.Select(d => d.Value).Order().ToArray();
        int usedSpace = sizes[^1];

        int remainingSpace = 70000000 - usedSpace;
        int filesToRemove = 30000000 - remainingSpace;

        Console.WriteLine(filesToRemove);

        foreach (int size in sizes)
        {
            if (size >= filesToRemove)
            {
                return $"{size}";
            }
        }

        return "failed";
    }
}