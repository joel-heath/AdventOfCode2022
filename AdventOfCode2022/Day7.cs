using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.Design.Serialization;

namespace AdventOfCode2022;
internal partial class Day7 : IDay
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
        public string Name { get; }
        public List<File> Files { get; set; }
        public void AddFile(string fileName, long size)
        => Files.Add(new File(fileName, size));

        public List<Folder> Children { get; set; }
        public Folder AddChild(string folderName)
        {
            Folder newFolder = new(this, folderName);
            Children.Add(newFolder);
            return newFolder;
        }
        public IEnumerable<Folder> DeepChildren()
        {
            foreach (var child in Children)
            {
                yield return child;
                
                foreach (var c in child.DeepChildren())
                {
                    yield return c;
                }
            }
        }
        public long Size => Files.Select(f => f.Size).Sum() + Children.Select(c => c.Size).Sum();
        public Folder(Folder? parent, string name)
        {
            Parent = parent;
            Name = name;
            Children = new List<Folder>();
            Files = new List<File>();
        }
    }

    record File (string Name, long Size);

    static Folder ParseInput(string input)
    {
        IEnumerable<string[]> commands = input.Split("$ ").Select(c => c.Split("\r\n").Where(s => s != string.Empty).ToArray()).Where(s => s.Length != 0);
        Folder current = new(null, "");

        foreach (string[] command in commands)
        {
            string[] operation = command[0].Split(" ");
            if (operation[0][0] == 'c') current = operation[1] == ".." ? current.Parent ?? current : current.AddChild(operation[1]);
            else command.Select(l => FileSizeName().Matches(l)).Where(l => l.Count > 0)
                    .Select(m => m.First().Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray()).ToList()
                    .ForEach(f => current.AddFile(f[1], long.Parse(f[0])));
        }
        while (current.Parent != null) current = current.Parent;

        return current;
    }

    public string SolvePart1(string input)
    => $"{ParseInput(input).DeepChildren().Select(c => c.Size).Where(c => c <= 100000).Sum()}";

    public string SolvePart2(string input)
    {
        Folder root = ParseInput(input);
        long size = root.Size + 30000000 - 70000000;

        return $"{root.DeepChildren().Select(f => f.Size).Order().Where(f => f >= size).First()}";
    }

    [GeneratedRegex("(\\d+) (.+)")]
    private static partial Regex FileSizeName();
}