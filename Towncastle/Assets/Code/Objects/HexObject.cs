using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexObject : LevelObject, IGridObject
{
    public enum StructureType
    {
        Floor,
        Wall,
        Room,
        Roof,
        Canopy,
        Protrusion,
        Object,
        Undefined
    }

    public Vector2Int Coordinates { get; set; }

    private GameObject childObj;
    private MeshFilter meshFilter; // This is in the child object, not the object with the script

    public HexMeshScriptableObject HexMesh { get; private set; }

    public StructureType Type
    {
        get
        {
            if (HexMesh == null)
                return StructureType.Undefined;

            return HexMesh.structureType;
        }
    }

    public void ChangeHexMesh(HexMeshScriptableObject hexMesh)
    {
        // TODO: Change hitbox also

        HexMesh = hexMesh;

        if (childObj == null)
            childObj = transform.GetChild(0).gameObject;

        if (meshFilter == null)
            meshFilter = childObj.GetComponent<MeshFilter>();

        meshFilter.mesh = hexMesh.mesh;

        float xAxis = HexMesh.imported ? -90 : 0;
        childObj.transform.rotation = Quaternion.Euler(xAxis, 0, 0);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        gameObject.SetActive(false);
    }
}
