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

    [SerializeField]
    private Utils.HexDirection defaultHexDirection = Utils.HexDirection.Right;

#pragma warning restore 0649

    private HexGrid grid;
    private Vector2Int coord;

    private Pool<HexObject> pool;

    private int currentHexMesh = 0;
    private float objRotation = 0;
    private float defaultDirectionRotationOffset;

    private enum PlacingMode
    {
        Stack,
        Insert,
        Attach,
        Remove
    }

    public Utils.HexDirection ObjectDirection { get; private set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        grid = GameManager.Instance.Grid;
        coord = new Vector2Int(-1, -1);
        ObjectDirection = defaultHexDirection;
        defaultDirectionRotationOffset =
            Utils.AngleFromHexDirectionToAnother(defaultHexDirection, Utils.HexDirection.Right);
        SetRotationForNextObject(ObjectDirection);

        if (hexObjPrefab != null)
        {
            pool = new Pool<HexObject>(hexObjPrefab, hexObjPoolSize, false);
            GameManager.Instance.AddLevelObjectsToList(pool.GetAllObjects());
        }

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
                AddObjectToGridCell(cell, 1);
            }
            else if (removeObj && !cellAvailable)
            {
                grid.EditCell(cell, null);
            }
            else if (!removeObj && !cellAvailable)
            {
                // Testing heightLevel
                AddObjectToGridCell(cell, 2);
            }
            else
            {
                // TODO: Make it possible to remove [heightLevel > 1] objects

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
                    AddObjectToGridCell(coord, 1);
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

    private void AddObjectToGridCell(Vector2Int cell, int heightLevel)
    {
        HexObject newObj = pool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.ChangeHexMesh(hexMeshes[currentHexMesh]);

            Vector3 newPosition = grid.GetCellCenterWorld(cell, defaultYAxis: false);
            if (newPosition.x >= 0)
            {
                // TODO: Ask available heightLevel from grid

                newPosition.y = newObj.HexMesh.defaultPositionY + (heightLevel - 1) * grid.CellHeight;
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

    public void ChangeObject(bool next)
    {
        if (hexMeshes != null && hexMeshes.Length > 1)
        {
            currentHexMesh += next ? 1 : -1;
            if (currentHexMesh >= hexMeshes.Length)
                currentHexMesh = 0;
            else if (currentHexMesh < 0)
                currentHexMesh = hexMeshes.Length - 1;
        }
    }

    public void ChangeRotationForNextObject(Utils.Direction direction)
    {
        bool left = direction == Utils.Direction.Left;
        switch (ObjectDirection)
        {
            case Utils.HexDirection.Left:
                ObjectDirection = (left ? Utils.HexDirection.DownLeft : Utils.HexDirection.UpLeft);
                break;
            case Utils.HexDirection.Right:
                ObjectDirection = (left ? Utils.HexDirection.UpRight : Utils.HexDirection.DownRight);
                break;
            case Utils.HexDirection.UpLeft:
                ObjectDirection = (left ? Utils.HexDirection.Left : Utils.HexDirection.UpRight);
                break;
            case Utils.HexDirection.UpRight:
                ObjectDirection = (left ? Utils.HexDirection.UpLeft : Utils.HexDirection.Right);
                break;
            case Utils.HexDirection.DownLeft:
                ObjectDirection = (left ? Utils.HexDirection.DownRight : Utils.HexDirection.Left);
                break;
            case Utils.HexDirection.DownRight:
                ObjectDirection = (left ? Utils.HexDirection.Right : Utils.HexDirection.DownLeft);
                break;
        }

        objRotation = Utils.AngleFromHexDirection(ObjectDirection);
    }

    public void SetRotationForNextObject(Utils.HexDirection direction)
    {
        objRotation = Utils.AngleFromHexDirection(direction);
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

        float rotY = objRotation +
                     hexMeshes[currentHexMesh].defaultRotationY +
                     Utils.AngleFromHexDirectionToAnother
                        (Utils.HexDirection.Right, hexMeshes[currentHexMesh].mainDirection); // Right is the world main direction

        if (hexMeshes[currentHexMesh].imported)
            newRotation.z = rotY;
        else
            newRotation.y = rotY;

        obj.transform.rotation = Quaternion.Euler(newRotation);
    }

    public string GetPlacementInfo()
    {
        if (hexMeshes == null || hexMeshes.Length == 0)
            return "No hex meshes!";
        
        return string.Format("Selected item: {0} ({1})\nDirection: {2}",
            hexMeshes[currentHexMesh].name, hexMeshes[currentHexMesh].structureType, ObjectDirection);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(Vector3.zero, new Vector3(Mathf.Cos(defaultDirectionRotationOffset), 0, Mathf.Sin(defaultDirectionRotationOffset)));
    //}
}
