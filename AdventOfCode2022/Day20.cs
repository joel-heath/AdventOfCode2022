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
using static AdventOfCode2022.Day12;

namespace AdventOfCode2022;
internal class Day20 : IDay
{
    public int Day => 20;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "1\r\n2\r\n-3\r\n3\r\n-2\r\n0\r\n4", "3" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "1\r\n2\r\n-3\r\n3\r\n-2\r\n0\r\n4", "1623178306" }
    };

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

    public Queue<LinkedListNode<long>> Mix(LinkedList<long> list, Queue<LinkedListNode<long>> dataStream)
    {
        Queue<LinkedListNode<long>> newDataStream = new();

        while (dataStream.TryDequeue(out LinkedListNode<long>? current))
        {
            LinkedListNode<long> previous = current;
            
            if (current.Value > 0)
            {
                for (int h = 0; h <= current.Value % (list.Count-1); h++)
                    previous = previous.Next ?? list.First!;

                newDataStream.Enqueue(list.AddBefore(previous, current.Value));
            }
            else
            {
                for (int h = 0; h <= Math.Abs(current.Value) % (list.Count-1); h++)
                    previous = previous.Previous ?? list.Last!;

                newDataStream.Enqueue(list.AddAfter(previous, current.Value));
            }

            list.Remove(current);
        }

        return newDataStream;
    }

    public string SolvePart1(string input)
    {
        LinkedList<long> list = new(input.Split("\r\n").Select(long.Parse));
        Mix(list, EnqueueNodes(list));

        List<long> longs = list.ToList();
        return $"{new int[] { 1000, 2000, 3000 }.Select(n => longs[(n + longs.IndexOf(0)) % longs.Count]).Sum()}";
    }

    public string SolvePart2(string input)
    {
        LinkedList<long> list = new(input.Split("\r\n").Select(n => 811589153 * long.Parse(n)));
        Queue<LinkedListNode<long>> dataStream = EnqueueNodes(list);
        for (int i = 0; i < 10; i++)
            dataStream = Mix(list, dataStream);

        List<long> longs = list.ToList();

        return $"{new int[] { 1000, 2000, 3000 }.Select(n => longs[(n + longs.IndexOf(0)) % longs.Count]).Sum()}";
    }
}