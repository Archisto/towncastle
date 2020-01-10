using UnityEngine;

[CreateAssetMenu(fileName = "Hex Mesh", menuName = "Scriptable Objects/Hex Mesh", order = 1)]
public class HexMeshScriptableObject : ScriptableObject
{
    public Mesh mesh;
    public bool imported = true;
    public HexObject.StructureType structureType = HexObject.StructureType.Object;
    public float defaultRotationY = 90;
}