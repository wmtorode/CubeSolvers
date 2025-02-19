using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using CubeSolver.Utils;
using NetCubeSolver;

namespace CubeSolver.Tools;

public class DemoConfigWriter: ITool
{
    public string Name { get; } = "Demo Config Create";
    public bool Run()
    {
        var config = new Config();
        config.Id = "Demo";
        config.DisplayName = "Demo Config";
        config.OutputFileName = "DemoSolution.json";
        
        var piece = new PuzzlePiece();
        piece.Id = 0;
        piece.Description = "Demo Piece";
        piece.Symbol = "0";
        
        var subpiece = new SubPiece();
        piece.Components.Add(subpiece);
        config.PuzzlePieces.Add(piece);
        
        var writeOptions = new JsonSerializerOptions { WriteIndented = true };
        var filePath = Path.Combine(ProgramUtils.WorkingDirectory, "DemoConfig.json");
        File.WriteAllText(filePath, JsonSerializer.Serialize(config, writeOptions));
        
        Console.WriteLine("Wrote DemoConfig.json!");
        
        return true;
    }
}