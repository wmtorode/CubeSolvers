using System.Text.Json.Serialization;

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
    
    [JsonIgnore]
    public Dictionary<int, string> PuzzlePieceSymbolLut { get; set; } = new();


    public void InitializePieces()
    {
        PuzzlePieceSymbolLut.Clear();
        foreach (var piece in PuzzlePieces)
        {
            piece.Initialize();
            PuzzlePieceSymbolLut.Add(piece.Id, piece.Symbol);
        }
        PuzzlePieceSymbolLut.Add(-1, "-");
    }
}