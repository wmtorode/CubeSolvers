package ca.jwolf.cubesolver.datamodels

import ca.jwolf.cubesolver.configs.Axis
import ca.jwolf.cubesolver.configs.PuzzlePiece
import java.io.BufferedWriter

class PuzzleSolution(puzzleSize: Int, pieceSymbolsLookup: Map<Int, String>) {
    val size: Int = puzzleSize
    private val puzzlePieceSymbolsLookup: Map<Int, String> = pieceSymbolsLookup
    private var cubes: Array<Array<Array<Int>>> = emptyArray()

    init {
        cubes = createEmptyCubes()
    }

    constructor(puzzleSize: Int, pieceSymbolsLookup: Map<Int, String>, zDiv: Int, solutionData: List<Int>) : this(puzzleSize, pieceSymbolsLookup) {

        val pieceData: MutableList<Int> = mutableListOf()
        var currentPieceId = -1
        val pieceCount = puzzlePieceSymbolsLookup.size - 1

        // solutionData is a stream of ints representing the solved cube.
        // it can be divided in sections with each section separated by the value of -1
        // each value is a column from the cover matrix that represents that column being 'true' for that row
        // if the column value is within the piece lookup, then it represents what piece the current section is for
        // otherwise it represents a position within the solution that the piece occupies.

        solutionData.forEach { point ->
            // -1 is a delimiter between different pieces in the stream
            if (point != -1) {
                if (puzzlePieceSymbolsLookup.containsKey(point))
                {
                    currentPieceId = point
                }
                else
                {
                    pieceData.add(point - pieceCount)
                }
            }
            else
            {
                fillFromPieceData(currentPieceId, pieceData, zDiv)
                pieceData.clear()
            }
        }

    }

    fun getPositionalHeaders(): List<Int> {
        return (0 until size*size*size).map { it }
    }

    fun insertPiece(piece: PuzzlePiece): Boolean {
        var ret = true
        piece.geometry().forEach{ component ->
            if(cubes[component.z][component.y][component.x] == -1)
            {
                cubes[component.z][component.y][component.x] = piece.id
            }
            else
            {
                ret = false

            }
        }

        if (!ret) {
            removePiece(piece.id)
        }
        return ret
    }

    fun removePiece(pieceId: Int) {
        for (z in 0 until size) {
            for (y in 0 until size) {
                for (x in 0 until size) {
                    if (cubes[z][y][x] == pieceId) {
                        cubes[z][y][x] = -1
                    }
                }
            }
        }
    }

    fun renderToString(title: String): String {
        val sb = StringBuilder()
        sb.appendLine(title)
        sb.appendLine()
        for (y in 0 until size) {
            var line = "\t"
            for (z in 0 until size) {
                for (x in 0 until size) {
                    line = line.plus("${puzzlePieceSymbolsLookup[cubes[z][y][x]]} ")
                }
                line = line.plus("  ")
            }
            sb.appendLine(line)
        }
        sb.appendLine()
        return sb.toString()
    }

    fun getCanonicalSolutionString(): String {
        val orientations: MutableMap<String, Array<Array<Array<Int>>>> = mutableMapOf()
        val sb = StringBuilder()
        orientations[getCanonicalString(cubes, sb)] = cubes
        sb.clear()

        var tempCubes = cubes
        for (i in 0 until 23)
        {
            tempCubes = rotate(i, tempCubes)
            orientations[getCanonicalString(tempCubes, sb)] = tempCubes
            sb.clear()
        }

        val sortedKeys = orientations.keys.sorted()
        cubes = orientations[sortedKeys[0]]!!
        return sortedKeys[0]

    }

    fun writeToTextFile(title: String, writer: BufferedWriter) {
        writer.write(title)
        writer.newLine()
        writer.newLine()
        for (y in 0 until size) {
            writer.write("\t")
            for (z in 0 until size) {
                for (x in 0 until size) {
                    writer.write("${puzzlePieceSymbolsLookup[cubes[z][y][x]]} ")
                }
                writer.write("  ")
            }
            writer.newLine()
        }
        writer.newLine()
    }

    private fun getCanonicalString(cubes: Array<Array<Array<Int>>>, sb: StringBuilder): String {
        for (z in 0 until size) {
            for (y in 0 until size) {
                for (x in 0 until size) {
                    sb.append(puzzlePieceSymbolsLookup[cubes[z][y][x]])
                }
            }
        }
        return sb.toString()
    }

    private fun createEmptyCubes(): Array<Array<Array<Int>>> {
        return Array(size) { Array(size) { Array(size) { -1 } } }
    }

    private fun fillFromPieceData(pieceId: Int, pieceData: List<Int>, zDiv: Int) {
        pieceData.forEach { point ->
            var x = point
            val z = x / zDiv
            x %= zDiv
            val y = x / size
            x %= size

            cubes[z][y][x] = pieceId
        }
    }

    private fun rotate(axis: Axis, currentCubes: Array<Array<Array<Int>>>): Array<Array<Array<Int>>> {
        var tempCubes = createEmptyCubes()
        for (z in 0 until size) {
            for (y in 0 until size) {
                for (x in 0 until size) {
                    when(axis){
                        Axis.X -> tempCubes[(size - 1) - y][z][x] = currentCubes[z][y][x]
                        Axis.Y -> tempCubes[x][y][(size - 1) - z] = currentCubes[z][y][x]
                        Axis.Z -> tempCubes[z][(size - 1) - x][y] = currentCubes[z][y][x]
                    }
                }
            }
        }
        return tempCubes
    }

    private fun rotate(currentOrientation: Int, currentCubes: Array<Array<Array<Int>>>): Array<Array<Array<Int>>> {
        var tempCubes = currentCubes
        val newOrientation = currentOrientation + 1
        tempCubes = rotate(Axis.X, tempCubes)
        when(newOrientation){
            4,8,12 -> tempCubes = rotate(Axis.Y, tempCubes)
            16 -> {
                tempCubes = rotate(Axis.Y, tempCubes)
                tempCubes = rotate(Axis.Z, tempCubes)
            }
            20 -> {
                tempCubes = rotate(Axis.Z, tempCubes)
                tempCubes = rotate(Axis.Z, tempCubes)
            }
        }
        return tempCubes
    }


}