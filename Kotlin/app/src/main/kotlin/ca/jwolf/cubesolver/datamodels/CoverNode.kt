package ca.jwolf.cubesolver.datamodels

class CoverNode( val columnId: Int) {

    val isColumn = columnId >= 0
    var size = 0

    constructor() : this(-1) {
    }

    var left : CoverNode? = null
    var right: CoverNode? = null
    var up: CoverNode? = null
    var down: CoverNode? = null
    var column: CoverNode? = null

}