# CubeSolvers

## What is this?
This repository is consists of multiple language implementations dedicated to solving cubic polycube puzzles where 
the objective is to take a set of polycube pieces and assemble them into an n x n x n cube with no empty spaces or 
extruding parts. Polycube puzzles will often have multiple possible solutions, so implementations must be able to find 
them all. In addition to unique solutions, due to the fact that the puzzle can be rotated, each solution will have an 
additional 23 rotational duplicates.

Note: while polycube puzzles can have differing sizes and dimensions, the implementations here are built for cubic puzzles only where all
dimensions of the puzzle are the same, such as 3x3x3 or 4x4x4 puzzles

Note 2: implementations here are not built to handle mirrored or identical pieces as the same solution, instead each 
since each mirrored or duplicate piece has a unique ID, each solution will be considered unique.

### Examples

Some examples of cube polycube puzzles are:
- Soma Cube (240/480 unique solutions)
- Tetris Cube (9,839 unique solutions)
- Bedlam Cube (19,186 unique solutions)


## Why this?

I believe a good way to learn or improve my understanding with a programming language is to implement a project in that language.
It just so happens that this project covers a good mix of things:
- File IO
- Building a CLI
- Taking user Input
- SerDes
- Math
- List, Arrays, Dictionaries and other notable data structures
- Multi-threading & Concurrency

While lacking a network component to it, this is still a good breadth of topics allowing you to learn alot about a language and how
provides a good benchmark for how performance can differ across languages and how to improve performance in a specific language.

This project initially took inspiration from an old C++ implementation of DLX I did many years ago that I came 
across while searching through old files on a backup system.

### Solving the Cubes

#### The Mind-Boggling Math

because of the number of pieces and the fact that each piece may have up to 24 unique orientations that it can be placed
in the cube and the number of possible permutations is mind-bogglingly big.

For example. The tetris Cube has 12 pieces:

that means if you wanted to search by hand for all solutions, you could line the pieces up in order and try to insert
the pieces, after each set of attempts, create a new order to try to insert the pieces until all possible orders have
been exhausted. doing so would yield 12 factorial or 479,001,600 possible orders.

Now recall that each piece may have up to 24 unique orientations and the math gets even bigger. Of the 12 pieces in the
Tetris Cube, 10 have 24 orientations and the 2 remaining have 12 each. Doing some math on this means that to exhaustively test
any particular piece order by trying all possible orientation combinations for it means you would try 9,130,086,859,014,144
unique orientation combinations **per piecing ordering** meaning a complete exhaustive test of all possible orderings
with all possible orientation combinations would equate to 4,373,326,213,606,749,398,630,400 different combinations to try.

Given that the Tetris Cube has 9,839 unique solutions and 236,136 total solutions (9839 x 24) when factoring in rotation
duplicates. that means any arbitrary lineup of pieces, each in an arbitrary orientation has about a 1 in 18,520,370,522,100,608,965 chance
of being a valid solution, or a 1 in 2,028 chance if orientations are not factored in.

#### The Naive Approach

One way to consider solving this is by brute force. However, even if you could try one combination per clock cycle,
a 4 GHz, 16 core CPU would still take 18,981,450,579 hours to exhaustively search all combinations.

Now of course, in practice the real number of combinations that you would need to try is nowhere near that high because 
huge numbers of combinations can be prematurely ruled out as many orientations in a slot will not fit, or would result 
in a void, automatically eliminating any combination that depends on those pieces being in those positions or 
orientations. Heuristics like these can make brute force searching an option and has been done in the past.

[Scott Kurowski](http://www.scottkurowski.com/tetriscube/), reported that such a search written using a C program he wrote, completed an exhaustive search using a 
single-core pentium 4, clocked at 2 GHz in ~43 hours. Long, yes but certainly feasible and computers today are much 
significantly faster and higher clocked today than they were then.

#### A Better Approach

So how can we do better? well it turns out that by framing the problem, correctly, the problem is actually a 
classic *Tiling problem*, which is itself an example of the [Exact Cover](https://en.wikipedia.org/wiki/Exact_cover) problem.

As it turns out, a reasonably efficient algorithm already exists for the exact cover problem in the 
form of [Knuth's Algorithm X](https://en.wikipedia.org/wiki/Knuth%27s_Algorithm_X), In particular we can use the 
Dancing Links (DLX) version of this. 

DLX uses sets of doubly-linked lists to quickly remove (cover) or reinsert (uncover) rows and columns from a matrix
until all remaining until each remaining column has exactly 1 row with a 1 and no rows with a 0 value.

in the case of our polycube puzzles, the matrix will have a column for each of the pieces and each possible position 
within the cube and one row for each possible set of locations an orientation for each piece can validly occupy.

In the case of our tetris cube this results in a 76 column matrix with 4080 rows.

So, how much faster is this? Well a true apples-to-apples comparison has not been done. However the C# implementation 
was tested to complete its search on my AMD Ryzen9 7950X in ~1.5 hours or roughly 28.5 times faster than Kurowski's 
implementation took on his laptop. While the hardware runs at twice the clock speed and IPC has been greatly improved since 
those days, its very doubtable that it alone is the reason for such an increase in speed.

#### Can we go even faster?

Yes! With a bit of extra work we can relatively efficiently multi-thread the DLX algorithm. To do this we slightly 
modify DLX to stop when a certain depth has been reached in its search. when this initial depth is reached. the current 
matrix is snap-shotted in the same manner as if it were a full solution.

Each of these partial solutions can then be applied to a new instance of the DLX runner, allowing it to continue 
searching for solutions from this starting point. the end results are then combined with the starting partial solution 
to form a full solution to the problem. Although this approach incurs some inefficiency because the matrix cannot be 
shared between each instance and must be created fresh for each one, the end result is still much faster on languages 
with efficient multi-threading capabilities. 

So how much better is it? Well running on that same 7950X as our single-threaded version and using all 16 cores/32 threads and an 
initial search depth of 2 (this seems to be the optimal value for the Tetris Cube), the multi-thread C# version can 
find all solutions within 6-7 minutes compared to 1.5 hours, so 12 - 15x faster.


## Configurations

### Puzzle Configs

This is the primary configuration file for a puzzle, containing details of the puzzle, and all its pieces.

example:
```json
{
  "Id": "SomaCube",
  "DisplayName": "Soma Cube",
  "OutputFileName": "SomaCubeSolutions",
  "CubeSize": 3,
  "UpdateInterval": 150,
  "Verbose": true,
  "PuzzlePieces": [
    {
      "Id": 0,
      "Symbol": "0",
      "Description": "V",
      "Components": [
        {
          "X": 0,
          "Y": 0,
          "Z": 0
        },
        {
          "X": 1,
          "Y": 0,
          "Z": 0
        },
        {
          "X": 0,
          "Y": 1,
          "Z": 0
        }
      ]
    }
  ]
}
```

- `Id`: a unique string identifier for this puzzle
- `DisplayName`: a display name for the puzzle
- `OutputFileName`: the name of the output file for solutions found for this puzzle, do not include a file extension
- `CubeSize`: the length of one side of the cube, expressed in the number of polycubes. for example a size of 3 means that the dimensions of the puzzle are 3 polycubes x 3 polycubes x 3 polycubes or 27 cubes in total
- `UpdateInterval`: Deprecated, a legacy field previously used to provide verbose updates while running
- `Verbose`: when `true` additional information will be output to the console while running
- `PuzzlePieces`: an array of all the puzzle pieces this puzzle contains

### Puzzle Pieces

This object describes the geometry and other information of a single puzzle piece within a puzzle

example:
```json
{
  "Id": 1,
  "Symbol": "1",
  "Description": "L",
  "Components": [
    {
      "X": 0,
      "Y": 0,
      "Z": 0
    },
    {
      "X": 1,
      "Y": 0,
      "Z": 0
    },
    {
      "X": 2,
      "Y": 0,
      "Z": 0
    },
    {
      "X": 2,
      "Y": 1,
      "Z": 0
    }
  ]
}
```

- `Id`: a unique identifier number to differentiate this piece from all others in the puzzle. Ideally this is enumerated from 0 - N-1 where N is the number of pieces in the puzzle. the value of `-1` is reserved by the application and should not be used.
- `Symbol`: a symbol that will be used in solutions that represent this piece. Generally this should be a single character long
- `Description`: a longer description of the piece, may describe its overall shape or characteristics
- `Components`: an array of polycubes that describe a single orientation of the piece within the overall solution

### PolyCubes

This represents a single polycube within a puzzle piece and is made of up a simple X,Y,Z coordinates that represent a location within the puzzle as a whole. a set of these is sufficient to provide the geometry of a piece.

example:
```json
{
  "X": 2,
  "Y": 1,
  "Z": 0
}
```


### Benchmark Settings

In order to provide measurable results for how well a given implementation compares to others benchmarks can be configured and then run

example:
```json
{
    "Id": "Benchmark32-100Soma",
    "Name": "32 Threads, 100 Iterations (Soma Only)",
    "Iterations": 100,
    "MultiThreaded": true,
    "MaxThreads": 32,
    "Configurations": [
        {
            "ConfigId": "SomaCube",
            "InitialDepth": 1
        }
    ]
}
```

- `Id`: a unique ID for this benchmark configuration, this ID will also be used for the resulting output file's name
- `Name`: a user-friendly name/description of the configuration
- `Iterations`: how many iterations of each configuration should be run for this benchmark
- `MultiThreaded`: when `true` configurations will be run in multi-threaded mode, when `false` the single-threaded implementation will be used instead
- `MaxThreads`: the maximum number of threads to use when using the multi-threaded runners
- `Configurations`: a list of configurations to run as part of this benchmark

### Benchmark Configurations

example:
```json
{
    "ConfigId": "SomaCube",
    "InitialDepth": 1,
    "MaxThreadsOverride": 0
}
```
- `ConfigId`: the ID of the puzzle configuration to be run
- `InitialDepth`: when using multi-threaded mode, the initial depth to run the first iteration of the DLX algorithm to
- `MaxThreadsOverride`: Optional, when using multi-threaded mode, override the max thread count for this configuration. if omitted or set to 0, then the settings `MaxThreads` value will be used