package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.configs.PuzzlePiece
import ca.jwolf.cubesolver.configs.SubPiece
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.utils.ProgramUtils
import java.io.File
import java.nio.file.Paths
import java.util.BitSet

abstract class BaseSolver {

    protected val matrixColumnNames = mutableListOf<Int>()
    protected val coverMatrix = mutableListOf<BitSet>()

    // inheriting classes must set these prior to it being useful
    protected lateinit var solution: PuzzleSolution
    protected lateinit var config: PuzzleConfig

    protected fun printPieceData()
    {
        config.puzzlePieces.forEach { piece ->
            ProgramUtils.writeLine(getPieceInfo(piece))
        }
    }



    protected fun generateCoverMatrix(){

        matrixColumnNames.clear()
        coverMatrix.clear()

        config.puzzlePieces.forEach { piece ->
            matrixColumnNames.add(piece.id)
        }

        matrixColumnNames.addAll(solution.getPositionalHeaders())

        for(i in 0 until config.puzzlePieces.size){
            for (o in 0 until config.puzzlePieces[i].maxOrientation){
                for (z in 0 until config.cubeSize){
                    for (y in 0 until config.cubeSize){
                        for (x in 0 until config.cubeSize){
                            val row = getMatrixRow(i, o, x, y, z)
                            if (row != null) {
                                coverMatrix.add(row)
                            }
                        }
                    }
                }
            }
        }
    }

    protected fun writeToTextFile(solutions: Map<String, PuzzleSolution>, sortedKeys: List<String>)
    {
        File(Paths.get(ProgramUtils.OutputDirectory, "${config.outputFileName}.txt").toString()).bufferedWriter().use { writer ->
            config.puzzlePieces.forEach { piece ->
                writer.write(getPieceInfo(piece))
                writer.newLine()
            }

            writer.newLine()
            writer.newLine()

            var solutionNumber = 0
            sortedKeys.forEach { key ->
                solutions[key]!!.writeToTextFile("Solution ${solutionNumber}", writer)
                solutionNumber++
                writer.newLine()
            }

        }
    }

    private fun getMatrixRow(pieceId: Int, orientationId: Int, x: Int, y: Int, z:Int): BitSet? {
        val row = BitSet(matrixColumnNames.size)
        row[pieceId] = true

        val piece = config.puzzlePieces[pieceId]
        piece.currentOrientation = orientationId
        val transform = SubPiece(x, y, z)
        piece.transform(transform)
        piece.geometry().forEach { component ->
            val index = findColumn(component)
            // piece would fall outside the valid area
            if (index == -1) {
                return null
            }
            row.set(index)
        }
        return row
    }

    private fun findColumn(subPiece: SubPiece): Int {
        if (subPiece.x < 0 || subPiece.x >= config.cubeSize ||
            subPiece.y < 0 || subPiece.y >= config.cubeSize ||
            subPiece.z < 0 || subPiece.z >= config.cubeSize)
        {
            return -1
        }
        return config.puzzlePieces.size + subPiece.x + (subPiece.y * config.cubeSize) + (subPiece.z * config.cubeSize * config.cubeSize)
    }

    private fun getPieceInfo(puzzlePiece: PuzzlePiece): String {
        return "Piece ${puzzlePiece.id}, using Symbol: ${puzzlePiece.symbol}, has ${puzzlePiece.components.size} Cube(s) and ${puzzlePiece.maxOrientation} Unique Orientation(s)"
    }

}