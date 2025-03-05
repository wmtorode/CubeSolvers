using CubeSolver.Utils;
using NetCubeSolver;

namespace CubeSolver.Tools;

public class ConfigVisualizer: ITool
{
    public string Name { get; } = "Config Visualizer";
    public bool Run()
    {
        var config = ProgramUtils.GetConfigFromUser();

        var solution = new PuzzleSolution(config.CubeSize, config.PuzzlePieceSymbolLut);

        foreach (var piece in config.PuzzlePieces)
        {
            piece.SnapToValidInsert(config.CubeSize);
            solution.InsertPiece(piece);
            Console.Write(solution.RenderToString($"Piece {piece.Id}: {piece.Description} has {piece.MaxOrientation} unique orientation(s)"));
            solution.RemovePiece(piece.Id);
        }
        
        
        return true;

    }
}