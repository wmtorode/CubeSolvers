using System.Text;
using NetCubeSolver.ExactCover;

namespace NetCubeSolver;

public class PuzzleSolution
{
    
    private readonly int _size;
    private readonly Dictionary<int, string> _puzzlePieceSymbols;
    
    private int [,,] _cubes;

    public PuzzleSolution(int size, Dictionary<int, string> puzzlePieceSymbols)
    {
        _size = size;
        _puzzlePieceSymbols = puzzlePieceSymbols;
        InitEmptyCubes();
    }

    public List<int> GetPositionHeaders()
    {
        return Enumerable.Range(0, _size*_size*_size).ToList();
    }

    public static PuzzleSolution ConvertToSolution(int size, Dictionary<int, string> puzzlePieceSymbols, int zDiv, List<int> solutionData)
    {
        var solution = new PuzzleSolution(size, puzzlePieceSymbols);
        List<int> pieceData = new List<int>();
        int currentPieceId = -1;
        int pieceCount = puzzlePieceSymbols.Count - 1;
        foreach (var point in solutionData)
        {
            if (point != -1)
            {
                if (puzzlePieceSymbols.ContainsKey(point))
                {
                    currentPieceId = point;
                }
                else
                {
                    pieceData.Add(point - pieceCount);
                }
            }
            else
            {
                solution.FillFromPieceData(currentPieceId, pieceData, zDiv);
                pieceData.Clear();
            }
        }
        
        return solution;
    }

    private void FillFromPieceData(int pieceId, List<int> pieceData, int zDiv)
    {
        foreach (var point in pieceData)
        {
            int x = point;
            int z = Math.DivRem(x, zDiv, out x);
            int y = Math.DivRem(x, _size, out x);
            
            _cubes[z, y, x] = pieceId;
            
        }
    }

    // The canonical solution is the one with the lowest string representation
    public string FindAndSetToCanonicalSolution()
    {
        int orientation = 0;
        Dictionary<string, int[,,]> orientations = new Dictionary<string, int[,,]>();
        StringBuilder sb = new StringBuilder(_size * _size * _size);
        
        orientations.Add(GetCanonicalString(_cubes, sb), _cubes);
        sb.Clear();
        
        var cubes = _cubes;
        for (int i = 0; i < 23; i++)
        {
            cubes = Rotate(i, cubes);
            orientations.Add(GetCanonicalString(cubes, sb), cubes);
            sb.Clear();
        }
        
        var keys = orientations.Keys.ToArray();
        Array.Sort(keys);
        _cubes = orientations[keys[0]];
        return keys[0];
        
    }

    private string GetCanonicalString(int[,,] cubes, StringBuilder sb)
    {
        for (int z = 0; z < _size; z++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    sb.Append(_puzzlePieceSymbols[cubes[z,y,x]]);
                }
            }
        }
        return sb.ToString();
    }

    public string RenderToString(string title)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(title);
        sb.AppendLine();
        for (int y = 0; y < _size; y++)
        {
            string line = "\t";
            for (int z = 0; z < _size; z++)
            {
                for (int x = 0; x < _size; x++)
                {
                    line += $"{_puzzlePieceSymbols[_cubes[z,y,x]]} ";
                }

                line += "  ";
            }
            sb.AppendLine(line);
        }
        sb.AppendLine();
        return sb.ToString();
    }
    
    public void WriteToTextFile(string title, StreamWriter writer)
    {
        writer.WriteLine(title);
        writer.WriteLine();
        for (int y = 0; y < _size; y++)
        {
            writer.Write("\t");
            for (int z = 0; z < _size; z++)
            {
                for (int x = 0; x < _size; x++)
                {
                    writer.Write($"{_puzzlePieceSymbols[_cubes[z,y,x]]} ");
                }

                writer.Write("  ");
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    public void Rotate(Axis axis)
    {
        _cubes = Rotate(axis, _cubes);
    }

    public bool RemovePiece(int pieceId)
    {
        bool ret = false;
        for (int z = 0; z < _size; z++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (_cubes[z,y,x] == pieceId)
                    {
                        _cubes[z,y,x] = -1;
                        ret = true;
                    }
                }
            }
        }

        return ret;
    }

    private bool DoInsert(PuzzlePiece piece, bool permanent)
    {
        bool ret = true;
        foreach (var cube in piece.Geometry)
        {
            if (_cubes[cube.Z, cube.Y, cube.X] == -1)
            {
                if (permanent)
                {
                    _cubes[cube.Z, cube.Y, cube.X] = piece.Id;
                }
            }
            else
            {
                ret = false;
            }
        }

        if (!ret && permanent) RemovePiece(piece.Id);
        
        return ret;
    }

    public bool InsertPiece(PuzzlePiece piece)
    {
        bool ret = DoInsert(piece, true);
        if (!ret) return false;
        
        return ret;
    }
    
    private int[,,] CreateEmptyCubes()
    {
        var cubes = new int[_size,_size,_size];
        for (int z = 0; z < _size; z++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    cubes[z,y,x] = -1;
                }
            }
        }
        return cubes;
    }

    private void InitEmptyCubes()
    {
        _cubes = CreateEmptyCubes();
    }
    
    private int[,,] Rotate(Axis axis, int[,,] currentCubes)
    {
        var cubes = new int[_size,_size,_size];

        for (int z = 0; z < _size; z++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    switch (axis)
                    {
                        case Axis.X:
                            cubes[(_size - 1) - y,z,x] = currentCubes[z,y,x];
                            break;
                        case Axis.Y:
                            cubes[x,y,(_size - 1)- z] = currentCubes[z,y,x];
                            break;
                        case Axis.Z:
                            cubes[z, (_size - 1) - x, y] = currentCubes[z,y,x];
                            break;
                    }
                }
            }
        }
        return cubes;
    }
    
    private int[,,] Rotate(int currentOrientation, int[,,] currentCubes)
    {
        var cubes = currentCubes;
        var newOrientation = currentOrientation +1;
        cubes = Rotate(Axis.X, cubes);
        switch (newOrientation)
        {
            case 4:
            case 8:
            case 12:
                cubes = Rotate(Axis.Y, cubes);
                break;
            case 16:
                cubes = Rotate(Axis.Y, cubes);
                cubes = Rotate(Axis.Z, cubes);
                break;
            case 20:
                cubes = Rotate(Axis.Z, cubes);
                cubes = Rotate(Axis.Z, cubes);
                break;
        }

        return cubes;
    }
    
}