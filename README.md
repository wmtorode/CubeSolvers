# CubeSolvers

## What is this?
This repository is consists of multiple language implementations dedicated to solving cube polycube puzzles where 
the objective is to take a set of polycube pieces and assemble them into an n x n x n cube with no empty spaces or 
extruding parts. Polycube puzzles will often have multiple possible solutions, so solutions must be able to find 
them all. In addition to unique solutions, due to the fact that the puzzle can be rotated, each solution will have an 
additional 23 rotational duplicates.

Note: while polycube puzzles can have differing sizes and dimensions, the implementations here are built for cube puzzles only where all
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
implementation took on his laptop. While the hardware runs at twice the close and IPC has been greatly improved since 
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