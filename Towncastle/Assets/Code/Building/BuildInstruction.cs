using UnityEngine;

public class BuildInstruction
{
    public HexMeshScriptableObject HexMesh { get; set; }

    public Vector2Int Cell { get; set; }

    public Utils.HexDirection Direction { get; set; }

    public float HeightLevel { get; set; }

    public BuildInstruction(HexMeshScriptableObject hexMesh,
                            Vector2Int cell,
                            Utils.HexDirection direction,
                            float heightLevel = 1)
    {
        HexMesh = hexMesh;
        Cell = cell;
        Direction = direction;
        HeightLevel = heightLevel;
    }

    public static BuildInstruction Default()
    {
        // TODO: HexMesh shouldn't be null
        return new BuildInstruction(null, Vector2Int.zero, Utils.HexDirection.Right);
    }

    public override string ToString()
    {
        return string.Format("HexMesh {0} at {1} (heightLevel: {2}) looking {3}",
            HexMesh.name, Cell, HeightLevel, Direction);
    }
}
