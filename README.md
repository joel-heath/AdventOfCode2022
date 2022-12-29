# Advent of Code 2022
My C# solutions to [Advent of Code 22](https://adventofcode.com/2022). Trying to one-liner as many as possible but I've already failed at Day 5.

## Notes

### Legend
🟢 Successful one-liner.<br>
🟡 Could be reduced to one-liner but would require repeated evaluations of the same code.<br>
🟠 Far from a one-liner, with minimal amounts of code reduced.<br>
🔴 Barely a visible effort to reduce the code.

| **Day** | **Verbosity** | **Notes** |
|:---:|:---:|:---:|
| [1](AdventOfCode2022/Day1.cs) | 🟢 | [part 1 it was relatively trivial.] |
| [2](AdventOfCode2022/Day2.cs) | 🟢 | This one was hard due to trying to find a way to convert ASCII codes into the correct output, and I ended feeding the part 2 input into my part 1 solution. |
| [3](AdventOfCode2022/Day3.cs) | 🟢 | This one had the challenge of flipping the alphabet relative to ASCII codes and uses pretty arbitrary seeming codes, but it works. |
| [4](AdventOfCode2022/Day4.cs) | 🟢 | This one posed the interesting problem of reducing about 30 different comparisons as much as possible, and although sorting the list *technically adds more comparisons*, the visible comparisons are less. The `ThenBy()` extension method came in handy for equal bounds. |
| [5](AdventOfCode2022/Day5.cs) | 🟠 | Miles from a one-liner, the input parsing makes it ridiculously long from the get-go, but it could be made more shorter using hard-coded values, as my code doesn't assume there are 9 stacks. This is also the kind of one where reducing it to a one liner would mean copious amounts of repeated code and repeated evaluations. |
| [6](AdventOfCode2022/Day6.cs) | 🟢 | Day 6 was very much an apology for the stress caused by day 5, my solution being one-liner with less characters than day 1. |
| [7](AdventOfCode2022/Day7.cs) | 🟠 | Started of tragically, I went for the non-object-oriented approach and could not solve part 2. After an entire re-attempt I did solve it, and now it's at the point where the code is relatively minimal, alongside using two classes which haven't really been reduced. |
| [8](AdventOfCode2022/Day8.cs) | 🟡 | This one was quite easy to reduce down to a few seperate functions, with the grid being stored in a variable so the input doesn't have to be parsed multiple times. |
| [9](AdventOfCode2022/Day9.cs) | 🟠 | This was a good day. Making the rope move frame by frame was very satisfying. 10/10 would solve again, however, my code is realtively short but certainly could do with some simplifying, and it is all very imperitive approached. |
| [10](AdventOfCode2022/Day10.cs) | 🟡 | This is another one of a single massive return line, plus some variables stored, and a couple functions to mitigate a horrific looking return line. Part 2 involved passing in the cycle count to a function unneccesarily, just so I could increment it as I passed it in. |
| [11](AdventOfCode2022/Day11.cs) | 🔴 | Another day 5, I'm not even going to pretend this is anywhere close to a one-liner. It did provide a good use of a queue for the monkey's items, and that part 2 was not fun |
| [12](AdventOfCode2022/Day12.cs) | 🟠 | A lot of class definitions and things so inately cannot be a one-liner, but a couple things a short. Spent ages making A* only to realise part 2 couldn't use it. Sad. |
| [13](AdventOfCode2022/Day13.cs) | 🔴 | Realistically, this one shouldn't be too hard to recude, I just haven't. At all. It also uses bubble sort so just don't look at it. |
| [14](AdventOfCode2022/Day14.cs) | 🔴 | This is one where I just have no idea how I would reduce it. But, the sand physics were fun to visualise, I also made a [falling snow screen-saver app](https://github.com/joelheath24/ChristmasConsole) based on the rules for this one! |
| [15](AdventOfCode2022/Day15.cs) | 🔴 | Ah this one's part 2 had me stumped for quite some time. (From this point on there are no one-liners) |
| [16](AdventOfCode2022/Day16.cs) | 🔴 | I finished this two days late, my final strategy was Dijkstra's from each valve, and heavily relied on caching states. Lightyears from a one-liner. |
| [17](AdventOfCode2022/Day17.cs) | 🔴 | For such a simple looking input it's such a complicated program. I have nothing more to say than part 2 was painful. |
| [18](AdventOfCode2022/Day18.cs) | 🟡 | Part one is entirely one-liner functions, part 2's flood fill not so much but i think I've done a pretty good job at making it as short as possible. Happy with this one. |
| [19](AdventOfCode2022/Day19.cs) | 🟠 | Giving this orange because the input parsing is one-liner (it's basically just one regex) and its not that long! Still baffled by the pythoneers solving it in 8 lines that runs in 20 seconds. |
| [20](AdventOfCode2022/Day20.cs) | 🔴 | I'm quite happy with my solution with a doubly linked list but I feel you'd have a much better shot at getting it tiny with a regular list. Again no-where near a one-liner. |
| [21](AdventOfCode2022/Day21.cs) | 🔴 | Not that it's small or anything but I'm quite happy with my solution especially to part 2. I took the approach of a recursive function down the tree evaluating for part 1, then for part 2, looked down each side, evaluated the side wihout humn and algebraically solved for the side of humn. |
| [22](AdventOfCode2022/Day22.cs) | 🔴 | Okay today's is horrific. It only works with the input net, not with the test one and it is a MASSIVE set of ifs just to work out where to go after falling off the edge, honestly though I'm still quite happy I managed it, with the aid of [a cube](Day22Cube.jpg) |
| [23](AdventOfCode2022/Day23.cs) | 🟠 | I'm quite happy with today's solution (not that it's at all fast), I've reduced the code down a good amount and it works. Lovely. |
| [24](AdventOfCode2022/Day24.cs) | 🔴 | I'm not entirely sure that my part 2 works, I think there's a couple seperate off-by-one errors that happened to line up for my input. Again todays was mostly solved by caching routes, and it took me a while to realise you can just calculate where the blizzards are at any given timestamp, you don't need to cache them too. |
| [25](AdventOfCode2022/Day25.cs) | 🟢 | A kind Christmas day challenge that still had a fun twist making it challenging, but adding to make the numbers positive solved that issue. Ended up with a clean one-liner with a recursive function for todays. |