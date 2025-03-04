package ca.jwolf.cubesolver.dancinglinks

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.datamodels.CoverNode
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.datamodels.Solver
import java.util.BitSet

class ThreadedDancingLinks(private val maxtrixColumnNames: List<Int>, private val coverMatrix: List<BitSet>,
                           private val partialSolution: List<CoverNode>, private val solver: Solver,
                           private val config: PuzzleConfig) {


    suspend fun search(){
        val dlx = DancingLinks(maxtrixColumnNames, coverMatrix)
        dlx.applyPartialCover(partialSolution)
        dlx.search()

        val solutions = dlx.getPuzzleSolutions(config, partialSolution)

        val uniqueSolutions: MutableMap<String, PuzzleSolution> = mutableMapOf()
        var duplicates = 0

        solutions.forEach { solution ->
            val canonicalString = solution.getCanonicalSolutionString()
            if (uniqueSolutions.containsKey(canonicalString)) {
                duplicates++
            } else {
                uniqueSolutions[canonicalString] = solution
            }
        }

        solver.addSolutions(uniqueSolutions, duplicates)
        dlx.clear()

    }

}