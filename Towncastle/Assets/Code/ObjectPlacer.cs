using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private HexObject hexObjPrefab;

    [SerializeField]
    private HexMeshScriptableObject[] hexMeshes;

    [SerializeField]
    private int hexObjPoolSize = 20;

#pragma warning restore 0649

    private HexGrid grid;
    private Vector2Int coord;

    private Pool<HexObject> pool;

    private int currentHexMesh = 0;
    private float objRotation = 0;

    private enum PlacingMode
    {
        Stack,
        Insert,
        Attach,
        Remove
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        grid = GameManager.Instance.Grid;
        coord = new Vector2Int(-1, -1);

        if (hexObjPrefab != null)
            pool = new Pool<HexObject>(hexObjPrefab, hexObjPoolSize, false);

        if (hexMeshes != null)
            Debug.Log("Selected item: " + hexMeshes[currentHexMesh].name);
    }

    public void TryPlaceObject(Vector3 mousePosition, bool removeObj)
    {
        Vector2Int cell = grid.GetCellFromWorldPos(mousePosition);

        if (grid.CellExists(cell.x, cell.y))
        {
            bool cellAvailable = grid.CellIsAvailable(cell);

            if (!removeObj && cellAvailable)
            {
                AddObjectToGridCell(cell);
            }
            else if (removeObj && !cellAvailable)
            {
                grid.EditCell(cell, null);
            }
            else
            {
                Debug.LogWarning("Cannot perform action to cell: " + cell);
            }
        }
    }

    public void SetCoord(int i, bool removeObj)
    {
        if (coord.x < 0)
        {
            coord.x = i;
        }
        else if (coord.y < 0)
        {
            coord.y = i;

            if (!grid.CellExists(coord.x, coord.y))
            {
                Debug.LogError("Cell doesn't exist: " + coord);
            }
            else
            {
                bool cellAvailable = grid.CellIsAvailable(coord);

                if (!removeObj && cellAvailable)
                {
                    AddObjectToGridCell(coord);
                }
                else if (removeObj && !cellAvailable)
                {
                    grid.EditCell(coord, null);
                }
                else
                {
                    Debug.LogWarning("Cell unavailable: " + coord);
                }
            }

            coord = new Vector2Int(-1, -1);
        }
    }

    private void AddObjectToGridCell(Vector2Int cell)
    {
        HexObject newObj = pool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.ChangeHexMesh(hexMeshes[currentHexMesh]);

            Vector3 newPosition = grid.GetCellCenterWorld(cell, defaultYAxis: false);
            if (newPosition.x >= 0)
            {
                newObj.transform.position = newPosition;
                SetRotationForObject(newObj.gameObject);
                newObj.gameObject.SetActive(true);
                grid.EditCell(cell, newObj.gameObject);
            }
            else
            {
                Debug.LogWarning("Unreachable cell: " + cell);
            }

            return;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
        }
    }

    public void ChangeObject()
    {
        if (hexMeshes != null && hexMeshes.Length > 1)
        {
            currentHexMesh++;
            if (currentHexMesh >= hexMeshes.Length)
                currentHexMesh = 0;
        }
    }

    public void ChangeRotationForNextObject(Utils.Direction direction)
    {
        float degrees = 360 / 6;
        if (direction == Utils.Direction.Left)
            degrees = -1 * degrees;

        objRotation += degrees;
        if (objRotation >= 360)
            objRotation -= 360;
        else if (objRotation < 0)
            objRotation += 360;
    }

    public void ChangeRotationForNextObject(float rotation)
    {
        objRotation = rotation;
        if (objRotation >= 360)
        {
            while (objRotation >= 360)
                objRotation -= 360;
        }
        else if (objRotation < 0)
        {
            while (objRotation < 0)
                objRotation += 360;
        }
    }

    public void SetRotationForObject(GameObject obj)
    {
        Vector3 newRotation = obj.transform.rotation.eulerAngles;

        if (hexMeshes[currentHexMesh].imported)
            newRotation.z = objRotation + hexMeshes[currentHexMesh].defaultRotationY;
        else
            newRotation.y = objRotation + hexMeshes[currentHexMesh].defaultRotationY;

        obj.transform.rotation = Quaternion.Euler(newRotation);
    }

    public string GetPlacementInfo()
    {
        return string.Format("Selected item: {0} ({1})\nRotation: {2} degrees",
            hexMeshes[currentHexMesh].name, hexMeshes[currentHexMesh].structureType, objRotation);
    }
}
