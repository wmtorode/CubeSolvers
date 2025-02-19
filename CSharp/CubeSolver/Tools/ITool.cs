namespace CubeSolver.Tools;

public interface ITool
{
    string Name { get; }
    bool Run();
}