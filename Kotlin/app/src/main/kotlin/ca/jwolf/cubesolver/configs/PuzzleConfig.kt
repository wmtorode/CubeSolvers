package ca.jwolf.cubesolver.configs

import com.fasterxml.jackson.annotation.JsonIgnore

class PuzzleConfig(
    val id: String,
    val displayName: String,
    val outputFileName: String,
    val cubeSize: Int,
    var verbose: Boolean,
    val puzzlePieces: List<PuzzlePiece>
    )
{

    fun initializePieces() {
        puzzlePieces.forEach { it.initialize() }
    }

    @JsonIgnore
    fun getPuzzlePieceSymbolLookup(): Map<Int, String> {
        return puzzlePieces.associate { it.id to it.symbol } + (-1 to "-")
    }
}