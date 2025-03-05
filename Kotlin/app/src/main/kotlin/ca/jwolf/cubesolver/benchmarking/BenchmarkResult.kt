package ca.jwolf.cubesolver.benchmarking

import ca.jwolf.cubesolver.utils.ProgramUtils.Companion.formatWithCommas
import com.fasterxml.jackson.annotation.JsonIgnore

class BenchmarkResult(val configId: String, val configName: String, val threads: Int, val initialDepth: Int,
                      val runDurationsMs: MutableList<Long> = mutableListOf(), var totalDurationMs: Long = 0,
                      var longestRunDurationMs: Long = 0, var shortestRunDurationMs: Long = 0, var averageRunDurationMs: Long = 0 ) {

    @JsonIgnore
    fun getSummary(): String {
        return "Benchmark for $configName Results: Max: ${longestRunDurationMs.formatWithCommas()}, Min: ${shortestRunDurationMs.formatWithCommas()}, Mean: ${averageRunDurationMs.formatWithCommas()} ms"
    }

}