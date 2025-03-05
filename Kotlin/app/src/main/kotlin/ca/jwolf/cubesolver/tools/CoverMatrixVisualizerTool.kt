package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.utils.ProgramUtils
import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.formatWithCommas
import java.util.BitSet
import kotlin.time.measureTime

class CoverMatrixVisualizerTool: Tool, BaseSolver() {
    override fun toolName(): String {
        return "Cover Matrix Visualizer"
    }

    override fun run(): Boolean {

        config = ProgramUtils.getConfigFromUser()
        solution = PuzzleSolution(config.cubeSize, config.getPuzzlePieceSymbolLookup())

        val elapsed =  measureTime {
            generateCoverMatrix()
        }

        ProgramUtils.writeFilledLine('-', "Cover Matrix generation took ${elapsed.inWholeMilliseconds.formatWithCommas()} ms")
        println()

        val columnCount = matrixColumnNames.size

        printRow(matrixColumnNames)
        coverMatrix.forEach{ row ->
            printRow(prepBitSetForPrint(row, columnCount))
        }

        println()
        ProgramUtils.writeFilledLine('-')
        ProgramUtils.writeLine("Matrix Columns: $columnCount")
        ProgramUtils.writeLine("Matrix Rows: ${coverMatrix.size}")

        return true

    }

    private fun <T> printRow(values: List<T>) {
        values.forEach { value ->
            print(value.toString().padStart(8))
        }
        println()
    }

    private fun prepBitSetForPrint(bitSet: BitSet, columns: Int): List<Boolean> {
        var result = mutableListOf<Boolean>()
        // using the column count is necessary due to how BitSets work
        // asking the bitset for its size will return the number of total bits in the set, which may be bigger than the requested size (for example requesting a size of 34 bits, will actually give you 64 since that aligns to a long
        // asking the bitset for its length will return the highest bit in the set that is set, which results in an odd printing
        for (i in 0 until columns) {
            if (!bitSet[i]) {
                result.add(false)
            }
            else
            {
                result.add(true)
            }
        }
        return result
    }

}