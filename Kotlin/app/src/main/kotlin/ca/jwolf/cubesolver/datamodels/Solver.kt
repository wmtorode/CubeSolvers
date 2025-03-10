package ca.jwolf.cubesolver.datamodels

import ca.jwolf.cubesolver.configs.PuzzleConfig

interface Solver {

    suspend fun addSolutions(potentialSolutions: Map<String, PuzzleSolution>, duplicates: Int)

    suspend fun solve(puzzleConfig: PuzzleConfig, initialDepth: Int, maxThreads: Int, writeSolutions: Boolean, verbose: Boolean)

    fun solveTimeMs(): Long

}