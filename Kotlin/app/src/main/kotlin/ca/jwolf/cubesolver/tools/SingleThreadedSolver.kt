package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.dancinglinks.DancingLinks
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.datamodels.Solver
import ca.jwolf.cubesolver.utils.ProgramUtils
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.formatWithCommas
import kotlin.time.measureTime

class SingleThreadedSolver: Tool, Solver, BaseSolver() {

    private var lastSolveTimeMs: Long = -1

    override fun toolName(): String {
        return "Single Threaded Solver"
    }

    override fun run(): Boolean {
        val puzzleConfig = ProgramUtils.getConfigFromUser()
        lastSolveTimeMs = -1
        solve(puzzleConfig, 0, 0, true, true)
        return true
    }

    override fun addSolution(solutions: Map<String, PuzzleSolution>, duplicates: Int) {
        // intentionally empty, this isn't needed for single threaded operations
    }

    override fun solve(
        puzzleConfig: PuzzleConfig,
        initialDepth: Int,
        maxThreads: Int,
        writeSolutions: Boolean,
        verbose: Boolean
    ) {
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
                dlx.search()
            }

            ProgramUtils.writeFilledLine('-', "Dancing Links Search took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms and found ${dlx.solutions.size.formatWithCommas()} solutions", verbose)
            val processedSolutions: List<PuzzleSolution>

            elapsed = measureTime {
                val zDivider = config.cubeSize * config.cubeSize
                val unprocessedSolutions = dlx.getConvertedSolutions(config.puzzlePieces.size)
                processedSolutions = unprocessedSolutions.map { solution ->
                    PuzzleSolution(config.cubeSize, config.getPuzzlePieceSymbolLookup(), zDivider, solution)
                }
            }

            ProgramUtils.writeFilledLine('-', "Conversion from Matrix solutions to Puzzle solutions took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms", verbose)

            val uniqueSolutions: MutableMap<String, PuzzleSolution> = mutableMapOf()
            var duplicates = 0
            elapsed = measureTime {
                processedSolutions.forEach { solution ->
                    val canonicalString = solution.getCanonicalSolutionString()
                    if (uniqueSolutions.containsKey(canonicalString)) {
                        duplicates++
                    } else {
                        uniqueSolutions[canonicalString] = solution
                    }
                }
            }
            ProgramUtils.writeFilledLine('-', "Deduplication took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms and found ${uniqueSolutions.size.formatWithCommas()} unique solutions and ${duplicates.formatWithCommas()} duplicates", verbose)

            val sortedSolutionKeys: List<String>
            elapsed = measureTime {
                sortedSolutionKeys = uniqueSolutions.keys.sorted()
            }
            ProgramUtils.writeFilledLine('-', "Sorting solutions took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms", verbose)

            if (writeSolutions) {
                elapsed = measureTime {
                    writeToTextFile(uniqueSolutions, sortedSolutionKeys)
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