/*
 * This file was generated by the Gradle 'init' task.
 */

plugins {
    id("buildlogic.kotlin-application-conventions")
    kotlin("plugin.serialization") version "2.1.10"

}

dependencies {
    implementation("org.apache.commons:commons-text")
    implementation("org.jetbrains.kotlinx:kotlinx-serialization-json:1.8.0")

}

tasks.getByName<JavaExec>("run") {
    standardInput = System.`in`
}

application {
    // Define the main class for the application.
    mainClass = "ca.jwolf.cubesolver.AppKt"
}
