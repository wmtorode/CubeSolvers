package ca.jwolf.cubesolver.datamodels

import ca.jwolf.cubesolver.configs.PuzzleConfig

interface Solver {

    fun addSolution(solutions: Map<String, PuzzleSolution>, duplicates: Int)

    fun solve(puzzleConfig: PuzzleConfig, initialDepth: Int, maxThreads: Int, writeSolutions: Boolean, verbose: Boolean)

    fun solveTimeMs(): Long

}