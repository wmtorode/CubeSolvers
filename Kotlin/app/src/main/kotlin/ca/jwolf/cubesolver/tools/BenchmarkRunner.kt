package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.benchmarking.BenchmarkConfig
import ca.jwolf.cubesolver.benchmarking.BenchmarkResult
import ca.jwolf.cubesolver.benchmarking.BenchmarkResults
import ca.jwolf.cubesolver.benchmarking.BenchmarkSettings
import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.datamodels.Solver
import ca.jwolf.cubesolver.utils.ProgramUtils
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.ConfigurationDirectory
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.formatWithCommas
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.jsonMapper
import com.fasterxml.jackson.module.kotlin.readValue
import kotlinx.coroutines.runBlocking
import java.io.File
import java.nio.file.Paths

class BenchmarkRunner: Tool {

    private lateinit var benchmarkSettings: BenchmarkSettings
    private lateinit var benchmarkResults: BenchmarkResults
    private lateinit var puzzleConfigs : List<PuzzleConfig>


    override fun toolName(): String {
       return "Run Benchmark"
    }

    override fun run(): Boolean {

        benchmarkSettings = ProgramUtils.getBenchmarkSettingsFromUser()
        benchmarkResults = BenchmarkResults()
        loadPuzzleConfigs()

        val solver: Solver
        if (benchmarkSettings.multiThreaded)
        {
            solver = ThreadedSolver()
        }
        else
        {
            solver = SingleThreadedSolver()
        }

        benchmarkSettings.configurations.forEach { config ->
            val puzzleConfig = puzzleConfigs.find { pConfig -> pConfig.id == config.configId }

            if (puzzleConfig == null)
            {
                ProgramUtils.writeFilledLine('!', "Error: No puzzle config found for id: ${config.configId}")
                return false
            }

            val threadCount = getMaxThreads(config)
            ProgramUtils.writeFilledLine('=', "Running benchmark for config: ${puzzleConfig.displayName}, Threaded: ${benchmarkSettings.multiThreaded}, Threads: $threadCount, Depth: ${config.initialDepth}")

            val benchmarkResult = BenchmarkResult(
                puzzleConfig.id,
                puzzleConfig.displayName,
                threadCount,
                if (benchmarkSettings.multiThreaded) config.initialDepth else 0
            )

            ProgramUtils.indent()
            for(i in 0 until benchmarkSettings.iterations)
            {
                // we want to block here, only 1 iteration should run at a time
                runBlocking {
                    solver.solve(puzzleConfig, config.initialDepth, threadCount, false, false)
                }
                benchmarkResult.runDurationsMs.add(solver.solveTimeMs())
                ProgramUtils.writeFilledLine('.', "Iteration ${i + 1} of ${benchmarkSettings.iterations} took: ${solver.solveTimeMs().formatWithCommas()} ms")

            }
            ProgramUtils.unindent()
            benchmarkResult.totalDurationMs = benchmarkResult.runDurationsMs.sum()
            benchmarkResult.averageRunDurationMs = benchmarkResult.totalDurationMs / benchmarkResult.runDurationsMs.size
            benchmarkResult.shortestRunDurationMs = benchmarkResult.runDurationsMs.min()
            benchmarkResult.longestRunDurationMs = benchmarkResult.runDurationsMs.max()
            benchmarkResults.results.add(benchmarkResult)
            ProgramUtils.writeFilledLine('=', benchmarkResult.getSummary())
            ProgramUtils.writeLine()

        }

        val results = jsonMapper.writeValueAsBytes(benchmarkResults)
        File(Paths.get(ProgramUtils.OutputDirectory, "BenchmarkResultsKotlin-${benchmarkSettings.id}.json").toString()).writeBytes(results)

        return true
    }

    private fun loadPuzzleConfigs()
    {
        val fileList = Paths.get(ConfigurationDirectory).toFile().listFiles()!!.map { it.name }.filter { it.endsWith(".json") }
        puzzleConfigs = fileList.map { jsonMapper.readValue<PuzzleConfig>(Paths.get(ConfigurationDirectory, it).toFile().readBytes().decodeToString()) }
        puzzleConfigs.forEach{
            it.initializePieces()
            it.verbose = false
        }
    }

    private fun getMaxThreads(benchmarkConfig: BenchmarkConfig) : Int
    {
        if (!benchmarkSettings.multiThreaded)
        {
            return 1
        }

        return if (benchmarkConfig.maxThreadsOverride > 0) benchmarkConfig.maxThreadsOverride else benchmarkSettings.maxThreads
    }
}