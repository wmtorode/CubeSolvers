using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;

namespace NetCubeSolver;

public class PuzzlePiece
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public string InfoName => $"{Id}: {Description}";
    public List<SubPiece> Components { get; set; } = new();

    [JsonIgnore]
    public int CurrentOrientation = 0;
    
    [JsonIgnore]
    public int MaxOrientation = 0;
    
    [JsonIgnore]
    public Dictionary<int, List<SubPiece>> Orientations = new();
    

    public bool Rotate()
    {
        CurrentOrientation++;
        if (CurrentOrientation >= MaxOrientation)
        {
            CurrentOrientation = MaxOrientation - 1;
            return false;
        }
        return true;
    }
    
    [JsonIgnore]
    public List<SubPiece> Geometry => Orientations[CurrentOrientation];

    public void SnapToValidInsert(int maxPoint)
    {
        int maxX, maxY, maxZ;
        int minX, minY, minZ;
        
        maxX = maxY = maxZ = 0;
        minX = minY = minZ = Int32.MaxValue;

        foreach (var component in Geometry)
        {
            maxX = Math.Max(maxX, component.X);
            maxY = Math.Max(maxY, component.Y);
            maxZ = Math.Max(maxZ, component.Z);
            
            minX = Math.Min(minX, component.X);
            minY = Math.Min(minY, component.Y);
            minZ = Math.Min(minZ, component.Z);
        }

        int x, y, z;
        x = y = z = 0;

        if (minX < 0)
        {
            x = -minX;
        }

        if (maxX > maxPoint)
        {
            x = maxPoint - maxX;
        }

        if (minY < 0)
        {
            y = -minY;
        }

        if (maxY > maxPoint)
        {
            y = maxPoint - maxY;
        }

        if (minZ < 0)
        {
            z = -minZ;
        }

        if (maxZ > maxPoint)
        {
            z = maxPoint - maxZ;
        }

        var transform = new SubPiece(x, y, z);
        Transform(transform);
    }
    
    public void Transform(SubPiece point)
    {
        var transformation = FindTransformation(point);
        foreach (var component in Geometry)
        {
            component.Transform(transformation);
        }
        
    }

    public void Initialize()
    {
        GenerateOrientations();
    }

    private void GenerateOrientations()
    {
        Orientations.Clear();
        CurrentOrientation = 0;
        // We may need to reset the subpeices here if this ever gets called outside of initialize
        Normalize();
        Orientations.Add(CurrentOrientation, Components.Select(x => x.Clone()).ToList());
        for (var i = 0; i < 23; i++)
        {
            RotateComponents();
            Normalize();
            var unique = true;
            foreach (var orientation in Orientations)
            {
                bool found = true;
                foreach (var component in Components)
                {
                    found &= orientation.Value.Contains(component);
                }

                if (found)
                {
                    unique = false;
                    break;
                }
            }

            if (unique)
            {
                Orientations.Add(Orientations.Count, Components.Select(x => x.Clone()).ToList());
            }
        }

        CurrentOrientation = 0;
        MaxOrientation = Orientations.Count;

    }

    private void RotateComponents()
    {
        var newOrientation = CurrentOrientation +1;
        RotateComponents(Axis.X);
        switch (newOrientation)
        {
            case 4:
            case 8:
            case 12:
                RotateComponents(Axis.Y);
                break;
            case 16:
                RotateComponents(Axis.Y);
                RotateComponents(Axis.Z);
                break;
            case 20:
                RotateComponents(Axis.Z);
                RotateComponents(Axis.Z);
                break;
        }
        CurrentOrientation = newOrientation;
    }

    private void RotateComponents(Axis axis)
    {
        foreach (var component in Components)
        {
            component.Rotate(axis);
        }
    }

    private void Normalize()
    {
        var transformation = FindTransformationForPiece(new SubPiece(0, 0, 0));
        foreach (var component in Components)
        {
            component.Transform(transformation);
        }
    }

    private SubPiece FindTransformationForPiece(SubPiece subPiece)
    {
        var index = FindLowestSubPiece();
        int x, y, z;
        x = subPiece.X - Components[index].X;
        y = subPiece.Y - Components[index].Y;
        z = subPiece.Z - Components[index].Z;
        
        return new SubPiece(x, y, z);
    }

    private int FindLowestSubPiece()
    {
        int lowestSubPiece = 0;
        for (int i=1; i < Components.Count; i++)
        {
            if (Components[i] < Components[lowestSubPiece])
            {
                lowestSubPiece = i;
            }
        }
        
        return lowestSubPiece;
    }

    private SubPiece FindTransformation(SubPiece subPiece)
    {
        var index = FindOrientationBase();
        int x, y, z;
        x = subPiece.X - Orientations[CurrentOrientation][index].X;
        y = subPiece.Y - Orientations[CurrentOrientation][index].Y;
        z = subPiece.Z - Orientations[CurrentOrientation][index].Z;
        
        return new SubPiece(x, y, z);
    }
    
    private int FindOrientationBase()
    {
        int lowestSubPiece = 0;
        for (int i=1; i < Orientations[CurrentOrientation].Count; i++)
        {
            if (Orientations[CurrentOrientation][i] < Orientations[CurrentOrientation][lowestSubPiece])
            {
                lowestSubPiece = i;
            }
        }
        
        return lowestSubPiece;
    }
}