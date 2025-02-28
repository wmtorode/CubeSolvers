package ca.jwolf.cubesolver.configs

import com.fasterxml.jackson.annotation.JsonIgnore


class PuzzlePiece(

    val id: Int,
    val symbol: String,
    val description: String,
    val components: List<SubPiece>
)
    {

    @JsonIgnore
    var currentOrientation: Int = 0

    @JsonIgnore
    var maxOrientation: Int = 0

    @JsonIgnore
    private var orientations: MutableMap<Int, List<SubPiece>> = mutableMapOf()

    fun geometry(): List<SubPiece> {
        return orientations[currentOrientation]!!
    }

    @JsonIgnore
    fun getInformationalText(): String {
        return "$id: $description"
    }
    
    fun rotate(): Boolean {
        currentOrientation++
        if (currentOrientation >= maxOrientation) {
            currentOrientation = maxOrientation - 1
            return false
        }
        return true
    }
    
    fun snapToValidInsert(maxPoint: Int)
    {
        var maxX = 0
        var maxY = 0
        var maxZ = 0
        var minX: Int = Integer.MAX_VALUE
        var minY: Int = Integer.MAX_VALUE
        var minZ: Int = Integer.MAX_VALUE
        
        for (component in geometry()) {
            maxX = maxOf(maxX, component.x)
            maxY = maxOf(maxY, component.y)
            maxZ = maxOf(maxZ, component.z)
            minX = minOf(minX, component.x)
            minY = minOf(minY, component.y)
            minZ = minOf(minZ, component.z)
        }

        // find any subpiece that is outside the allowed area and calculate what it will take to move it
        // back into a legal space, when combined they will provide an overall transformation for the piece as a
        // whole to place it entirely within the valid area
        val transformX = when {
            minX < 0 -> -minX
            maxX > maxPoint -> maxPoint - maxX
            else -> 0
        }

        val transformY = when {
            minY < 0 -> -minY
            maxY > maxPoint -> maxPoint - maxY
            else -> 0
        }

        val transformZ = when {
            minZ < 0 -> -minZ
            maxZ > maxPoint -> maxPoint - maxZ
            else -> 0
        }


        val transformation = SubPiece(transformX, transformY, transformZ)
        transform(transformation)
    }

    fun transform(point: SubPiece) {
        val transformation = findTransformation(point)
        for (component in geometry()) {
            component.transform(transformation)
        }
    }

    fun initialize()
    {
        generateOrientations()
    }


    private fun generateOrientations() {
        orientations.clear()
        currentOrientation = 0
        normalize()
        orientations[currentOrientation] = components.map { it.copy() }
        for (i in 0..22)
        {
            rotateComponents()
            normalize()
            var unique = true
            for (orientation in orientations.values) {
                var found = true
                for (component in components) {
                    found = found && (component in orientation)
                }
                if (found) {
                    unique = false
                    break
                }
            }

            if (unique)
            {
                orientations[orientations.count()] = components.map { it.copy() }
            }
        }

        currentOrientation = 0
        maxOrientation = orientations.count()
    }

    private fun rotateComponents() {
        val newOrientation = currentOrientation + 1
        rotateComponents(Axis.X)
        when (newOrientation)
        {
            4, 8, 12 -> rotateComponents(Axis.Y)
            16 -> {
                rotateComponents(Axis.Y)
                rotateComponents(Axis.Z)
            }
            20 -> {
                rotateComponents(Axis.Z)
                rotateComponents(Axis.Z)
            }
        }
        currentOrientation = newOrientation
    }


    private fun rotateComponents(axis: Axis) {
        for (component in components) {
            component.rotate(axis)
        }
    }

    private fun normalize() {
        val transformation = findTransformationForPiece(SubPiece(0, 0, 0))
        for (component in components)
        {
            component.transform(transformation)
        }
    }

    private fun findTransformationForPiece(point: SubPiece): SubPiece {
        val baseComponentForTransformation = findLowestComponentOfBaseGeometry()
        return SubPiece(
            point.x - components[baseComponentForTransformation].x,
            point.y - components[baseComponentForTransformation].y,
            point.z - components[baseComponentForTransformation].z
        )
    }


    private fun findLowestComponentOfBaseGeometry(): Int {
        var lowestSubPiece = 0
        for (i in components.indices) {
            if (components[i] < components[lowestSubPiece]) {
                lowestSubPiece = i
            }
        }
        return lowestSubPiece
    }


    private fun findTransformation(point: SubPiece): SubPiece {
        val baseComponentForTransformation = findLowestComponentOfCurrentGeometry()
        val geometry = geometry()
        return SubPiece(
            point.x - geometry[baseComponentForTransformation].x,
            point.y - geometry[baseComponentForTransformation].y,
            point.z - geometry[baseComponentForTransformation].z
        )
    }

    private fun findLowestComponentOfCurrentGeometry(): Int
    {
        var lowestSubPiece = 0
        val geometry = geometry()
        for(i in geometry.indices)
        {
            if(geometry[i] < geometry[lowestSubPiece])
            {
                lowestSubPiece = i
            }
        }
        return lowestSubPiece
    }

}