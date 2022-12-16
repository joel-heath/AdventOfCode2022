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
        public readonly string Name;
        public readonly int FlowRate;
        public string[] Tunnels;
        public List<Valve> ConnectedValves = new();
        public bool Open;

        public Valve(string name, int flowrate,  string[] valveList)
        {
            Name = name;
            FlowRate = flowrate;
            Tunnels = valveList;
            Open = false;
        }
    }

    class Route
    {
        public List<Valve> ValvesOpened;
        public List<Valve> ValvesVisited;
        public int Score;
        public int TimeRemaining;

        public Route ToRoute()
        {
            Route route = new()
            {
                ValvesOpened = this.ValvesOpened.ToList(),
                ValvesVisited = this.ValvesVisited.ToList(),
                TimeRemaining = this.TimeRemaining,
                Score = this.Score
            };

            return route;
        }

        public static bool operator ==(Route a, Route b)
        {
            if (a.ValvesOpened.Count != b.ValvesOpened.Count) return false;

            for (int i = 0; i < a.ValvesOpened.Count; i++)
            {
                if (a.ValvesOpened[i] != b.ValvesOpened[i]) return false;
            }

            return true;
        }

        public static bool operator !=(Route a, Route b) => !(a == b);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (ValvesOpened.Count != ((Route)obj).ValvesOpened.Count)
                return false;

            for (int i = 0; i < ValvesOpened.Count; i++)
            {
                if (ValvesOpened[i] != ((Route)obj).ValvesOpened[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => HashCode.Combine(ValvesOpened);
    }

    static List<Valve> ParseInput(string input)
    {
        List<Valve> valves = new();

        Regex r = InputParse();
        
        string[] lines = input.Split("\r\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = r.Match(lines[i]).Groups.Cast<Group>().Skip(1).Select(c => c.Value).ToArray();

            valves.Add(new Valve(line[0], int.Parse(line[1]),
                new string[] { line[2] }.Concat(line[3].Split(", "))
                .Select(s => s.Trim(' ').Trim(',')).Where(s => s != string.Empty).ToArray()));
        }

        foreach (Valve v in valves)
        {
            List<Valve> connectedValves = new();

            foreach (string t in v.Tunnels)
                connectedValves.Add(valves.Where(v => v.Name == t).First());

            v.ConnectedValves = connectedValves;
        }

        return valves;
    }

    static int AttemptRoute(Route route, Valve valve)
    {
        int max = 0;

        if (route.TimeRemaining > 0)
        {
            if (!route.ValvesOpened.Contains(valve) && valve.FlowRate > 0)
            {
                Route newRoute = route.ToRoute();
                newRoute.TimeRemaining--;
                newRoute.ValvesVisited.Add(valve);
                newRoute.ValvesOpened.Add(valve);

                int score = valve.FlowRate * newRoute.TimeRemaining + AttemptRoute(newRoute, valve);
                if (score > max) max = score;
            }

            foreach (Valve v in valve.ConnectedValves)
            {
                if (route.ValvesVisited[^1] == v) continue;

                Route newRoute = route.ToRoute();
                newRoute.TimeRemaining--;
                newRoute.ValvesVisited.Add(valve);

                int score = AttemptRoute(newRoute, v);
                if (score > max) max = score;
            }
        }

        return max;
    }

    public string SolvePart1(string input)
    {
        var valves = ParseInput(input);
        Valve currentValve = valves.Where(v => v.Name == "AA").First();

        Route route = new();
        route.TimeRemaining = 30;
        route.ValvesOpened = new();
        route.ValvesVisited = new() { currentValve };

        return $"{AttemptRoute(route, currentValve)}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }

    [GeneratedRegex("Valve (\\w+) has flow rate=(\\d+); tunnels? leads? to valves?( \\w+)((?:, \\w+)*)")]
    private static partial Regex InputParse();
}
