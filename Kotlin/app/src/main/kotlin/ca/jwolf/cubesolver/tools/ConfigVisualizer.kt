package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.datamodels.PuzzleSolution
import ca.jwolf.cubesolver.utils.ProgramUtils

class ConfigVisualizer: Tool {
    override fun toolName(): String {
        return "Config Visualizer"
    }

    override fun run(): Boolean {

        val config = ProgramUtils.getConfigFromUser()
        val solution = PuzzleSolution(config.cubeSize, config.getPuzzlePieceSymbolLookup())

        config.puzzlePieces.forEach{puzzlePiece ->
            puzzlePiece.snapToValidInsert(config.cubeSize)
            solution.insertPiece(puzzlePiece)
            print(solution.renderToString("Piece ${puzzlePiece.id}: ${puzzlePiece.description} has ${puzzlePiece.maxOrientation} unique orientation(s)"))
            solution.removePiece(puzzlePiece.id)

        }

        return true

    }
}