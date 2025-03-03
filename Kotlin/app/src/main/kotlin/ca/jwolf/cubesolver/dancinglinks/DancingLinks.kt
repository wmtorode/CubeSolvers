package ca.jwolf.cubesolver.dancinglinks

import ca.jwolf.cubesolver.datamodels.CoverNode
import java.util.BitSet

class DancingLinks(maxtrixColumnNames: List<Int>, coverMatrix: List<BitSet>) {

    val solutions: MutableList<MutableList<CoverNode>> = mutableListOf()

    private val coverNodes :MutableList<CoverNode> = mutableListOf()
    private lateinit var headerNode:CoverNode

    private var partialSolution:MutableList<CoverNode> = mutableListOf()
    private var maxtrixSize:Int = 0

    init {
        maxtrixSize = maxtrixColumnNames.size
        initializeCoverNodes(maxtrixColumnNames, coverMatrix)
    }


    private fun initializeCoverNodes(maxtrixColumnNames: List<Int>, coverMatrix: List<BitSet>) {
        coverNodes.clear()
        headerNode = CoverNode(-1)
        headerNode.left = headerNode
        headerNode.right = headerNode

        var previousNode: CoverNode = headerNode
        for (i in 0 until maxtrixColumnNames.size)
        {
            val columnNode = CoverNode(i)
            coverNodes.add(columnNode)
            columnNode.left = previousNode
            previousNode.right = columnNode
            headerNode.left = columnNode
            columnNode.right = headerNode

            columnNode.up = columnNode
            columnNode.down = columnNode
            columnNode.column = columnNode

            previousNode = columnNode
        }

        coverMatrix.forEach { row ->
            addRowToCoverNodes(row)
        }

    }

    private fun addRowToCoverNodes(matrixRow: BitSet) {
        var previousNode: CoverNode? = null
        var columnHeader = headerNode
        for (i in 0 until maxtrixSize) {
            columnHeader = columnHeader.right!!
            if (matrixRow[i]) {
                val rowNode = CoverNode(i)
                coverNodes.add(rowNode)

                rowNode.column = columnHeader
                rowNode.column!!.size++

                columnHeader.up?.down = rowNode
                rowNode.down = columnHeader
                rowNode.up = columnHeader.up
                columnHeader.up = rowNode

                if (previousNode == null) {
                    rowNode.left = rowNode
                    rowNode.right = rowNode
                }
                else {
                    rowNode.left = previousNode
                    rowNode.right = previousNode.right
                    previousNode.right!!.left = rowNode
                    previousNode.right = rowNode
                }

                previousNode = rowNode

            }
        }
    }

}