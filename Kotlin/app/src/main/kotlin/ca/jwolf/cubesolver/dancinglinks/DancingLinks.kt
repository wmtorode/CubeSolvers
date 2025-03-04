package ca.jwolf.cubesolver.dancinglinks

import ca.jwolf.cubesolver.configs.PuzzleConfig
import ca.jwolf.cubesolver.datamodels.CoverNode
import ca.jwolf.cubesolver.datamodels.PuzzleSolution
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

    fun clear()
    {
        solutions.clear()
        coverNodes.clear()
        partialSolution.clear()
        headerNode = CoverNode()
    }

    fun getPuzzleSolutions(config: PuzzleConfig, partialSolution:List<CoverNode>? = null):List<PuzzleSolution> {
        val partialData: MutableList<Int> = mutableListOf()
        val zDivider = config.cubeSize * config.cubeSize
        val pieceLookup = config.getPuzzlePieceSymbolLookup()

        partialSolution?.forEach { node ->
            var tempNode = node
            do {
                partialData.add(tempNode.column!!.columnId)
                tempNode = tempNode.right!!
            } while (tempNode != node)

            partialData.add(-1)
        }

//        val size = maxtrixSize + pieceCount
        val convertedSolutions: MutableList<PuzzleSolution> = mutableListOf()
        solutions.forEach { solution ->
            val solutionData = mutableListOf<Int>()
            solution.forEach {node ->
                var tempNode = node
                do {
                    solutionData.add(tempNode.column!!.columnId)
                    tempNode = tempNode.right!!
                } while (tempNode != node)

                solutionData.add(-1)
            }

            if (partialData.isNotEmpty()) {
                solutionData.addAll(partialData)
            }

            convertedSolutions.add(PuzzleSolution(config.cubeSize, pieceLookup, zDivider, solutionData))
        }

        return convertedSolutions
    }

    fun search(column: Int = 0, maxDepth: Int = 0){
        if ((maxDepth != 0 && column >= maxDepth) || headerNode.right == headerNode) {
            solutions.add(partialSolution.slice(0 until column).toMutableList())
            return
        }

        var currentNode = getMinColumn()

        cover(currentNode)

        var node = currentNode.down!!
        while (node != currentNode) {
            if (partialSolution.size <= column)
            {
                partialSolution.add(node)
            }
            else
            {
                partialSolution[column] = node
            }

            var tempNode = node.right!!
            while (tempNode != node) {
                cover(tempNode.column!!)
                tempNode = tempNode.right!!
            }
            search(column + 1, maxDepth)
            node = partialSolution[column]
            currentNode = node.column!!

            tempNode = node.left!!
            while (tempNode != node) {
                uncover(tempNode.column!!)
                tempNode = tempNode.left!!
            }

            node = node.down!!
        }

        uncover(currentNode)
    }

    fun applyPartialCover(coverNodes: List<CoverNode>){
        val columns: MutableList<Int> = mutableListOf()
        coverNodes.forEach { coverNode ->
            var tempNode = coverNode
            while(!columns.contains(tempNode.column!!.columnId)) {
                columns.add(tempNode.column!!.columnId)
                tempNode = tempNode.right!!
            }
        }

        columns.forEach { column ->
            var node = headerNode.right!!
            while (node != headerNode) {
                if (node.column!!.columnId == column)
                {
                    cover(node)
                    break
                }
                node = node.right!!
            }
        }

    }

    private fun cover(coverNode: CoverNode) {
        coverNode.right!!.left = coverNode.left
        coverNode.left!!.right = coverNode.right

        var node = coverNode.down!!
        while (node != coverNode) {
            var tempNode = node.right!!
            while (tempNode != node) {
                tempNode.down!!.up = tempNode.up
                tempNode.up!!.down = tempNode.down
                tempNode.column!!.size--
                tempNode = tempNode.right!!
            }
            node = node.down!!
        }
    }

    private fun uncover(coverNode: CoverNode) {
        var node = coverNode.up!!
        while (node != coverNode) {
            var tempNode = node.left!!
            while (tempNode != node) {
                tempNode.column!!.size++
                tempNode.down!!.up = tempNode
                tempNode.up!!.down = tempNode

                tempNode = tempNode.left!!
            }
            node = node.up!!
        }

        coverNode.right!!.left = coverNode
        coverNode.left!!.right = coverNode
    }
    private fun getMinColumn(): CoverNode {
        var node = headerNode.right!!
        var minNode = node

        var minSize = node.size
        while (node != headerNode) {
            if (node.size < minSize) {
                minNode = node
                minSize = node.size
            }
            node = node.right!!
        }
        return minNode
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