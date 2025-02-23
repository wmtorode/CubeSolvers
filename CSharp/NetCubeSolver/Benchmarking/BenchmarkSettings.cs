namespace NetCubeSolver.Benchmarking;

public class BenchmarkSettings
{
    public int Iterations { get; set; }
    public bool MultiThreaded { get; set; }
    public int MaxThreads { get; set; }
    public List<BenchmarkConfig> Configurations { get; set; } = new();
}