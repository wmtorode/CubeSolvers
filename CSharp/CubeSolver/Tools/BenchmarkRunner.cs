using System.Text.Json;
using CubeSolver.Utils;
using NetCubeSolver;
using NetCubeSolver.Benchmarking;
using NetCubeSolver.ExactCover;

namespace CubeSolver.Tools;

public class BenchmarkRunner: ITool
{
    
    private BenchmarkSettings _benchmarkSettings;
    private BenchmarkResults _benchmarkResults;
    private List<Config> _configs;
    
    
    public string Name => "Run Benchmark";

    public bool Run()
    {
        _benchmarkSettings = ProgramUtils.GetBenchmarkSettingsFromUser();
        _benchmarkResults = new BenchmarkResults();
        LoadConfigs();

        ISolver solver;
        if (_benchmarkSettings.MultiThreaded)
        {
            solver = new ThreadedCubeSolver();
        }
        else
        {
            solver = new CubeSolver();
        }

        foreach (var benchmark in _benchmarkSettings.Configurations)
        {
            var config = GetConfig(benchmark);
            if (config == null)
            {
                ProgramUtils.WriteFilledLine('!', $"Error: No config with id {benchmark.ConfigId} found");
                return false;
            }
            
            var threadCount = GetThreadCount(benchmark);
            
            ProgramUtils.WriteFilledLine('=', $"Running benchmark for {config.DisplayName}, Threaded: {_benchmarkSettings.MultiThreaded}, Threads: {threadCount}, Depth: {benchmark.InitialDepth}");
            var benchmarkResult = new BenchmarkResult()
            {
                ConfigId = config.Id,
                ConfigName = config.DisplayName,
                InitialDepth = _benchmarkSettings.MultiThreaded ? benchmark.InitialDepth : 0,
                Threads = threadCount
            };

            ProgramUtils.Indent();
            for (int i = 0; i < _benchmarkSettings.Iterations; i++)
            {
                solver.Solve(config, benchmark.InitialDepth, threadCount, false, false);
                benchmarkResult.RunDurationsMs.Add(solver.SolveTime);
                ProgramUtils.WriteFilledLine('.', $"Iteration {i + 1} of {_benchmarkSettings.Iterations} Took: {solver.SolveTime:n0}ms");
            }
            ProgramUtils.Unindent();
            benchmarkResult.TotalDurationMs = benchmarkResult.RunDurationsMs.Sum();
            benchmarkResult.AverageRunDurationMs = benchmarkResult.TotalDurationMs / benchmarkResult.RunDurationsMs.Count;
            benchmarkResult.ShortestRunDurationMs = benchmarkResult.RunDurationsMs.Min();
            benchmarkResult.LongestRunDurationMs = benchmarkResult.RunDurationsMs.Max();
            _benchmarkResults.Results.Add(benchmarkResult);
            ProgramUtils.WriteFilledLine('=', benchmarkResult.GetSummary());
            ProgramUtils.Write();
        }
        
        var resultsFile = Path.Combine(ProgramUtils.OutputDirectory, $"BenchmarkResultsCSharp-{_benchmarkSettings.Id}.json");
        var writeOptions = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(resultsFile, JsonSerializer.Serialize(_benchmarkResults, writeOptions));
        
        ProgramUtils.Write();
        ProgramUtils.WriteFilledLine('=', "Benchmarking Complete:");
        ProgramUtils.Indent();
        foreach (var benchmarkResult in _benchmarkResults.Results)
        {
            ProgramUtils.Write(benchmarkResult.GetSummary());
        }
        ProgramUtils.Unindent();
        
        
        
        return true;
    }

    private void LoadConfigs()
    {
        _configs = new List<Config>();
        DirectoryInfo dir = new DirectoryInfo(ProgramUtils.WorkingDirectory);
        var configFiles = dir.GetFiles().Where(f => f.Extension == ".json").ToList();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        foreach (var configFile in configFiles)
        {
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configFile.FullName), options);
            config.InitializePieces();
            config.Verbose = false;
            _configs.Add(config);
        }
    }


    private Config GetConfig(BenchmarkConfig benchmarkConfig)
    {
        foreach (var config in _configs)
        {
            if (config.Id == benchmarkConfig.ConfigId) return config;
        }
        
        return null;
    }

    private int GetThreadCount(BenchmarkConfig benchmarkConfig)
    {
        if (!_benchmarkSettings.MultiThreaded)
        {
            return 1;
        }
        return benchmarkConfig.MaxThreadsOverride > 0? benchmarkConfig.MaxThreadsOverride : _benchmarkSettings.MaxThreads;
    }
}