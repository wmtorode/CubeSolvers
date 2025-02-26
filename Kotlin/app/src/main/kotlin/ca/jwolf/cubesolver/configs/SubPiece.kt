package ca.jwolf.cubesolver.configs

data class SubPiece(var x: Int = 0, var y: Int = 0, var z: Int = 0): Comparable<SubPiece> {

    fun clone(): SubPiece {
        return SubPiece(x, y, z)
    }

    override fun compareTo(other: SubPiece): Int
    {
        if (z < other.z) return -1
        if (z > other.z) return 1

        if (y < other.y) return -1
        if (y > other.y) return 1

        if (x < other.x) return -1
        if (x > other.x) return 1

        return 0
    }

    fun rotate(axis: Axis)
    {
        val temp: Int
        when (axis)
        {
            Axis.X -> {
                temp = y
                y = z
                z = -temp
            }
            Axis.Y -> {
                temp = z
                z = x
                x = -temp
            }
            Axis.Z -> {
                temp = x
                x = y
                y = -temp
            }
        }
    }

    fun transform(subPiece: SubPiece)
    {
        x += subPiece.x
        y += subPiece.y
        z += subPiece.z
    }



}