using System.Collections;

namespace NetCubeSolver.ExactCover;

public class ThreadedDancingLinks
{
    
    private List<int> MatrixColumnNames;
    private List<BitArray> CoverMatrix;
    private List<CoverNode> PartialSolution = new List<CoverNode>();
    private ISolver Solver;
    private Config CubeConfig;

    public ThreadedDancingLinks(List<int> matrixColumnNames, List<BitArray> coverMatrix, List<CoverNode> partialSolution,
        ISolver solver, Config cubeConfig)
    {
        MatrixColumnNames = matrixColumnNames;
        CoverMatrix = coverMatrix;
        PartialSolution = partialSolution;
        Solver = solver;
        CubeConfig = cubeConfig;
    }

    public void Search(object? stateInfo)
    {
        var dancingLinks = new DancingLinks(MatrixColumnNames, CoverMatrix);
        dancingLinks.ApplyPartialCover(PartialSolution);
        dancingLinks.Search();

        var solutions = dancingLinks.ConvertedSolutions(CubeConfig, PartialSolution);
        
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
        
        Solver.AddSolutions(uniqueSolutions, duplicateSolutions);
        dancingLinks.Clear();
        PartialSolution.Clear();
    }
    
}