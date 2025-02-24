namespace NetCubeSolver.Benchmarking;

public class BenchmarkConfig
{
    public string ConfigId {get; set;}
    public int MaxThreadsOverride { get; set; } = 0;
    public int InitialDepth { get; set; } = 1;

}