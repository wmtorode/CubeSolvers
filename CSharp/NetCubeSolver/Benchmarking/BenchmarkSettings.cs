namespace NetCubeSolver.Benchmarking;

public class BenchmarkSettings
{
    public string Name { get; set; }
    public string Id { get; set; }
    public int Iterations { get; set; }
    public bool MultiThreaded { get; set; } = false;
    public int MaxThreads { get; set; } = 1;
    public List<BenchmarkConfig> Configurations { get; set; } = new();
}