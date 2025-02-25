package ca.jwolf.cubesolver.configs

import kotlinx.serialization.Serializable


@Serializable
class PuzzleConfig(
    val id: String,
    val displayName: String,
    val outputFileName: String,
    val cubeSize: Int,
    val verbose: Boolean,
    val puzzlePieces: List<PuzzlePiece>
    )
{

    fun initializePieces() {
        puzzlePieces.forEach { it.initialize() }
    }

    fun getPuzzlePieceSymbolLookup(): Map<Int, String> {
        return puzzlePieces.associate { it.id to it.symbol } + (-1 to "-")
    }
}