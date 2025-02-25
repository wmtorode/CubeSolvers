package ca.jwolf.cubesolver.tools

import kotlinx.serialization.json.Json

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.configs.PuzzlePiece
import ca.jwolf.cubesolver.configs.SubPiece
import ca.jwolf.cubesolver.utils.ProgramUtils
import java.io.File
import java.nio.file.Paths

class DemoConfigCreatorTool: Tool {
    override fun toolName(): String {
        return "Demo Config Create"
    }

    override fun run(): Boolean {

        val subPiece = SubPiece()
        val puzzlePiece = PuzzlePiece(
            id = 0,
            description = "Demo Piece",
            symbol = "0",
            components = listOf(subPiece)
        )
        val config = PuzzleConfig(
            id = "Demo",
            displayName = "Demo Config",
            outputFileName = "DemoSolution.json",
            cubeSize = 2,
            verbose = false,
            puzzlePieces = listOf(puzzlePiece)
        )

        val prettyJson = Json {
            prettyPrint = true
            encodeDefaults = true
        }

        val configJson = prettyJson.encodeToString(config)
        File(Paths.get(ProgramUtils.ConfigurationDirectory, "DemoConfigKotlin.json").toString()).writeBytes(configJson.toByteArray())
        return true


    }
}