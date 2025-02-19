namespace NetCubeSolver.ExactCover;

public interface ISolver
{
    void AddSolutions(Dictionary<string, PuzzleSolution> solutions, int duplicates);
    
    void Solve(Config config, int initialDepth, int maxThreads, bool writeSolutions, bool verbose);
    
    long SolveTime { get; }
}