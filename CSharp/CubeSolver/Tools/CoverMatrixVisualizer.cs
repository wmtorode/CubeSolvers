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

        Solution = new PuzzleSolution(CubeConfig.CubeSize, CubeConfig.GetPuzzlePieceSymbolLut());

        var watch = System.Diagnostics.Stopwatch.StartNew();
        GenerateCoverMatrix();
        watch.Stop();
        
        ProgramUtils.WriteFilledLine('-', $"Cover Matrix Generation took {watch.ElapsedMilliseconds} ms");
        Console.WriteLine();
        
        var valueSize = MatrixColumnNames.Count;
        
        PrintValues(MatrixColumnNames, valueSize);
        foreach (var row in CoverMatrix)
        {
            PrintValues(row, valueSize);
        }
        
        Console.WriteLine();
        ProgramUtils.WriteFilledLine('-');
        
        ProgramUtils.Write($"Matrix Columns:{valueSize}");
        ProgramUtils.Write($"Matrix Rows:{CoverMatrix.Count}");
        
        return true;
    }
    
    private void PrintValues( IEnumerable myList, int myWidth )  {
        int i = myWidth;
        foreach ( Object obj in myList ) {
            if ( i <= 0 )  {
                i = myWidth;
                Console.WriteLine();
            }
            i--;
            Console.Write( "{0,8}", obj );
        }
        Console.WriteLine();
    }
}