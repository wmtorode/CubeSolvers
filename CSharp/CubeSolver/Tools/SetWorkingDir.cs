using CubeSolver.Utils;

namespace CubeSolver.Tools;

public class SetWorkingDir: ITool
{
    public string Name { get; } = "Set Working Dirs";
    public bool Run()
    {
        var newWorkingDir = ProgramUtils.GetUserInput("New Config Directory");
        ProgramUtils.SetupWorkingDirectory(newWorkingDir);
        
        var newOutoutDir = ProgramUtils.GetUserInput("New Output Directory");
        ProgramUtils.OutputDirectory = newOutoutDir;
        
        return true;
    }
}