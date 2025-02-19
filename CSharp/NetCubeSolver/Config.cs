namespace NetCubeSolver;

public class Config
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string OutputFileName { get; set; }
    public int CubeSize { get; set; }
    public int UpdateInterval { get; set; }
    public bool Verbose { get; set; }
    public List<PuzzlePiece> PuzzlePieces { get; set; } = new();


    public void InitializePieces()
    {
        foreach (var piece in PuzzlePieces)
        {
            piece.Initialize();
        }
    }
    
    public Dictionary<int, string> GetPuzzlePieceSymbolLut()
    {
        Dictionary<int, string> puzzlePieceLut = new();
        foreach (var piece in PuzzlePieces)
        {
            puzzlePieceLut.Add(piece.Id, piece.Symbol);
        }

        puzzlePieceLut.Add(-1, "-");

        
        return puzzlePieceLut;
    }
}