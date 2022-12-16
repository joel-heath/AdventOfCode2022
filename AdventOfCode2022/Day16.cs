using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        { "TestInput", "Output" }
    };

    class Valve
    {
        public readonly string Name;
        public readonly int FlowRate;
        public string[] Tunnels;
        public List<Valve> ConnectedVales = new();

        public Valve(string name, int flowrate,  string[] valveList)
        {
            Name = name;
            FlowRate = flowrate;
            Tunnels = valveList;
        }
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
            {
                connectedValves.Add(valves.Where(v => v.Name == t).First());
            }

            v.ConnectedVales = connectedValves;
        }

        return valves;
    }

    static bool CompareRoutes(List<Valve> a, List<Valve> b)
    {
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }

        return true;
    }

    static (List<Valve>?, int total, int time) AttemptRoute(Valve currentValve, List<List<Valve>> attemptedRoutes, List<Valve> currentRoute, int total, int time, List<int> openValves)
    {
        while (time > 0)
        {
            int max = 0;
            Valve chosenValve = null;
            foreach (Valve consideration in currentValve!.ConnectedVales)
            {
                bool alreadyAttemptedRoute = false;
                foreach (var route in attemptedRoutes)
                {
                    if (CompareRoutes(route, currentRoute.Concat(new List<Valve>() { consideration }).ToList()))
                    {
                        alreadyAttemptedRoute = true;
                        break;
                    }
                }
                if (alreadyAttemptedRoute) continue;

                if (consideration.FlowRate > max || chosenValve == null)
                {
                    max = consideration.FlowRate;
                    chosenValve = consideration;
                }
            }

            // all rotues already attempted
            if (chosenValve == null)
            {
                return (null, total, time);
            }


            currentValve = chosenValve!;
            time--; // minute travelling

            openValves.Add(currentValve.FlowRate);
            currentRoute!.Add(currentValve);
            total += openValves.Sum();
            time--; // minute opening valve
        }

        attemptedRoutes.Add(currentRoute);

        return (currentRoute!, total, time);
    }

    public string SolvePart1(string input)
    {
        var valves = ParseInput(input);
        Valve currentValve = valves.Where(v => v.Name == "AA").First();
        List<List<Valve>> attemptedRoutes = new();
        List<Valve> currentRoute = new();
        List<int> openValves = new();
        int time = 30;
        int total = 0;

        int max = 0;
        while (true)
        {
            try
            {
                (var route, total, time) = AttemptRoute(currentValve, attemptedRoutes, currentRoute, total, time, openValves);
                if (total > max) max = total;
                if (route == null)
                {
                    currentRoute.RemoveAt(currentRoute.Count - 1);
                    attemptedRoutes.Add(currentRoute);
                }
                else
                {
                    currentRoute = route;
                    attemptedRoutes.Add(route);
                }
                Console.WriteLine(max);
            }
            catch (ArgumentOutOfRangeException)
            {
                break;
            }
        }

        return $"{max}";
    }

    public string SolvePart2(string input)
    {
        return string.Empty;
    }

    [GeneratedRegex("Valve (\\w+) has flow rate=(\\d+); tunnels? leads? to valves?( \\w+)((?:, \\w+)*)")]
    private static partial Regex InputParse();
}
