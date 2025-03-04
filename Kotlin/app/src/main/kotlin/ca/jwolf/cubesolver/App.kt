package ca.jwolf.cubesolver

import ca.jwolf.cubesolver.tools.*
import ca.jwolf.cubesolver.utils.ProgramUtils


class App{

    fun runApp(args: Array<String>)
    {
        val tools: List<Tool> = listOf(
            SetWorkingDirTool(),
            DemoConfigCreatorTool(),
            ConfigVisualizer(),
            PieceInfoTool(),
            CoverMatrixVisualizerTool(),
            SingleThreadedSolver()
        )

        ProgramUtils.writeFilledLine('#')
        ProgramUtils.writeFilledLine(' ', "Select Run Mode")
        ProgramUtils.writeFilledLine('#')
        ProgramUtils.writeLine()

        ProgramUtils.SetupConfigurationDirectory(System.getProperty("user.dir"))
        ProgramUtils.OutputDirectory = ProgramUtils.ConfigurationDirectory

        if (args.count() > 0)
        {
            ProgramUtils.SetupConfigurationDirectory(args[0])
        }

        if (args.count() > 1)
        {
            ProgramUtils.OutputDirectory = args[1]
        }

        ProgramUtils.writeLine("Config Dir: ${ProgramUtils.ConfigurationDirectory}")
        ProgramUtils.writeLine("Output Dir: ${ProgramUtils.OutputDirectory}")

        val toolNames = tools.map { it.toolName() }

        while (true)
        {
            val selectedTool = ProgramUtils.getUserSelection("Options", toolNames)
            val selectedToolInstance = tools[selectedTool]

            ProgramUtils.indent()
            selectedToolInstance.run()
            ProgramUtils.writeFilledLine('=')
            ProgramUtils.unindent()
            ProgramUtils.writeLine()
        }
    }
}

fun main(args: Array<String>) {

   App().runApp(args)

}