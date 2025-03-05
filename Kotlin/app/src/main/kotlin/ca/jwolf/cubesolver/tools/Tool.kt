package ca.jwolf.cubesolver.tools

interface Tool {

    fun toolName() : String
    fun run(): Boolean
}