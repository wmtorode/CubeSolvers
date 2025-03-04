package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.dancinglinks.DancingLinks
import ca.jwolf.cubesolver.dancinglinks.ThreadedDancingLinks
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.datamodels.Solver
import ca.jwolf.cubesolver.utils.ProgramUtils
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.formatWithCommas
import kotlinx.coroutines.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import java.util.concurrent.Executors
import kotlin.time.measureTime

class ThreadedSolver: Tool, Solver, BaseSolver() {

    private var lastSolveTimeMs: Long = -1
    private var duplicateSolutionsCount = 0
    private val solutions: MutableMap<String, PuzzleSolution> = mutableMapOf()
    private val mutex = Mutex()



    override fun toolName(): String {
        return "Multi-Threaded Cube Solver"
    }

    override fun run(): Boolean {
        val puzzleConfig = ProgramUtils.getConfigFromUser()
        val initialDepth = ProgramUtils.getUserInputInt("Initial Depth", 1)
        val maxThreads = ProgramUtils.getUserInputInt("Max Threads", Runtime.getRuntime().availableProcessors())
        runBlocking {
            solve(puzzleConfig, initialDepth, maxThreads, true, true)
        }
        return true
    }

    override suspend fun addSolutions(potentialSolutions: Map<String, PuzzleSolution>, duplicates: Int) {
        mutex.withLock {
            val initDups = duplicateSolutionsCount
            val initSolutions = solutions.size
            duplicateSolutionsCount += duplicates
            potentialSolutions.forEach { (canonicalString, potentialSolution) ->
                if(solutions.containsKey(canonicalString)) {
                    duplicateSolutionsCount++
                }
                else {
                    solutions[canonicalString] = potentialSolution
                }
            }
            if (config.verbose)
            {
                ProgramUtils.reWrite("Unique Solutions: ${solutions.size.formatWithCommas()} (+${(solutions.size - initSolutions).formatWithCommas()}), Duplicate Solutions: ${duplicateSolutionsCount.formatWithCommas()} (+${(duplicateSolutionsCount - initDups).formatWithCommas()})")
            }
        }
    }

    override suspend fun solve(
        puzzleConfig: PuzzleConfig,
        initialDepth: Int,
        maxThreads: Int,
        writeSolutions: Boolean,
        verbose: Boolean
    )= coroutineScope {
        solutions.clear()
        duplicateSolutionsCount = 0

        config = puzzleConfig
        solution = PuzzleSolution(config.cubeSize, config.getPuzzlePieceSymbolLookup())

        if (verbose)
        {
            ProgramUtils.writeFilledLine('=', config.displayName)
            println()
            printPieceData()
            println()
        }

        val totalElapsed = measureTime {

            var elapsed = measureTime {
                generateCoverMatrix()
            }

            ProgramUtils.writeFilledLine('-', "Cover Matrix Generation took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms", verbose)

            val dlx = DancingLinks(matrixColumnNames, coverMatrix)

            elapsed = measureTime {
                dlx.search(0, initialDepth)
            }

            ProgramUtils.writeFilledLine('-', "Dancing Links Initial Search took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms and found ${dlx.solutions.size.formatWithCommas()} partial solutions", verbose)

            elapsed = measureTime {
                ProgramUtils.writeLine("Processing using up to $maxThreads threads", verbose)
                val dispatcher = Executors.newFixedThreadPool(maxThreads).asCoroutineDispatcher()
                val jobs: MutableList<Job> = mutableListOf()
                dlx.solutions.forEach { partialSolution ->
                    val threadedDlx = ThreadedDancingLinks(matrixColumnNames, coverMatrix, partialSolution, this@ThreadedSolver, config)
                    jobs.add(launch(dispatcher) {
                        threadedDlx.search()
                    })
                }
                jobs.joinAll()
                dispatcher.close()

            }

            if (verbose)
            {
                ProgramUtils.writeLine()
                ProgramUtils.writeFilledLine('-', "DLX jobs took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms, found ${solutions.size.formatWithCommas()} unique solutions, ${duplicateSolutionsCount.formatWithCommas()} duplicate solutions")
            }

            val sortedSolutionKeys: List<String>
            elapsed = measureTime {
                sortedSolutionKeys = solutions.keys.sorted()
            }
            ProgramUtils.writeFilledLine('-', "Sorting solutions took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms", verbose)

            if (writeSolutions) {
                elapsed = measureTime {
                    writeToTextFile(solutions, sortedSolutionKeys)
                }
                ProgramUtils.writeFilledLine('-', "Write to text file took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms", verbose)
            }

        }

        lastSolveTimeMs = totalElapsed.inWholeMilliseconds
        ProgramUtils.writeFilledLine('-', "Total solve time: ${lastSolveTimeMs.formatWithCommas()} ms", verbose)
    }

    override fun solveTimeMs(): Long {
        return lastSolveTimeMs
    }
}