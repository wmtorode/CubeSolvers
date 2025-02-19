using System.Collections;
using System.Diagnostics;
using System.Text;
using CubeSolver.Utils;
using NetCubeSolver;

namespace CubeSolver.Tools;

public class CubeSolver: BaseSolver, ITool
{
    public string Name { get; } = "Cube Solver";

    
    public bool Run()
    {
        CubeConfig = ProgramUtils.GetConfigFromUser();

        Solution = new PuzzleSolution(CubeConfig.CubeSize, CubeConfig.GetPuzzlePieceSymbolLut());
        
        // print out some basics about the piece
        ProgramUtils.WriteFilledLine('=', CubeConfig.DisplayName);
        Console.WriteLine();
        PrintPieceData();
        Console.WriteLine();
        
        var totalWatch = Stopwatch.StartNew();
        
        
        // Generate the Exact Cover Matrix
        var watch = Stopwatch.StartNew();
        GenerateCoverMatrix();
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Cover Matrix Generation took {watch.ElapsedMilliseconds:n0} ms");
        
        // Create the Nodes we will need to run the "Dancing Links" on
        var dancingLinks = CreateDancingLinks();
        
        watch = Stopwatch.StartNew();
        dancingLinks.Search();
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Dancing Links Generation took {watch.ElapsedMilliseconds:n0} ms and found {dancingLinks.Solutions.Count:n0} solutions");

        int zDiv = CubeConfig.CubeSize * CubeConfig.CubeSize;

        watch = Stopwatch.StartNew();
        var unprocessedSolutions = dancingLinks.ConvertedSolutions(CubeConfig.PuzzlePieces.Count);
        var solutions = new List<PuzzleSolution>();
        var PieceLUT = CubeConfig.GetPuzzlePieceSymbolLut();
        foreach (var solution in unprocessedSolutions)
        {
            solutions.Add(PuzzleSolution.ConvertToSolution(CubeConfig.CubeSize, PieceLUT,
                zDiv, solution));
        }
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Conversion from Matrix Solutions to Puzzle Solutions took {watch.ElapsedMilliseconds:n0} ms");
        
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
        
        ProgramUtils.WriteFilledLine('-', $"Deduplication took {watch.ElapsedMilliseconds:n0} ms and found {uniqueSolutions.Count:n0} unique solutions, discarded: {duplicateSolutions:n0} duplicates");
        
        watch = Stopwatch.StartNew();
        var keys = uniqueSolutions.Keys.ToArray();
        Array.Sort(keys);
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Sorting Solutions took {watch.ElapsedMilliseconds:n0} ms");

        watch = Stopwatch.StartNew();
        WriteToTextFile(uniqueSolutions, keys);
        watch.Stop(); 
        
        ProgramUtils.WriteFilledLine('-', $"Write to text file took {watch.ElapsedMilliseconds:n0} ms");
        
        totalWatch.Stop();
        ProgramUtils.WriteFilledLine('-', $"Total Time Elapsed : {totalWatch.ElapsedMilliseconds:n0} ms");
        
        return true;
    }
    
    
    
}