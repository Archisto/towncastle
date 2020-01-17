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

#pragma warning disable 0649

    [SerializeField]
    private bool isPreviewObject;

#pragma warning restore 0649

    // Child object and its components
    private GameObject childObj;
    private MeshFilter meshFilter;
    private MeshRenderer meshRend;
    private BuildInstruction buildInstruction;

    private Vector2Int coordinates;
    private Utils.HexDirection direction;
    private float heightLevel = 1;

    private int mainLayer;
    private int hiddenLayer = 0; // Default layer

    public HexMeshScriptableObject HexMesh { get; private set; }

    public Vector2Int Coordinates
    {
        get
        {
            return coordinates;
        }
        set
        {
            coordinates = value;

            if (IsBuildable && !SetupBuildInstruction())
                BuildInstruction.Cell = coordinates;
        }
    }

    public Utils.HexDirection Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;

            if (IsBuildable && !SetupBuildInstruction())
                BuildInstruction.Direction = direction;
        }
    }

    public float HeightLevel
    {
        get
        {
            return heightLevel;
        }
        set
        {
            heightLevel = value;

            if (IsBuildable && !SetupBuildInstruction())
                BuildInstruction.HeightLevel = heightLevel;
        }
    }

    public BuildInstruction BuildInstruction
    {
        get
        {
            if (IsBuildable)
                return buildInstruction;
            else
                return null;
        }
        set
        {
            buildInstruction = value;
        }
    }

    public bool IsBuildable { get => !isPreviewObject; }

    public bool IsBuilt { get => IsBuildable && gameObject.activeSelf; }

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

        mainLayer = childObj.layer;
    }

    private bool SetupBuildInstruction()
    {
        if (buildInstruction == null)
        {
            BuildInstruction =
                new BuildInstruction(HexMesh, Coordinates, Direction, HeightLevel);
            return true;
        }

        return false;
    }

    public void Build()
    {
        // TODO: Is this needed? ObjectPlacer handles building.

        // Grid needs to know about this

        if (BuildInstruction != null)
        {
            SetHexMesh(buildInstruction.HexMesh);
            Coordinates = buildInstruction.Cell;
            Direction = buildInstruction.Direction;
            HeightLevel = buildInstruction.HeightLevel;

            Vector3 newPosition = GameManager.Instance.Grid.GetCellCenterWorld(Coordinates, false);
            newPosition.y += (HeightLevel - 1) * GameManager.Instance.Grid.CellHeight;
            transform.position = newPosition;

            // TODO: Rotation

            Hide(false);
            gameObject.SetActive(true);
        }
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

        int scaleX = HexMesh.flipX ? -1 : 1;
        int scaleY = HexMesh.imported ? (HexMesh.flipZ ? -1 : 1) : 1;
        int scaleZ = HexMesh.imported ? 1 : (HexMesh.flipZ ? -1 : 1);
        childObj.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        if (IsBuildable && !SetupBuildInstruction())
            BuildInstruction.HexMesh = HexMesh;
    }

    public void SetMaterial(Material material, bool disableShadows)
    {
        if (meshRend == null)
            meshRend = childObj.GetComponent<MeshRenderer>();

        meshRend.material = material;

        if (disableShadows)
            meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        childObj.layer = layer;
    }

    public void Hide(bool hide)
    {
        Hidden = hide;
        meshRend.enabled = !Hidden;
        SetLayer(Hidden ? hiddenLayer : mainLayer);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (meshRend != null)
            Hide(false);

        gameObject.SetActive(false);
    }
}
