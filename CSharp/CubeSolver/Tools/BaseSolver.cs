using System.Collections;
using CubeSolver.Utils;
using NetCubeSolver;
using NetCubeSolver.ExactCover;

namespace CubeSolver.Tools;

public class BaseSolver
{
    protected List<int> MatrixColumnNames = new List<int>();
    protected List<BitArray> CoverMatrix = new List<BitArray>();

    protected Config CubeConfig;
    
    protected PuzzleSolution Solution;
    
    protected List<CoverNode> CoverNodes = new List<CoverNode>();

    protected CoverNode HeaderNode;

    protected void PrintPieceData()
    {
        foreach (var piece in CubeConfig.PuzzlePieces)
        {
            ProgramUtils.Write(GetPieceInfo(piece));
        }
    }
    
    protected string GetPieceInfo(PuzzlePiece piece)
    {
        return $"Piece {piece.Id}, using Symbol: {piece.Symbol}, has {piece.Components.Count} Cube(s) and {piece.MaxOrientation} Unique Orientation(s)";
    }
    
    protected void GenerateCoverMatrix()
    {
        MatrixColumnNames.Clear();
        CoverMatrix.Clear();

        foreach (var piece in CubeConfig.PuzzlePieces)
        {
            MatrixColumnNames.Add(piece.Id);
        }
        
        MatrixColumnNames.AddRange(Solution.GetPositionHeaders());

        for (int i = 0; i < CubeConfig.PuzzlePieces.Count; i++)
        {
            for (int j = 0; j < CubeConfig.PuzzlePieces[i].MaxOrientation; j++)
            {
                for (int z = 0; z < CubeConfig.CubeSize; z++)
                {
                    for (int y = 0; y < CubeConfig.CubeSize; y++)
                    {
                        for (int x = 0; x < CubeConfig.CubeSize; x++)
                        {
                            var row = GetMatrixRow(i, j, x, y, z);
                            if (row != null)
                            {
                                CoverMatrix.Add(row);
                            }
                        }
                    }
                }
            }
        }
    }

    protected void WriteToTextFile(Dictionary<string, PuzzleSolution> solutions, string[] keys)
    {
        using (var streamWriter =
               new StreamWriter(Path.Combine(ProgramUtils.OutputDirectory, $"{CubeConfig.OutputFileName}.txt")))
        {
            foreach (var piece in CubeConfig.PuzzlePieces)
            {
                streamWriter.WriteLine(GetPieceInfo(piece)); 
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine();
            
            for (int i = 0; i < keys.Length; i++)
            {
                solutions[keys[i]].WriteToTextFile($"Solution {i}", streamWriter);
                streamWriter.WriteLine();
            }
            
        }
    }

    protected DancingLinks CreateDancingLinks()
    {
        return new DancingLinks(MatrixColumnNames, CoverMatrix);
    }

    private BitArray? GetMatrixRow(int pieceId, int orientation, int x, int y, int z)
    {
        var row = new BitArray(MatrixColumnNames.Count, false);
        row[pieceId] = true;
        var piece = CubeConfig.PuzzlePieces[pieceId];
        piece.CurrentOrientation = orientation;
        var transform = new SubPiece(x, y, z);
        piece.Transform(transform);
        foreach (var component in piece.Geometry)
        {
            var index = FindColumn(component);
            if (index == -1)
            {
                return null;
            }
            row[index] = true;
        }
        return row;
    }

    private int FindColumn(SubPiece subPiece)
    {
        if (subPiece.X < 0 || subPiece.X >= CubeConfig.CubeSize || subPiece.Y < 0 || subPiece.Y >= CubeConfig.CubeSize || subPiece.Z < 0 || subPiece.Z >= CubeConfig.CubeSize) return -1;
        
        return CubeConfig.PuzzlePieces.Count + subPiece.X + (subPiece.Y * CubeConfig.CubeSize) + (subPiece.Z * CubeConfig.CubeSize * CubeConfig.CubeSize); 
    }
}