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

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }

    public float Y { get => transform.position.y; }

    public HexGrid HexGrid { get; set; }

    public void ShapeTerrain()
    {
        if (frozen)
            return;

        Vector3 newPosition = transform.position;
        newPosition.y = HexGrid.GetYWhereHitAbove(newPosition);
        transform.position = newPosition;
    }
}
