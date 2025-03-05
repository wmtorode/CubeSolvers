using System.Collections;
using System.Diagnostics;
using System.Text;
using CubeSolver.Utils;
using NetCubeSolver;
using NetCubeSolver.ExactCover;

namespace CubeSolver.Tools;

public class CubeSolver: BaseSolver, ITool, ISolver
{
    public string Name { get; } = "Cube Solver";

    
    public bool Run()
    {
        var config = ProgramUtils.GetConfigFromUser();
        Solve(config, 0, 0, true, true);
        return true;
    }


    public void AddSolutions(Dictionary<string, PuzzleSolution> solutions, int duplicates)
    {
        
    }

    public void Solve(Config config, int initialDepth, int maxThreads, bool writeSolutions, bool verbose)
    {
        CubeConfig = config;

        Solution = new PuzzleSolution(CubeConfig.CubeSize, CubeConfig.PuzzlePieceSymbolLut);
        
        if (verbose)
        // print out some basics about the piece
        {
            ProgramUtils.WriteFilledLine('=', CubeConfig.DisplayName);
            Console.WriteLine();
            PrintPieceData();
            Console.WriteLine();
        }
        
        var totalWatch = Stopwatch.StartNew();
        
        
        // Generate the Exact Cover Matrix
        var watch = Stopwatch.StartNew();
        GenerateCoverMatrix();
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Cover Matrix Generation took {watch.ElapsedMilliseconds:n0} ms");
        }
        
        // Create the Nodes we will need to run the "Dancing Links" on
        var dancingLinks = CreateDancingLinks();
        
        watch = Stopwatch.StartNew();
        dancingLinks.Search();
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-',
                $"Dancing Links Generation took {watch.ElapsedMilliseconds:n0} ms and found {dancingLinks.Solutions.Count:n0} solutions");
        }

        watch = Stopwatch.StartNew();
        var solutions = dancingLinks.ConvertedSolutions(CubeConfig);
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-',
                $"Conversion from Matrix Solutions to Puzzle Solutions took {watch.ElapsedMilliseconds:n0} ms");
        }
        
        watch = Stopwatch.StartNew();
        Dictionary<string, PuzzleSolution> uniqueSolutions = new();
        int duplicateSolutions = 0;

        foreach (var solution in solutions)
        {
            var canonicalString = solution.FindAndSetToCanonicalSolution();
            if (uniqueSolutions.ContainsKey(canonicalString))
            {
                duplicateSolutions++;
            }
            else
            {
                uniqueSolutions.Add(canonicalString, solution);
            }
        }
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-',
                $"Deduplication took {watch.ElapsedMilliseconds:n0} ms and found {uniqueSolutions.Count:n0} unique solutions, discarded: {duplicateSolutions:n0} duplicates");
        }
        
        watch = Stopwatch.StartNew();
        var keys = uniqueSolutions.Keys.ToArray();
        Array.Sort(keys);
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Sorting Solutions took {watch.ElapsedMilliseconds:n0} ms");
        }
        
        if (writeSolutions)
        {
            watch = Stopwatch.StartNew();
            WriteToTextFile(uniqueSolutions, keys);
            watch.Stop();

            if (verbose)
            {
                ProgramUtils.WriteFilledLine('-', $"Write to text file took {watch.ElapsedMilliseconds:n0} ms");
            }
        }
        totalWatch.Stop();
        
        SolveTime = totalWatch.ElapsedMilliseconds;
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Total Time Elapsed : {totalWatch.ElapsedMilliseconds:n0} ms");
        }
    }

    public long SolveTime { get;  private set; }
}