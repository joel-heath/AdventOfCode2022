using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Immutable;

namespace AdventOfCode2022;
internal class Day20 : IDay
{
    public int Day => 20;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1\r\n2\r\n-3\r\n3\r\n-2\r\n0\r\n4", "3" },
        { "0\r\n0\r\n0\r\n1\r\n0\r\n0\r\n0", "1" },
        { "0\r\n-2\r\n0\r\n0\r\n0\r\n0\r\n0", "-2" },
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "TestInput", "Output" }
    };

    /*
    static LinkedListNode<T> FindAtIndex<T>(LinkedList<T> list, int index)
    {
        LinkedListNode<T> previous = list.First!;
        for (int h = 0; h < index; h++)
            previous = previous.Next ?? list.First!;

        return previous;
    }*/

    static Queue<LinkedListNode<T>> EnqueueNodes<T>(LinkedList<T> list)
    {
        Queue<LinkedListNode<T>> copy = new();
        LinkedListNode<T> current = list.First!;

        copy.Enqueue(current);
        while (current.Next != null)
        {
            current = current.Next;
            copy.Enqueue(current);
        }

        return copy;
    }

    public string SolvePart1(string input)
    {
        LinkedList<int> list = new(input.Split("\r\n").Select(int.Parse));
        Queue<LinkedListNode<int>> dataStream = EnqueueNodes(list);
        LinkedListNode<int>? zero = null;

        while (dataStream.TryDequeue(out LinkedListNode<int>? current))
        {
            int digitToMove = current.Value;

            if (digitToMove == 0)
            {
                zero = current;
                continue;
            }

            LinkedListNode<int> previous = current;
            if (digitToMove > 0)
            {
                for (int h = 0; h <= digitToMove; h++)
                    previous = previous.Next ?? list.First!;

                list.AddBefore(previous, digitToMove);
            }
            else
            {
                for (int h = 0; h >= digitToMove; h--)
                    previous = previous.Previous ?? list.Last!;

                list.AddAfter(previous, digitToMove);
            }

            list.Remove(current);

            Console.WriteLine(string.Join(", ", list));
        }

        /*
        int indexOfZero = list.Select((v, i) => (v, i)).First(n => n.v == 0).i;
        Console.WriteLine(indexOfZero);

        int x = FindAtIndex(list, (indexOfZero + 1000) % list.Count).Value;
        int y = FindAtIndex(list, (indexOfZero + 2000) % list.Count).Value;
        int z = FindAtIndex(list, (indexOfZero + 3000) % list.Count).Value;
        */

        LinkedListNode<int> node = zero!;

        for (int i = 0; i < 1000; i++)
            node = node.Next ?? list.First!;
        int x = node.Value;
        for (int i = 0; i < 1000; i++)
            node = node.Next ?? list.First!;
        int y = node.Value;
        for (int i = 0; i < 1000; i++)
            node = node.Next ?? list.First!;
        int z = node.Value;

        Console.WriteLine($"({x}, {y}, {z})");

        Console.ReadKey();
        return $"{x+y+z}";

        // -15631 incorrec
        // -9516 incorrect
        // 7603 too low
    }

    public string SolvePart2(string input)
    {
        return $"{string.Empty}";
    }
}