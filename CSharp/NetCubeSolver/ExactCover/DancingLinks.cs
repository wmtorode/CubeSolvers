using System.Collections;

namespace NetCubeSolver.ExactCover;

public class DancingLinks
{
    private List<CoverNode> CoverNodes = new List<CoverNode>();

    private CoverNode HeaderNode;
    
    public List<List<CoverNode>> Solutions = new List<List<CoverNode>>();
    
    private List<CoverNode> PartialSolution = new List<CoverNode>();

    private int matrixSize;

    public DancingLinks(List<int> matrixColumnNames, List<BitArray> coverMatrix)
    {
        matrixSize = matrixColumnNames.Count;
        InitializeCoverNodes( matrixColumnNames,  coverMatrix);
    }

    public void Clear()
    {
        CoverNodes.Clear();
        Solutions.Clear();
        PartialSolution.Clear();
        HeaderNode = null;
    }
    
    public List<PuzzleSolution> ConvertedSolutions(Config config, List<CoverNode>? partialSolution = null)
    {
        
        List<int> partialData = new List<int>();
        if (partialSolution != null)
        {
            foreach (var node in partialSolution)
            {
                var tempNode = node;
                do
                {
                    partialData.Add(tempNode.Column.ColumnId);
                    tempNode = tempNode.Right;
                }while(tempNode != node);
                
                partialData.Add(-1);
            }
        }

        var size = matrixSize + config.PuzzlePieces.Count;
        var zDivider = config.CubeSize * config.CubeSize;
        var convertedSolutions = new List<PuzzleSolution>(Solutions.Count);
        
        List<int> data = new List<int>(size);
        
        foreach (var solution in Solutions)
        {
            
            foreach (var node in solution)
            {
                var tempNode = node;
                do
                {
                    data.Add(tempNode.Column.ColumnId);
                    tempNode = tempNode.Right;
                } while (tempNode != node);

                data.Add(-1);
            }

            // combine the partial cover with the data here to create a complete solution
            if (partialData.Count != 0)
            {
                data.AddRange(partialData);
            }
            convertedSolutions.Add(PuzzleSolution.ConvertToSolution(config.CubeSize, config.PuzzlePieceSymbolLut, zDivider, data));
            data.Clear();
        }

        return convertedSolutions;
    }

    public void Search(int column = 0, int maxDepth = 0)
    {
        if ((maxDepth != 0 && column >= maxDepth) || HeaderNode.Right == HeaderNode)
        {
            Solutions.Add(PartialSolution.Slice(0, column));
            return;
        }

        var currentNode = GetMinColumn();
        
        Cover(ref currentNode);

        var node = currentNode.Down;
        while (node != currentNode)
        {
            if (PartialSolution.Count <= column)
            {
                PartialSolution.Add(node);
            }
            else
            {
                PartialSolution[column] = node;
            }

            var tempNode = node.Right;
            while (tempNode != node)
            {
                Cover(ref tempNode.Column);
                tempNode = tempNode.Right;
            }
            
            Search(column + 1, maxDepth);
            
            node = PartialSolution[column];
            currentNode = node.Column;
            
            tempNode = node.Left;
            while (tempNode != node)
            {
                Uncover(ref tempNode.Column);
                tempNode = tempNode.Left;
            }
            
            node = node.Down;
        }
        
        Uncover(ref currentNode);

    }

    public void ApplyPartialCover(List<CoverNode> coverNodes)
    {
        List<int> columns = new List<int>();
        foreach (var node in coverNodes)
        {
            var tempNode = node;
           while(!columns.Contains(tempNode.Column.ColumnId))
           {
               columns.Add(tempNode.Column.ColumnId);
               tempNode = tempNode.Right;
           }
        }

        foreach (var column in columns)
        {
            var node = HeaderNode.Right;
            while (node != HeaderNode)
            {
                if (node.Column.ColumnId == column)
                {
                    Cover(ref node);
                    break;
                }
                node = node.Right;
            }
        }
    }

    private void Cover(ref CoverNode coverNode)
    {
        coverNode.Right.Left = coverNode.Left;
        coverNode.Left.Right = coverNode.Right;

        var node = coverNode.Down;
        while (node != coverNode)
        {
            var tempNode = node.Right;
            while (tempNode != node)
            {
                tempNode.Down.Up = tempNode.Up;
                tempNode.Up.Down = tempNode.Down;
                tempNode.Column.Size--;

                tempNode = tempNode.Right;
            }
            node = node.Down;
        }
    }

    private void Uncover(ref CoverNode coverNode)
    {
        var node = coverNode.Up;
        while (node != coverNode)
        {
            var tempNode = node.Left;
            while (tempNode != node)
            {
                tempNode.Column.Size++;
                tempNode.Down.Up = tempNode;
                tempNode.Up.Down = tempNode;

                tempNode = tempNode.Left;
            }
            node = node.Up;
        }

        coverNode.Right.Left = coverNode;
        coverNode.Left.Right = coverNode;
    }

    private CoverNode GetMinColumn()
    {
        var node = HeaderNode.Right;
        var minNode = node;
        
        int minSize = node.Size;
        while (node != HeaderNode)
        {
            var nodeSize = node.Size;
            if (nodeSize < minSize)
            {
                minNode = node;
                minSize = nodeSize;
            }
            
            node = node.Right;
        }
        
        return minNode;
    }
    
    private void InitializeCoverNodes(List<int> matrixColumnNames, List<BitArray> coverMatrix)
    {
        CoverNodes.Clear();
        HeaderNode = new CoverNode(-1);
        HeaderNode.Left = HeaderNode;
        HeaderNode.Right = HeaderNode;
        
        CoverNode PreviousNode = HeaderNode;
        for (int i = 0; i < matrixColumnNames.Count; i++)
        {
            var columnNode = new CoverNode(i);
            CoverNodes.Add(columnNode);
            columnNode.Left = PreviousNode;
            PreviousNode.Right = columnNode;
            HeaderNode.Left = columnNode;
            columnNode.Right = HeaderNode;

            columnNode.Up = columnNode;
            columnNode.Down = columnNode;
            columnNode.Column = columnNode;
            
            PreviousNode = columnNode;
        }

        for(int i = 0; i < coverMatrix.Count; i++)
        {
            AddRowToCoverNodes(i, coverMatrix);
        }
    }

    private void AddRowToCoverNodes(int row, List<BitArray> coverMatrix)
    {
        CoverNode PreviousNode = null;
        var header = HeaderNode;
        for (int i = 0; i < coverMatrix[row].Count; i++)
        {
            header = header.Right;
            if (coverMatrix[row][i])
            {
                var rowNode = new CoverNode();
                CoverNodes.Add(rowNode);

                rowNode.Column = header;
                rowNode.Column.Size++;

                header.Up.Down = rowNode;
                rowNode.Down = header;
                rowNode.Up = header.Up;
                header.Up = rowNode;

                if (PreviousNode == null)
                {
                    rowNode.Left = rowNode;
                    rowNode.Right = rowNode;
                }
                else
                {
                    rowNode.Left = PreviousNode;
                    rowNode.Right = PreviousNode.Right;
                    PreviousNode.Right.Left = rowNode;
                    PreviousNode.Right = rowNode;
                }

                PreviousNode = rowNode;
            }
        }
    }
}