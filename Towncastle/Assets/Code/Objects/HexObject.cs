using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexObject : LevelObject
{
    private MeshFilter meshFilter;
    //private ObjectPlacer objPlacer;

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

    //public void Init(ObjectPlacer objPlacer)
    //{
    //    this.objPlacer = objPlacer;
    //}

    public void ChangeHexMesh(HexMeshScriptableObject hexMesh)
    {
        HexMesh = hexMesh;

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = hexMesh.mesh;

        float xAxis = HexMesh.imported ? -90 : 0;
        transform.rotation = Quaternion.Euler(xAxis, 0, 0);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        gameObject.SetActive(false);
    }
}
