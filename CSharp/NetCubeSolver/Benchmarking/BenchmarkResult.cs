namespace NetCubeSolver.Benchmarking;

public class BenchmarkResult
{
    public string ConfigId { get; set; }
    public string ConfigName { get; set; }
    public int Threads { get; set; }
    public int InitialDepth { get; set; }
    public List<long> RunDurationsMs { get; set; } = new();
    public long TotalDurationMs { get; set; }
    public long LongestRunDurationMs { get; set; }
    public long ShortestRunDurationMs { get; set; }
    public long AverageRunDurationMs { get; set; }
}