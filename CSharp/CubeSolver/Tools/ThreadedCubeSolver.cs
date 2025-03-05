using System.Diagnostics;
using CubeSolver.Utils;
using NetCubeSolver;
using NetCubeSolver.ExactCover;

namespace CubeSolver.Tools;

public class ThreadedCubeSolver: BaseSolver, ITool, ISolver
{
    public string Name { get; } = "Multi-Threaded Cube Solver";

    private int DuplicateSolutions = 0;
    private Dictionary<string, PuzzleSolution> Solutions = new();
    private readonly object _solutionsLock = new();
    private ManualResetEvent _doneEvent = new(false);
    private int _jobsUnreported = 0;
    private Stopwatch _stopwatch;
    
    public bool Run()
    {

        var config = ProgramUtils.GetConfigFromUser();
        var initialDepth = ProgramUtils.GetUserInputInt("Initial Depth", 1);
        var maxThreads = ProgramUtils.GetUserInputInt("Max Threads", Environment.ProcessorCount);
        
        Solve(config, initialDepth, maxThreads, true, true);
        
        return true;
    }
    
    public void Solve(Config config, int initialDepth, int maxThreads, bool writeSolutions, bool verbose)
    {
        Solutions.Clear();
        DuplicateSolutions = 0;
        
        CubeConfig = config;
        Solution = new PuzzleSolution(CubeConfig.CubeSize, CubeConfig.PuzzlePieceSymbolLut);
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('=', CubeConfig.DisplayName);
            Console.WriteLine();
            PrintPieceData();
            Console.WriteLine();
        }
        
        _stopwatch = Stopwatch.StartNew();
        
        // Generate the Exact Cover Matrix
        var watch = Stopwatch.StartNew();
        GenerateCoverMatrix();
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Cover Matrix Generation took {watch.ElapsedMilliseconds:n0} ms");
        }
        
        // Create the Nodes we will need to run the "Dancing Links" on
        var dancingLinks = CreateDancingLinks();
        
        watch = Stopwatch.StartNew();
        dancingLinks.Search(0, initialDepth);
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-',
                $"Dancing Links Initial Generation took {watch.ElapsedMilliseconds:n0} ms and found {dancingLinks.Solutions.Count:n0} partial solutions");
        }

        watch = Stopwatch.StartNew();
        int currentIOC;
        if (maxThreads > 0)
        {
            
            ThreadPool.GetMinThreads(out var minThreads, out currentIOC);
            ThreadPool.SetMinThreads(maxThreads, currentIOC);
            ThreadPool.GetMaxThreads(out var threads, out currentIOC);
            ThreadPool.SetMaxThreads(maxThreads, currentIOC);
        }
        
        int newThreadCount;
        ThreadPool.GetMaxThreads(out newThreadCount, out currentIOC);
        
        if (verbose)
        {
            ProgramUtils.Write($"Progressing using up to {newThreadCount} threads");
        }
        
        _doneEvent.Reset();
        var events = new ManualResetEvent[]
        {
            _doneEvent
        };

        _jobsUnreported = dancingLinks.Solutions.Count;
        if (CubeConfig.Verbose && verbose) ProgramUtils.ReWrite($"Unique Solutions: 0 (+0), Duplicate Solutions: 0 (+0)");
        
        for(int i = 0; i < dancingLinks.Solutions.Count; i++)
        {
            var threadedDancingLinks = new ThreadedDancingLinks(MatrixColumnNames, CoverMatrix,
                dancingLinks.Solutions[i], this, CubeConfig);

            ThreadPool.QueueUserWorkItem(threadedDancingLinks.Search);
        }
        
        WaitHandle.WaitAll(events);
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.Write();
            ProgramUtils.WriteFilledLine('-',
                $"DLX Jobs took {watch.ElapsedMilliseconds:n0} ms, found {Solutions.Count:n0} unique solutions, {DuplicateSolutions:n0} duplicate solutions");
        }
        
        watch = Stopwatch.StartNew();
        var keys = Solutions.Keys.ToArray();
        Array.Sort(keys);
        watch.Stop();
        
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Sorting Solutions took {watch.ElapsedMilliseconds:n0} ms");
        }

        if (writeSolutions)
        {
            watch = Stopwatch.StartNew();
            WriteToTextFile(Solutions, keys);
            watch.Stop();
            if (verbose)
            {
                ProgramUtils.WriteFilledLine('-', $"Write to text file took {watch.ElapsedMilliseconds:n0} ms");
            }
        } 

        _stopwatch.Stop();
        SolveTime = _stopwatch.ElapsedMilliseconds;
        if (verbose)
        {
            ProgramUtils.WriteFilledLine('-', $"Total Time Elapsed : {_stopwatch.ElapsedMilliseconds:n0} ms");
        }
        
        Solutions.Clear();
        dancingLinks.Clear();
        CoverMatrix.Clear();
        MatrixColumnNames.Clear();
    }

    public long SolveTime { get; private set;}


    public void AddSolutions(Dictionary<string, PuzzleSolution> solutions, int duplicates)
    {
        lock (_solutionsLock)
        {
            var initDups = DuplicateSolutions;
            var initSolutions = Solutions.Count;
            DuplicateSolutions += duplicates;
            foreach (var solutionKP in solutions)
            {
                if (!Solutions.TryAdd(solutionKP.Key, solutionKP.Value))
                {
                    DuplicateSolutions++;
                }
            }

            _jobsUnreported -= 1;
            if (_jobsUnreported <= 0)
            {
                _doneEvent.Set();
            }
            
            if (CubeConfig.Verbose) ProgramUtils.ReWrite($"Unique Solutions: {Solutions.Count:n0} (+{Solutions.Count-initSolutions:n0}), Duplicate Solutions: {DuplicateSolutions:n0} (+{DuplicateSolutions-initDups:n0})               ");
        }
    }

    
}