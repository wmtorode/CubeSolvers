using System.Collections;

namespace NetCubeSolver.ExactCover;

public class ThreadedDancingLinks
{
    
    private List<int> MatrixColumnNames;
    private List<BitArray> CoverMatrix;
    private List<CoverNode> PartialSolution = new List<CoverNode>();
    private ISolver Solver;
    private Dictionary<int, string> PieceLUT;
    private int CubeSize;

    public ThreadedDancingLinks(List<int> matrixColumnNames, List<BitArray> coverMatrix, List<CoverNode> partialSolution,
        ISolver solver, int cubeSize, Dictionary<int, string> pieceLUT)
    {
        MatrixColumnNames = matrixColumnNames;
        CoverMatrix = coverMatrix;
        PartialSolution = partialSolution;
        Solver = solver;
        CubeSize = cubeSize;
        PieceLUT = pieceLUT;
    }

    public void Search(object? stateInfo)
    {
        var dancingLinks = new DancingLinks(MatrixColumnNames, CoverMatrix);
        dancingLinks.ApplyPartialCover(PartialSolution);
        dancingLinks.Search();

        var unprocessedSolutions = dancingLinks.ConvertedSolutions(PieceLUT.Count - 1, PartialSolution);
        var solutions = new List<PuzzleSolution>();
        int zDiv = CubeSize * CubeSize;
        
        foreach (var solution in unprocessedSolutions)
        {
            solutions.Add(PuzzleSolution.ConvertToSolution(CubeSize, PieceLUT,
                zDiv, solution));
        }
        
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