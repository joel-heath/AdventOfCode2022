using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml.Linq;
using System.ComponentModel;
using System.Xml.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Data;

namespace AdventOfCode2022;
internal partial class Day16 : IDay
{
    public int Day => 16;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", "1651" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", "1707" }
    };

    class Valve
    {
        public readonly int FlowRate;
        public string[] AdjacentValves;
        public Dictionary<Valve, int> Tunnels = new();
        public readonly string Name;

        public Valve(string name, int flowrate, string[] adjactents)
        {
            Name = name;
            FlowRate = flowrate;
            AdjacentValves = adjactents;
        }
    }

    static Dictionary<string, Valve> ParseInput(string input)
    {
        Dictionary<string, Valve> valves = new();
        List<(string name, string[] tunnels)> nodes = new();

        Regex r = InputParse();
        
        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = r.Match(lines[i]).Groups.Cast<Group>().Skip(1).Select(c => c.Value).ToArray();

            string[] tunnels = new string[] { line[2] }
                .Concat(line[3].Split(", "))
                .Select(s => s.Trim(' ').Trim(',')).Where(s => s != string.Empty).ToArray();

            nodes.Add((line[0], tunnels));

            valves.Add(line[0], new Valve(line[0], int.Parse(line[1]), tunnels));
        }

        foreach (var v in valves)
        {
            v.Value.Tunnels = Dijkstras(v.Value, valves);
        }

        IEnumerable<string> uselessValves = valves.Where(v => v.Value.FlowRate == 0 && v.Key != "AA").Select(kvp => kvp.Key);
        foreach (string valveName in uselessValves)
        {
            Valve valve = valves[valveName];
            valves.Remove(valveName);
            foreach (var v in valves)
            {
                v.Value.Tunnels.Remove(valve);
            }
        }

        return valves;
    }

    static Dictionary<Valve, int> Dijkstras(Valve start, Dictionary<string, Valve> valves)
    {
        Dictionary<Valve, int> closed = new();
        Dictionary<Valve, int> open = new() { { start, 0 } };

        while (open.Count > 0)
        {
            var kvp = open.MinBy(s => s.Value);
            Valve chosen = kvp.Key;
            int distance = kvp.Value;

            open.Remove(chosen);
            closed.Add(chosen, distance);

            foreach (string valveName in chosen.AdjacentValves)
            {
                Valve valve = valves[valveName];

                if (!closed.ContainsKey(valve))
                {
                    if (open.TryGetValue(valve, out int oldDistance))
                    {
                        if (distance + 1 < oldDistance)
                            open[valve] = distance + 1;
                    }
                    else
                    {
                        open.Add(valve, distance + 1);
                    }
                }
            }
        }

        closed.Remove(start);

        return closed;
    }

    public string SolvePart1(string input)
        => $"{CalculateScore(ParseInput(input), new() { "AA" }, ("AA", 30), ("XXX", 0), new())}";

    static int CalculateScore(Dictionary<string, Valve> valves, HashSet<string> opened, (string pos, int time) p1, (string pos, int time) p2, Dictionary<string, int> cache)
    {
        var cacheKey = $"{string.Join(',', opened.Order())},{p1},{p2}";
        if (cache.TryGetValue(cacheKey, out int score))
            return score;

        (var player, var other) = p1.time >= p2.time ? (p1, p2) : (p2, p1);

        int max = 0;
        Valve current = valves[player.pos];

        foreach (var kvp in current.Tunnels.Where(t => !opened.Contains(t.Key.Name) && player.time > t.Value + 1))
        {
            Valve valve = kvp.Key;
            int distance = kvp.Value;
            string name = valve.Name;
            int remainingTime = player.time - distance - 1;

            int flow = valve.FlowRate * remainingTime;

            max = Math.Max(flow + CalculateScore(valves, new(opened) { name }, (name, remainingTime), other, cache), max);
        }
        if (other.pos != "XXX") max = Math.Max(CalculateScore(valves, opened, ("XXX", 0), other, cache), max);

        cache[cacheKey] = max;
        return max;
    }


    public string SolvePart2(string input)
        => $"{CalculateScore(ParseInput(input), new() { "AA" }, ("AA", 26), ("AA", 26), new())}";

    [GeneratedRegex("Valve (\\w+) has flow rate=(\\d+); tunnels? leads? to valves?( \\w+)((?:, \\w+)*)")]
    private static partial Regex InputParse();
}
