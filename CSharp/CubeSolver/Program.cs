// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using CubeSolver.Tools;
using CubeSolver.Utils;

internal class Program
{

    private static ITool[] _tools =
    {
        new SetWorkingDir(),
        new DemoConfigWriter(),
        new ConfigVisualizer(),
        new PieceInfo(),
        new CoverMatrixVisualizer(),
        new CubeSolver.Tools.CubeSolver(),
        new ThreadedCubeSolver(),
        new BenchmarkRunner()
    };

    private static void Main(string[] args)
    {
        ProgramUtils.WriteFilledLine('#');
        ProgramUtils.WriteFilledLine(' ', "Select Run Mode");
        ProgramUtils.WriteFilledLine('#');
        ProgramUtils.Write();
        
        ProgramUtils.SetupWorkingDirectory(Directory.GetCurrentDirectory());

        if (args.Length > 0)
        {
            ProgramUtils.SetupWorkingDirectory(args[0]);
        }
        
        if (args.Length > 1)
        {
            ProgramUtils.OutputDirectory = args[1];
        }
        
        Console.WriteLine($"Config Dir: {ProgramUtils.WorkingDirectory}");
        Console.WriteLine($"Output Dir: {ProgramUtils.OutputDirectory}");
        
        
        var toolNames = _tools.Select(x => x.Name).ToArray();

        while (true)
        {
            var selectedTool = ProgramUtils.GetUserSelection("Options", toolNames);
            var tool = _tools[selectedTool];
            
            ProgramUtils.Indent();
            tool.Run();
            ProgramUtils.WriteFilledLine('=');
            ProgramUtils.Unindent();
            ProgramUtils.Write();
        }
        
    }
    
    
}