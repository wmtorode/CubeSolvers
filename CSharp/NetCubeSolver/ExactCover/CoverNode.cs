namespace NetCubeSolver.ExactCover;

public class CoverNode
{

    public int ColumnId;
    public int Size;
    public bool IsColumn;

    public CoverNode Left;
    public CoverNode Right;
    public CoverNode Up;
    public CoverNode Down;
    public CoverNode Column;
    
    public CoverNode()
    {
        IsColumn = false;
        ColumnId = -1;
        Size = 0;
    }

    public CoverNode(int columnId)
    {
        ColumnId = columnId;
        IsColumn = true;
        Size = 0;
    }
}