package ca.jwolf.cubesolver.tools

import ca.jwolf.cubesolver.utils.ProgramUtils

class SetWorkingDirTool: Tool {
    override fun toolName(): String {
        return "Set Working Dirs"
    }

    override fun run(): Boolean {

        val newConfigDir = ProgramUtils.getUserInputString("New Config Directory")
        val newOutputDir = ProgramUtils.getUserInputString("New Output Directory")

        ProgramUtils.SetupConfigurationDirectory(newConfigDir)
        ProgramUtils.OutputDirectory = newOutputDir

        return true

    }
}