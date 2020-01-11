using UnityEngine;

[CreateAssetMenu(fileName = "Hex Mesh", menuName = "Scriptable Objects/Hex Mesh", order = 1)]
public class HexMeshScriptableObject : ScriptableObject
{
    [Tooltip("The 3D model.")]
    public Mesh mesh;

    [Tooltip("Determines whether the up axis is Y (false) or Z (true).")]
    public bool imported = true;

    public HexObject.StructureType structureType = HexObject.StructureType.Object;
    public Utils.HexDirection mainDirection = Utils.HexDirection.Right;
    public float defaultRotationY = 90;
    public float defaultPositionY = 0;
}