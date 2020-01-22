using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexBase : MonoBehaviour, IGridObject
{
#pragma warning disable 0649

    [SerializeField]
    private Vector2Int coordinates;

    [SerializeField]
    private float heightLevel;

    [SerializeField]
    public bool frozen;

#pragma warning restore 0649

    private HexGrid grid;

    public Material mainMaterial;
    public Material containsHiddenObjsMaterial;

    private MeshRenderer meshRend;
    private bool objectsHidden;

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }

    public float HeightLevel { get => heightLevel; set => heightLevel = value; }

    public bool ObjectsHidden
    {
        get
        {
            return objectsHidden;
        }
        set
        {
            if (containsHiddenObjsMaterial == null)
                return;

            objectsHidden = value;
            meshRend.material = (value ? containsHiddenObjsMaterial : mainMaterial);
        }
    }

    public void SetHexGrid(HexGrid hexGrid)
    {
        grid = hexGrid;
    }

    private void Start()
    {
        meshRend = GetComponentInChildren<MeshRenderer>();
    }

    public void ShapeTerrain()
    {
        if (frozen)
            return;

        Vector3 newPosition = transform.position;
        newPosition.y = grid.GetYWhereHitAboveRoundedUp(newPosition);
        HeightLevel = FindObjectOfType<HexGrid>().GetHeightLevelFromPosY(newPosition.y);
        transform.position = newPosition;
    }
}
