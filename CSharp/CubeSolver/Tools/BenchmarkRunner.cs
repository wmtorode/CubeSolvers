using CubeSolver.Utils;
using NetCubeSolver;
using NetCubeSolver.Benchmarking;

namespace CubeSolver.Tools;

public class BenchmarkRunner: ITool
{
    
    private BenchmarkSettings _benchmarkSettings;
    private BenchmarkResults _benchmarkResults;
    
    public string Name => "Run Benchmark";

    public bool Run()
    {
        _benchmarkSettings = ProgramUtils.GetBenchmarkSettingsFromUser();
        _benchmarkResults = new BenchmarkResults();
        
        
        
        return true;
    }
}