package ca.jwolf.cubesolver.utils

import ca.jwolf.cubesolver.benchmarking.BenchmarkSettings
import ca.jwolf.cubesolver.configs.PuzzleConfig
import com.fasterxml.jackson.databind.DeserializationFeature
import com.fasterxml.jackson.databind.MapperFeature
import com.fasterxml.jackson.databind.SerializationFeature
import com.fasterxml.jackson.databind.json.JsonMapper
import com.fasterxml.jackson.module.kotlin.readValue
import com.fasterxml.jackson.module.kotlin.registerKotlinModule

import java.nio.file.Paths
import kotlin.system.exitProcess

class ProgramUtils {

    companion object {
        private const val LINE_LENGTH = 100
        private var indent = 0
        val jsonMapper = JsonMapper.builder()
            .configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true)
            .configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false)
            .enable(SerializationFeature.INDENT_OUTPUT)
            .build().registerKotlinModule()

        var ConfigurationDirectory = ""
            private set

        var OutputDirectory = ""

        var BenchmarkConfigurationDirectory = ""
            private set

        fun indent() {
            indent += 2
        }

        fun unindent() {
            indent -= 2
        }

        fun SetupConfigurationDirectory(directory: String) {
            ConfigurationDirectory = directory
            BenchmarkConfigurationDirectory = Paths.get(ConfigurationDirectory, "Benchmarks").toString()
        }

        fun writeLine(text: String ="", actuallyWrite: Boolean = true)
        {
            if (!actuallyWrite) {
                return
            }

            val prefix = " ".repeat(indent)
            println(prefix + text)
        }

        fun reWrite(text: String="", actuallyWrite: Boolean = true)
        {
            if (!actuallyWrite) {
                return
            }

            val prefix = " ".repeat(indent)
            print("\r$prefix$text")
        }

        fun writeFilledLine(fillCharacter: Char, text: String="", actuallyWrite: Boolean = true)
        {
            if (!actuallyWrite) {
                return
            }


            val targetLineLength = LINE_LENGTH - indent
            val textLength = text.length

            if (textLength == 0)
            {
                writeLine(fillCharacter.toString().repeat(targetLineLength))
            }
            else
            {
                val middleLineLength = (targetLineLength - textLength - 2)/2
                val filler = fillCharacter.toString().repeat(middleLineLength)
                writeLine("$filler $text $filler")
            }

        }

        fun getUserInputString(prompt: String, defaultValue: String="") : String
        {
            val inputPrompt = ">> $prompt "
            val prefix = " ".repeat(indent)
            var defaultedValue = ""
            if (defaultValue.isNotBlank()) defaultedValue = "($defaultValue) "
            print("$prefix$inputPrompt$defaultedValue")

            var input = readLine()
            input = input!!.replace(inputPrompt, "").trim()
            if (input.isBlank()) return defaultValue
            return input
        }

        fun getUserInputInt(prompt: String, defaultValue: Int=-1) : Int
        {
            val input = getUserInputString(prompt, defaultValue.toString())
            return input.toIntOrNull() ?: defaultValue
        }

        fun getUserSelection(context: String, options: List<String>) : Int
        {
            writeLine(":::: $context ::::")
            var optionIndex = 0
            for (option in options)
            {
                optionIndex++
                writeLine("$optionIndex: $option")
            }

            val lastOptionIndex = optionIndex
            optionIndex++
            writeLine("$optionIndex: Exit")

            val selected = getUserInputInt("Select an option: ")
            if (0 >= selected || selected > lastOptionIndex)
            {
                exitProcess(0)
            }


            return (selected - 1)
        }

        fun getConfigFromUser(): PuzzleConfig {
            val fileList = Paths.get(ConfigurationDirectory).toFile().listFiles()!!.map { it.name }.filter { it.endsWith(".json") }
            val configFile = getUserSelection("Select a config file to load:", fileList)

            val config =  jsonMapper.readValue<PuzzleConfig>(Paths.get(ConfigurationDirectory, fileList[configFile]).toFile().readBytes().decodeToString())
            config.initializePieces()
            return config
        }

        fun getBenchmarkSettingsFromUser(): BenchmarkSettings {
            val fileList = Paths.get(BenchmarkConfigurationDirectory).toFile().listFiles()!!.map { it.name }.filter { it.endsWith(".json") }
            val configFile = getUserSelection("Select a benchmark file to load:", fileList)

            val config =  jsonMapper.readValue<BenchmarkSettings>(Paths.get(BenchmarkConfigurationDirectory, fileList[configFile]).toFile().readBytes().decodeToString())
            return config
        }

        //extension methods
        fun Long.formatWithCommas(): String = String.format("%,d", this)
        fun Int.formatWithCommas(): String = String.format("%,d", this)

    }

}