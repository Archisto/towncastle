using UnityEngine;

public class BuildInstruction
{
    public HexMeshScriptableObject HexMesh { get; private set; }

    public Vector2Int Cell { get; private set; }

    public Utils.HexDirection Direction { get; private set; }

    public BuildInstruction(HexMeshScriptableObject hexMesh,
                            Vector2Int cell,
                            Utils.HexDirection direction)
    {
        HexMesh = hexMesh;
        Cell = cell;
        Direction = direction;
    }

    public string GetInfo()
    {
        return string.Format("Object {0} at {1} looking {2}",
            HexMesh, Cell, Direction);
    }
}
