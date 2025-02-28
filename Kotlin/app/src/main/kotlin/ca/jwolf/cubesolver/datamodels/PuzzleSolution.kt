package ca.jwolf.cubesolver.datamodels

import ca.jwolf.cubesolver.configs.PuzzlePiece

class PuzzleSolution{
    val size: Int
    private val puzzlePieceSymbolsLookup: Map<Int, String>
    private var cubes: Array<Array<Array<Int>>> = emptyArray()

    constructor(puzzleSize: Int, pieceSymbolsLookup: Map<Int, String>) {
        size = puzzleSize
        puzzlePieceSymbolsLookup = pieceSymbolsLookup
        cubes = createEmptyCubes()
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

    private fun createEmptyCubes(): Array<Array<Array<Int>>> {
        return Array(size) { Array(size) { Array(size) { -1 } } }
    }



}