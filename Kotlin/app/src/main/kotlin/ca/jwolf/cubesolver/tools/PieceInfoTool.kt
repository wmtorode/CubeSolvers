package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.configs.PuzzlePiece
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.utils.ProgramUtils

class PieceInfoTool: Tool {
    override fun toolName(): String {
        return "Piece Info"
    }


    override fun run(): Boolean {

        val config = ProgramUtils.getConfigFromUser()
        val solution = PuzzleSolution(config.cubeSize, config.getPuzzlePieceSymbolLookup())

        val pieceNames = config.puzzlePieces.map { it.getInformationalText() }

        val pieceChoosen = ProgramUtils.getUserSelection("Select Piece", pieceNames)

        val piece = config.puzzlePieces[pieceChoosen]

        println(" Piece: ${piece.id}")
        println(" Name: ${piece.description}")
        println(" Symbol: ${piece.symbol}")
        println(" Unique Orientations: ${piece.maxOrientation}")
        println()

        printOrientation(piece, solution)

        while (piece.rotate()){
            printOrientation(piece, solution)
        }

        return true
    }

    private fun printOrientation(piece: PuzzlePiece, solution: PuzzleSolution) {

        piece.snapToValidInsert(solution.size)
        solution.insertPiece(piece)
        print(solution.renderToString("Orientation: ${piece.currentOrientation}"))
        solution.removePiece(piece.id)

    }


}