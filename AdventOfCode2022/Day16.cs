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
        { "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", "1651" },
        { "Valve TM has flow rate=3; tunnels lead to valves WB, PE, DX, TK, CH\r\nValve ST has flow rate=21; tunnels lead to valves NS, DE, UX, XU\r\nValve IX has flow rate=0; tunnels lead to valves DK, LR\r\nValve OG has flow rate=0; tunnels lead to valves MN, FK\r\nValve FR has flow rate=0; tunnels lead to valves JQ, GS\r\nValve HU has flow rate=0; tunnels lead to valves TJ, XX\r\nValve WC has flow rate=15; tunnel leads to valve TJ\r\nValve JT has flow rate=0; tunnels lead to valves OV, AA\r\nValve DW has flow rate=0; tunnels lead to valves FK, AA\r\nValve RG has flow rate=0; tunnels lead to valves PS, DK\r\nValve JQ has flow rate=14; tunnels lead to valves VM, FR\r\nValve XX has flow rate=5; tunnels lead to valves GP, MN, WB, LM, HU\r\nValve IN has flow rate=11; tunnels lead to valves OK, GS, DU\r\nValve LR has flow rate=7; tunnels lead to valves IX, NR, YY, HZ, PR\r\nValve TK has flow rate=0; tunnels lead to valves TM, OV\r\nValve VM has flow rate=0; tunnels lead to valves KQ, JQ\r\nValve IC has flow rate=0; tunnels lead to valves FK, DU\r\nValve CH has flow rate=0; tunnels lead to valves EZ, TM\r\nValve OV has flow rate=10; tunnels lead to valves YW, JT, NN, TK\r\nValve KQ has flow rate=17; tunnels lead to valves VM, YW, CY\r\nValve NR has flow rate=0; tunnels lead to valves FK, LR\r\nValve MN has flow rate=0; tunnels lead to valves OG, XX\r\nValve YY has flow rate=0; tunnels lead to valves LR, LM\r\nValve OK has flow rate=0; tunnels lead to valves CY, IN\r\nValve DK has flow rate=20; tunnels lead to valves FA, RG, IX\r\nValve CY has flow rate=0; tunnels lead to valves KQ, OK\r\nValve PR has flow rate=0; tunnels lead to valves DX, LR\r\nValve DE has flow rate=0; tunnels lead to valves ST, EL\r\nValve TJ has flow rate=0; tunnels lead to valves WC, HU\r\nValve NS has flow rate=0; tunnels lead to valves WU, ST\r\nValve PE has flow rate=0; tunnels lead to valves TM, XO\r\nValve DU has flow rate=0; tunnels lead to valves IN, IC\r\nValve DX has flow rate=0; tunnels lead to valves TM, PR\r\nValve EQ has flow rate=0; tunnels lead to valves AA, GP\r\nValve AA has flow rate=0; tunnels lead to valves JT, EZ, HZ, DW, EQ\r\nValve WB has flow rate=0; tunnels lead to valves TM, XX\r\nValve PF has flow rate=23; tunnels lead to valves BP, WU\r\nValve FJ has flow rate=19; tunnels lead to valves DO, TY, NN, PS\r\nValve GP has flow rate=0; tunnels lead to valves XX, EQ\r\nValve FK has flow rate=4; tunnels lead to valves DW, XO, OG, IC, NR\r\nValve DO has flow rate=0; tunnels lead to valves XU, FJ\r\nValve XO has flow rate=0; tunnels lead to valves FK, PE\r\nValve PS has flow rate=0; tunnels lead to valves RG, FJ\r\nValve MD has flow rate=25; tunnel leads to valve BP\r\nValve EZ has flow rate=0; tunnels lead to valves CH, AA\r\nValve GS has flow rate=0; tunnels lead to valves IN, FR\r\nValve XU has flow rate=0; tunnels lead to valves DO, ST\r\nValve WU has flow rate=0; tunnels lead to valves PF, NS\r\nValve YW has flow rate=0; tunnels lead to valves OV, KQ\r\nValve HZ has flow rate=0; tunnels lead to valves LR, AA\r\nValve TY has flow rate=0; tunnels lead to valves FJ, EL\r\nValve BP has flow rate=0; tunnels lead to valves MD, PF\r\nValve EL has flow rate=18; tunnels lead to valves DE, TY\r\nValve UX has flow rate=0; tunnels lead to valves FA, ST\r\nValve FA has flow rate=0; tunnels lead to valves UX, DK\r\nValve NN has flow rate=0; tunnels lead to valves OV, FJ\r\nValve LM has flow rate=0; tunnels lead to valves XX, YY", "2119" }

    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", "1707" },
        //{ "Valve TM has flow rate=3; tunnels lead to valves WB, PE, DX, TK, CH\r\nValve ST has flow rate=21; tunnels lead to valves NS, DE, UX, XU\r\nValve IX has flow rate=0; tunnels lead to valves DK, LR\r\nValve OG has flow rate=0; tunnels lead to valves MN, FK\r\nValve FR has flow rate=0; tunnels lead to valves JQ, GS\r\nValve HU has flow rate=0; tunnels lead to valves TJ, XX\r\nValve WC has flow rate=15; tunnel leads to valve TJ\r\nValve JT has flow rate=0; tunnels lead to valves OV, AA\r\nValve DW has flow rate=0; tunnels lead to valves FK, AA\r\nValve RG has flow rate=0; tunnels lead to valves PS, DK\r\nValve JQ has flow rate=14; tunnels lead to valves VM, FR\r\nValve XX has flow rate=5; tunnels lead to valves GP, MN, WB, LM, HU\r\nValve IN has flow rate=11; tunnels lead to valves OK, GS, DU\r\nValve LR has flow rate=7; tunnels lead to valves IX, NR, YY, HZ, PR\r\nValve TK has flow rate=0; tunnels lead to valves TM, OV\r\nValve VM has flow rate=0; tunnels lead to valves KQ, JQ\r\nValve IC has flow rate=0; tunnels lead to valves FK, DU\r\nValve CH has flow rate=0; tunnels lead to valves EZ, TM\r\nValve OV has flow rate=10; tunnels lead to valves YW, JT, NN, TK\r\nValve KQ has flow rate=17; tunnels lead to valves VM, YW, CY\r\nValve NR has flow rate=0; tunnels lead to valves FK, LR\r\nValve MN has flow rate=0; tunnels lead to valves OG, XX\r\nValve YY has flow rate=0; tunnels lead to valves LR, LM\r\nValve OK has flow rate=0; tunnels lead to valves CY, IN\r\nValve DK has flow rate=20; tunnels lead to valves FA, RG, IX\r\nValve CY has flow rate=0; tunnels lead to valves KQ, OK\r\nValve PR has flow rate=0; tunnels lead to valves DX, LR\r\nValve DE has flow rate=0; tunnels lead to valves ST, EL\r\nValve TJ has flow rate=0; tunnels lead to valves WC, HU\r\nValve NS has flow rate=0; tunnels lead to valves WU, ST\r\nValve PE has flow rate=0; tunnels lead to valves TM, XO\r\nValve DU has flow rate=0; tunnels lead to valves IN, IC\r\nValve DX has flow rate=0; tunnels lead to valves TM, PR\r\nValve EQ has flow rate=0; tunnels lead to valves AA, GP\r\nValve AA has flow rate=0; tunnels lead to valves JT, EZ, HZ, DW, EQ\r\nValve WB has flow rate=0; tunnels lead to valves TM, XX\r\nValve PF has flow rate=23; tunnels lead to valves BP, WU\r\nValve FJ has flow rate=19; tunnels lead to valves DO, TY, NN, PS\r\nValve GP has flow rate=0; tunnels lead to valves XX, EQ\r\nValve FK has flow rate=4; tunnels lead to valves DW, XO, OG, IC, NR\r\nValve DO has flow rate=0; tunnels lead to valves XU, FJ\r\nValve XO has flow rate=0; tunnels lead to valves FK, PE\r\nValve PS has flow rate=0; tunnels lead to valves RG, FJ\r\nValve MD has flow rate=25; tunnel leads to valve BP\r\nValve EZ has flow rate=0; tunnels lead to valves CH, AA\r\nValve GS has flow rate=0; tunnels lead to valves IN, FR\r\nValve XU has flow rate=0; tunnels lead to valves DO, ST\r\nValve WU has flow rate=0; tunnels lead to valves PF, NS\r\nValve YW has flow rate=0; tunnels lead to valves OV, KQ\r\nValve HZ has flow rate=0; tunnels lead to valves LR, AA\r\nValve TY has flow rate=0; tunnels lead to valves FJ, EL\r\nValve BP has flow rate=0; tunnels lead to valves MD, PF\r\nValve EL has flow rate=18; tunnels lead to valves DE, TY\r\nValve UX has flow rate=0; tunnels lead to valves FA, ST\r\nValve FA has flow rate=0; tunnels lead to valves UX, DK\r\nValve NN has flow rate=0; tunnels lead to valves OV, FJ\r\nValve LM has flow rate=0; tunnels lead to valves XX, YY", "8" }
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

    static int bestSoFar = 0;
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
            max = Math.Max(flow + CalculateScore(valves, new(opened) { name }, (name, remainingTime), ("XXX",0), cache), max);
        }

        if (max > bestSoFar)
        {
            bestSoFar = max;
            Console.WriteLine(max);
        }
        cache[cacheKey] = max;
        return max;
    }


    public string SolvePart2(string input)
        => $"{CalculateScore(ParseInput(input), new() { "AA" }, ("AA", 26), ("AA", 26), new())}";

    [GeneratedRegex("Valve (\\w+) has flow rate=(\\d+); tunnels? leads? to valves?( \\w+)((?:, \\w+)*)")]
    private static partial Regex InputParse();
}
