using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.Design.Serialization;

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

    class Folder
    {
        public Folder? Parent { get; }
        //public Folder Parent => parent ?? this;
        public string Name { get; }
        public long Size()
        {
            long size = 0;
            
            foreach (File file in Files)
            {
                size += file.Size;
            }
            foreach (Folder child in Children)
            {
                size += child.Size();
            }
            

            size = Files.Select(f => f.Size).Sum() + Children.Select(c => c.Size()).Sum();

            return size;
        }
        
        public List<Folder> Children { get; }
        public List<File> Files { get; }

        public Folder AddOrGetChild(string folderName)
        {
            Folder[] kids = Children.Where(f => f.Name == folderName).ToArray();

            if (kids.Length == 0)
            {
                Folder newFolder = new Folder(this, folderName);
                Children.Add(newFolder);
                return newFolder;
            }
            else
            {
                return Children[Children.IndexOf(kids[0])];
                // same as return kids[0] ?? dunno
            }

        }
        public void AddFile(string fileName, long size)
        {
            Files.Add(new File(fileName, size));
        }

        public IEnumerable<Folder> GetChildren()
        {
            foreach (var child in Children)
            {
                yield return child;
                
                foreach (var c in child.GetChildren())
                {
                    yield return c;
                }
            }
        }

        public Folder(Folder? parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            Children = new List<Folder>();
            Files = new List<File>();
        }

        public override string ToString()
        {
            return $"{Name} {Size()}";
        }
    }

    class File
    {
        public string Name { get; }
        public long Size { get; }
        public File(string name, long size)
        {
            this.Name = name;
            this.Size = size;
        }
    }

    public string SolvePart1(string input)
    {
        string[][] commands = input.Split("$ ").Select(c => c.Split("\r\n")).ToArray();

        Folder current = new Folder(null, "");

        for (int i = 0; i < commands.Length; i++)
        {
            // 0th item is command, rest are computer response
            string[] command = commands[i][0].Split(" ");
            switch (command[0])
            {
                case "cd":
                    if (command[1] == "/")
                    {
                        while (current.Parent != null) current = current.Parent;
                    }
                    else if (command[1] == "..")
                    {
                        current = current.Parent ?? current;
                    }
                    else
                    {
                        current = current.AddOrGetChild(command[1]);
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
                            current.AddFile(file[1], long.Parse(file[0]));
                        }
                    }

                    break;
            }
        }

        /*
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
        */
        
        return $"not implemented";
    }

    public string SolvePart2(string input)
    {
        string[][] commands = input.Split("$ ").Select(c => c.Split("\r\n").Where(s => s != string.Empty).ToArray()).Where(s => s.Length != 0).ToArray();

        Folder current = new Folder(null, "");


        for (int i = 0; i < commands.Length; i++)
        {
            // 0th item is command, rest are computer response
            string[] command = commands[i][0].Split(" ");
            switch (command[0])
            {
                case "cd":
                    if (command[1] == "/")
                    {
                        while (current.Parent != null) current = current.Parent;
                    }
                    else if (command[1] == "..")
                    {
                        current = current.Parent ?? current;
                    }
                    else
                    {
                        current = current.AddOrGetChild(command[1]);
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
                            current.AddFile(file[1], long.Parse(file[0]));
                        }
                    }

                    break;
            }
        }
        // traverse back to root now
        while (current.Parent != null) current = current.Parent;

        var usedSpace = current.Size();
        var freeSpace = 70000000 - usedSpace;
        var spaceToClear = 30000000 - freeSpace;


        return $"{current.GetChildren().Select(f => f.Size()).Order().Where(f => f >= spaceToClear).First()}";
    }
}