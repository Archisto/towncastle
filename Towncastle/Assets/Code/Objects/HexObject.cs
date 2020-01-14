using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexObject : LevelObject, IGridObject
{
    public enum StructureType
    {
        Floor,
        Wall,
        Room,
        Roof,
        Support,
        Protrusion,
        Object,
        Undefined
    }

    // Child object and its components
    private GameObject childObj;
    private MeshFilter meshFilter;
    private MeshRenderer meshRend;

    private int mainLayer;
    private int hiddenLayer = 0; // Default layer

    public HexMeshScriptableObject HexMesh { get; private set; }

    public Vector2Int Coordinates { get; set; }

    public bool Hidden { get; private set; }

    public StructureType Type
    {
        get
        {
            if (HexMesh == null)
                return StructureType.Undefined;

            return HexMesh.structureType;
        }
    }

    public bool IsContentStructure
    {
        get
        {
            return
                Type == StructureType.Support ||
                Type == StructureType.Protrusion ||
                Type == StructureType.Object;
        }
    }

    protected override void InitObject()
    {
        base.InitObject();

        if (childObj == null)
            childObj = transform.GetChild(0).gameObject;

        if (meshRend == null)
            meshRend = childObj.GetComponent<MeshRenderer>();

        //if (!HexMesh.imported)
        //    childObj.transform.rotation = Quaternion.Euler(0, 0, 0);

        mainLayer = childObj.layer;
    }

    public void SetHexMesh(HexMeshScriptableObject hexMesh)
    {
        // TODO: Change hitbox also
        // TODO: Weird rotations, with child object especially

        HexMesh = hexMesh;

        if (childObj == null)
            childObj = transform.GetChild(0).gameObject;

        if (meshFilter == null)
            meshFilter = childObj.GetComponent<MeshFilter>();

        meshFilter.mesh = hexMesh.mesh;

        //float xAxis = HexMesh.imported ? -90 : 0;
        //childObj.transform.rotation = Quaternion.Euler(xAxis, 0, 0);
    }

    public void SetMaterial(Material material, bool disableShadows)
    {
        if (meshRend == null)
            meshRend = childObj.GetComponent<MeshRenderer>();

        meshRend.material = material;

        if (disableShadows)
            meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void Hide(bool hide)
    {
        Hidden = hide;
        meshRend.enabled = !Hidden;
        gameObject.layer = (Hidden ? hiddenLayer : mainLayer);
        childObj.layer = (Hidden ? hiddenLayer : mainLayer);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (meshRend != null)
            Hide(false);

        gameObject.SetActive(false);
    }
}
