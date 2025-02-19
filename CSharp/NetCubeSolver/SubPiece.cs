using System.Text.Json.Serialization;

namespace NetCubeSolver;

public class SubPiece: IComparable<SubPiece>, IEquatable<SubPiece>
{
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    

    public SubPiece()
    { 
        X = Y = Z = 0;
    }

    public SubPiece Clone()
    {
        var clone = new SubPiece();
        clone.X = X;
        clone.Y = Y;
        clone.Z = Z;
        return clone;
    }
    
    public SubPiece(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public void Rotate(Axis axis)
    {
        int temp;
        switch (axis)
        {
            case Axis.X:
                temp = Y;
                Y = Z;
                Z = -temp;
                break;
            case Axis.Y:
                temp = Z;
                Z = X;
                X = -temp;
                break;
            case Axis.Z:
                temp = X;
                X = Y;
                Y = -temp;
                break;
        }
    }

    public void Transform(SubPiece subPiece)
    {
        X += subPiece.X;
        Y += subPiece.Y;
        Z += subPiece.Z;
    }

    public int CompareTo(SubPiece? other)
    {
        if (Z < other.Z) return -1;
        if (Z > other.Z) return 1;
        
        if (Y < other.Y) return -1;
        if (Y > other.Y) return 1;
        
        if (X < other.X) return -1;
        if (X > other.X) return 1;

        return 0;
    }

    public static bool operator <(SubPiece a, SubPiece b)
    {
        return a.CompareTo(b) < 0;
    }
    
    public static bool operator >(SubPiece a, SubPiece b)
    {
        return a.CompareTo(b) > 0;
    }
        
    public static bool operator <=(SubPiece a, SubPiece b)
    {
        return a.CompareTo(b) <= 0;
    }

    public static bool operator >=(SubPiece a, SubPiece b)
    {
        return a.CompareTo(b) >= 0;
    }

    public bool Equals(SubPiece? other)
    {
        return CompareTo(other) == 0;
    }
}