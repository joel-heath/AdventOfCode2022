# Advent of Code 2022
My C# solutions to [Advent of Code 22](https://adventofcode.com/2022). Trying to one-liner as many as possible but I've already failed at Day 5.

## Notes

### Legend
🟢 Successful one-liner.

🟡 Could be reduced to one-liner but would require repeated evaluations of the same code.

🟠 Far from a one-liner, with minimal amounts of code reduced.

🔴 Barely a visible effort to reduce the code.

| **Day** | **Verbosity** | **Notes** |
|:---:|:---:|:---:|
| [1](AdventOfCode2022/Day1.cs) | 🟢 | [part 1 it was relatively trivial.] |
| [2](AdventOfCode2022/Day2.cs) | 🟢 | This one was hard due to trying to find a way to convert ASCII codes into the correct output, and I ended feeding the part 2 input into my part 1 solution. |
| [3](AdventOfCode2022/Day3.cs) | 🟢 | This one had the challenge of flipping the alphabet relative to ASCII codes and uses pretty arbitrary seeming codes, but it works. |
| [4](AdventOfCode2022/Day4.cs) | 🟢 | This one posed the interesting problem of reducing about 30 different comparisons as much as possible, and although sorting the list *technically adds more comparisons*, the visible comparisons are less. The ThenBy() function came in handy for equal bounds. |
| [5](AdventOfCode2022/Day5.cs) | 🔴 | Miles from a one-liner, the input parsing makes it ridiculously long from the get-go, but it could be made more shorter using hard-coded values, as my code doesn't assume there are 9 stacks. This is also the kind of one where reducing it to a one liner would mean copious amounts of repeated code and repeated evaluations. |
| [6](AdventOfCode2022/Day6.cs) | 🟢 | Day 6 was very much an apology for the stress caused by day 5, my solution being one-liner with less characters than day 1. |
| [7](AdventOfCode2022/Day7.cs) | 🟠 | Started of tragically, I went for the non-object-oriented approach and could not solve part 2. After an entire re-attempt I did solve it, and now it's at the point where the code is relatively minimal, alongside using two classes which haven't really been reduced. |
| [8](AdventOfCode2022/Day8.cs) | 🟡 | This one was quite easy to reduce down to a few seperate functions, with the grid being stored in a variable so the input doesn't have to be parsed multiple times. |
| [9](AdventOfCode2022/Day9.cs) | 🟠 | This was a good day. Making the rope move frame by frame was very satisfying. 10/10 would solve again, however, my code is realtively short but certainly could do with some simplifying, and it is all very imperitive approached. |
| [10](AdventOfCode2022/Day10.cs) | 🟡 | This is another one of a single massive return line, plus some variables stored, and a couple functions to mitigate a horrific looking return line. Part 2 involved passing in the cycle count to a function unneccesarily, just so I could increment it as I passed it in. |
| [11](AdventOfCode2022/Day11.cs) | 🔴 | Another day 5, I'm not even going to pretend this is anywhere close to a one-liner. It did provide a good use of a queue for the monkey's items, and that part 2 was a scoundrel. |
