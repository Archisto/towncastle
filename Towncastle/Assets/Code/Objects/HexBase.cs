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
    public bool frozen;

#pragma warning restore 0649

    public Material mainMaterial;
    public Material containsHiddenObjsMaterial;

    private MeshRenderer meshRend;
    private bool objectsHidden;

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }

    public float HeightLevel { get; set; }

    public HexGrid HexGrid { get; set; }

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

    private void Start()
    {
        meshRend = GetComponentInChildren<MeshRenderer>();
    }

    public void ShapeTerrain()
    {
        if (frozen)
            return;

        Vector3 newPosition = transform.position;
        newPosition.y = HexGrid.GetYWhereHitAboveRoundedUp(newPosition);
        HeightLevel = FindObjectOfType<HexGrid>().GetHeightLevelFromPosY(newPosition.y);
        transform.position = newPosition;
    }
}
