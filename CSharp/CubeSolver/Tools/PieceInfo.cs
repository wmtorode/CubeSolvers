using CubeSolver.Utils;
using NetCubeSolver;

namespace CubeSolver.Tools;

public class PieceInfo: ITool
{
    public string Name { get; } = "Piece Info";
    public bool Run()
    {
        var config = ProgramUtils.GetConfigFromUser();

        var solution = new PuzzleSolution(config.CubeSize, config.GetPuzzlePieceSymbolLut());

        var pieceNames = config.PuzzlePieces.Select(x => x.InfoName).ToArray();

        var pieceSelection = ProgramUtils.GetUserSelection("Select Piece", pieceNames);
        
        var piece = config.PuzzlePieces[pieceSelection];
        
        Console.WriteLine($" Piece: {piece.Id}");
        Console.WriteLine($" Name: {piece.Description}");
        Console.WriteLine($" Symbol: {piece.Symbol}");
        Console.WriteLine($" Unique Orientation(s): {piece.MaxOrientation}");
        Console.WriteLine();
        
        piece.SnapToValidInsert(config.CubeSize);
        solution.InsertPiece(piece);
        Console.Write(solution.RenderToString($"Orientation: {piece.CurrentOrientation}"));
        solution.RemovePiece(piece.Id);

        while (piece.Rotate())
        {
            piece.SnapToValidInsert(config.CubeSize);
            solution.InsertPiece(piece);
            Console.Write(solution.RenderToString($"Orientation: {piece.CurrentOrientation}"));
            solution.RemovePiece(piece.Id); 
        }

        return true;
    }
}