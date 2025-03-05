using System.Collections;
using System.ComponentModel.Design;
using CubeSolver.Utils;
using NetCubeSolver;

namespace CubeSolver.Tools;

public class CoverMatrixVisualizer: BaseSolver, ITool
{
    public string Name { get; } = "Cover Matrix Visualizer";

    public bool Run()
    {
        CubeConfig = ProgramUtils.GetConfigFromUser();

        Solution = new PuzzleSolution(CubeConfig.CubeSize, CubeConfig.PuzzlePieceSymbolLut);

        var watch = System.Diagnostics.Stopwatch.StartNew();
        GenerateCoverMatrix();
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Cover Matrix Generation took {watch.ElapsedMilliseconds} ms");
        Console.WriteLine();
        
        PrintValues(MatrixColumnNames);
        foreach (var row in CoverMatrix)
        {
            PrintValues(row);
        }
        
        Console.WriteLine();
        ProgramUtils.WriteFilledLine('-');
        
        ProgramUtils.Write($"Matrix Columns:{MatrixColumnNames.Count}");
        ProgramUtils.Write($"Matrix Rows:{CoverMatrix.Count}");
        
        return true;
    }
    
    private void PrintValues( IEnumerable myList)  {
        foreach ( Object obj in myList ) {
            Console.Write( "{0,8}", obj );
        }
        Console.WriteLine();
    }
}