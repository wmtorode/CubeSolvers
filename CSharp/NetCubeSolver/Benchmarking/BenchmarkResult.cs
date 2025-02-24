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


    public string GetSummary()
    {
        return $"Benchmark for {ConfigName} Results: Max: {LongestRunDurationMs:n0}, Min: {ShortestRunDurationMs:n0}, Mean: {AverageRunDurationMs:n0} ms";
    }
}