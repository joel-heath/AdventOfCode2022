using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;
internal partial class Day19 : IDay
{
    public int Day => 19;
    public Dictionary<string, string> UnitTestsP1 => new()
    {
        { "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.\r\nBlueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.", "33" }
    };
    public Dictionary<string, string> UnitTestsP2 => new()
    {
        { "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.\r\nBlueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.", "3472" }
    };

    static int[][] ParseInput(string input)
        => input.Split("\r\n").Select(l => InputParse().Match(l).Groups.Cast<Group>().Skip(1).Select(c => int.Parse(c.Value)).ToArray()).ToArray();

    public int PlayGame(int[] blueprint, (int c, int r) ore, (int c, int r) clay, (int c, int r) obsidian, (int c, int r) geode, int timeRemaining, Dictionary<(int,int,int,int,int,int,int,int,int), int> cache)
    {
        var cacheKey = (ore.c, ore.r, clay.c, clay.r, obsidian.c, obsidian.r, geode.c, geode.r, timeRemaining);
        if (cache.TryGetValue(cacheKey, out int score))
            return score;

        int max = 0;

        if (timeRemaining > 0)
        {
            ore.c += ore.r; clay.c += clay.r; obsidian.c += obsidian.r; geode.c += geode.r;

            // buy geode
            if (ore.c - ore.r >= blueprint[5] && obsidian.c - obsidian.r >= blueprint[6])
                max = Math.Max(PlayGame(blueprint, (ore.c - blueprint[5], ore.r), clay, (obsidian.c - blueprint[6], obsidian.r), (geode.c, geode.r + 1), timeRemaining - 1, cache), max);
            else
            {
                // buy obsidian
                if (ore.c - ore.r >= blueprint[3] && clay.c - clay.r >= blueprint[4])
                    max = Math.Max(PlayGame(blueprint, (ore.c - blueprint[3], ore.r), (clay.c - blueprint[4], clay.r), (obsidian.c, obsidian.r + 1), geode, timeRemaining - 1, cache), max);
                
                // dont bother buying clay if we have tons
                if (clay.c < 3 * blueprint[4])
                {
                    // buy clay
                    if (ore.c - ore.r >= blueprint[2])
                        max = Math.Max(PlayGame(blueprint, (ore.c - blueprint[2], ore.r), (clay.c, clay.r + 1), obsidian, geode, timeRemaining - 1, cache), max);
                }
                // dont bother buying ore if we have tons
                if (ore.c < 3 * blueprint[2])
                {
                    // buy ore
                    if (ore.c - ore.r >= blueprint[1])
                        max = Math.Max(PlayGame(blueprint, (ore.c - blueprint[1], ore.r + 1), clay, obsidian, geode, timeRemaining - 1, cache), max);
                }
                // wait
                max = Math.Max(PlayGame(blueprint, ore, clay, obsidian, geode, timeRemaining - 1, cache), max);
            }
        }

        max = Math.Max(geode.c, max);

        cache[cacheKey] = max;
        return max;
    }

    public string SolvePart1(string input)
    {
        int[][] blueprints = ParseInput(input);
        int total = 0;

        foreach (int[] blueprint in blueprints)
        {
            int score = PlayGame(blueprint, (0, 1), (0, 0), (0, 0), (0, 0), 24, new());
            int quality = score * blueprint[0];
            total += quality;
            //Console.WriteLine($"Game {blueprint[0]} finished, total geodes: {score}, quality level: {quality}");
            //Console.WriteLine($"Total so far: {total}");
        }

        return $"{total}";
    }

    public string SolvePart2(string input)
    {
        int[][] blueprints = ParseInput(input).Take(3).ToArray();
        int total = 1;

        foreach (int[] blueprint in blueprints)
        {
            int score = PlayGame(blueprint, (0, 1), (0, 0), (0, 0), (0, 0), 32, new());
            total *= score;
            Console.WriteLine($"Game {blueprint[0]} finished, total geodes: {score}");
            Console.WriteLine($"Total so far: {total}");
        }

        return $"{total}";
    }

    [GeneratedRegex("Blueprint (\\d+): Each ore robot costs (\\d+) ore. Each clay robot costs (\\d+) ore. Each obsidian robot costs (\\d+) ore and (\\d+) clay. Each geode robot costs (\\d+) ore and (\\d+) obsidian.")]
    private static partial Regex InputParse();
}
