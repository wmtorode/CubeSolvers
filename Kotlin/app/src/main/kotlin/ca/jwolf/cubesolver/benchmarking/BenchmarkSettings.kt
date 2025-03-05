package ca.jwolf.cubesolver.benchmarking

class BenchmarkSettings(val name: String, val id: String, val iterations: Int, val configurations: List<BenchmarkConfig>, val multiThreaded: Boolean=false, val maxThreads: Int=1) {
}