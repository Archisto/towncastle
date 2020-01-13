using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private const string PreviewObjectString = "PreviewObject";

    private enum EditMode
    {
        Edit, // Add and remove
        Pick, // Copy object to preview
        Hide,
        Isolate // Hide all other objects
    }

    private enum PlacingMode
    {
        Stack, // On the top unoccupied level
        Insert, // Same cell; pushes the cell and everything above up one height level
        Replace, // Same cell
        Attach, // On a neigboring cell
        Remove
    }

    private enum RemovalMode
    {
        MostRecent,
        AllSingleCell,
        AllFullHeight,
        Floor,
        Wall,
        Content,
        Roof
    }

#pragma warning disable 0649

    [SerializeField]
    private HexObject hexObjPrefab;

    [SerializeField]
    private Material previewObjMaterial;

    [SerializeField]
    private HexMeshScriptableObject[] hexMeshes;

    [SerializeField]
    private int hexObjPoolSize = 20;

    [SerializeField]
    private Utils.HexDirection initialHexDirection = Utils.HexDirection.Right;

#pragma warning restore 0649

    private HexGrid grid;
    private Vector2Int coord;

    private Pool<HexObject> pool;

    private int currentHexMesh = 0;
    private float objRotation = 0;

    public Utils.HexDirection ObjectDirection { get; private set; }

    public HexObject PreviewObj { get; private set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        grid = GameManager.Instance.Grid;
        coord = new Vector2Int(-1, -1);
        ObjectDirection = initialHexDirection;
        objRotation = Utils.AngleFromHexDirection(ObjectDirection);
        SetRotationForNextObject(ObjectDirection);

        if (hexObjPrefab != null)
        {
            pool = new Pool<HexObject>(hexObjPrefab, hexObjPoolSize, false);
            GameManager.Instance.AddLevelObjectsToList(pool.GetAllObjects());
            PreviewObj = pool.GetPooledObject(true);
        }

        if (hexMeshes != null)
        {
            InitPreviewObject();
        }
    }

    private void InitPreviewObject()
    {
        if (PreviewObj == null)
            return;

        PreviewObj.gameObject.name = PreviewObjectString;
        PreviewObj.SetHexMesh(hexMeshes[currentHexMesh]);
        PreviewObj.SetMaterial(previewObjMaterial, true);
        SetRotationForObject(PreviewObj.gameObject);
        PreviewObj.gameObject.layer = 0; // Default layer

        // TODO: Proper support for objects following the mouse cursor
        //GameManager.Instance.Mouse.testObj = PreviewObj.gameObject;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        UpdatePreviewObjectPosition();
    }

    /// <summary>
    /// Updates the preview object's position.
    /// </summary>
    private void UpdatePreviewObjectPosition()
    {
        Vector2Int previewCell = GameManager.Instance.Mouse.Coordinates;

        if (PreviewObj == null || !grid.CellExists(previewCell))
            return;

        if (PreviewObj.Coordinates != previewCell)
        {
            //Debug.Log("Previewing: " + previewCell);
            RepositionPreviewObject(previewCell);
        }
    }

    /// <summary>
    /// Updates the preview object's hex mesh.
    /// </summary>
    public void UpdatePreviewObjectHexMesh()
    {
        if (PreviewObj != null)
        {
            PreviewObj.SetHexMesh(hexMeshes[currentHexMesh]);
            RepositionPreviewObject(PreviewObj.Coordinates);
            SetRotationForObject(PreviewObj.gameObject);
        }
    }

    private void RepositionPreviewObject(Vector2Int previewCell)
    {
        // TODO: Better height level

        int heightLevel = 1;
        if (!grid.CellIsAvailable(previewCell))
            heightLevel = 2;

        PlaceObject(PreviewObj, previewCell, heightLevel, false);
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

            UpdatePreviewObjectHexMesh();
        }
    }

    public void PickObject(Vector2Int cell)
    {
        HexObject hexObject = grid.GetObjectInCell(cell);

        if (hexObject == null)
            return;

        for (int i = 0; i < hexMeshes.Length; i++)
        {
            if (hexObject.HexMesh == hexMeshes[i])
            {
                currentHexMesh = i;
                break;
            }
        }

        UpdatePreviewObjectHexMesh();
    }

    public void TryPlaceObject(Vector2Int cell, bool removeObj)
    {
        // TODO: Just use whatever position and rotation the preview object has?

        // Rules for placing objects:
        // 1. Each cell can have a maximum of 4 objects:
        //    1 Floor, 1 Wall, 1 Content (Object, Support, Protrusion) and 1 Roof
        // 2. There has to be a Wall or Support in a cell for placing objects in the cell above
        // 3. Protrusions are exceptions to rule 2:
        //    They can be placed if the neighboring cell on the same height level has a Wall
        // 4. If an object is removed and it is a prerequisite for building on the cells above,
        //    the cells above are also cleared
        //    (the user will be warned and asked to use the Replace mode if they do not want this)
        // 5. Removing an object removes the most recent object in the selected cell
        //    if a special removal mode is not active
        // 6. The rotation of an object never matters

        if (grid.CellExists(cell))
        {
            bool cellAvailable = grid.CellIsAvailable(cell);

            if (!removeObj && cellAvailable)
            {
                AddObjectToGridCell(cell, 1);
            }
            else if (removeObj && !cellAvailable)
            {
                grid.EditCell(cell, null);
                RepositionPreviewObject(cell);
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

    public void TryPlaceObject(Vector3 position, bool removeObj)
    {
        Vector2Int cell = grid.GetCellFromWorldPos(position);
        TryPlaceObject(cell, removeObj);
    }

    private void AddObjectToGridCell(Vector2Int cell, int heightLevel)
    {
        HexObject newObj = pool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.SetHexMesh(hexMeshes[currentHexMesh]);
            PlaceObject(newObj, cell, heightLevel, true);

            return;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
        }
    }

    private void PlaceObject(HexObject hexObj, Vector2Int cell, int heightLevel, bool addToGrid)
    {
        hexObj.Coordinates = cell;

        Vector3 newPosition = grid.GetCellCenterWorld(cell, defaultYAxis: false);
        newPosition.y +=
            hexObj.HexMesh.defaultPositionY + (heightLevel - 1) * grid.CellHeight;
        hexObj.transform.position = newPosition;

        if (addToGrid)
        {
            SetRotationForObject(hexObj.gameObject);
            hexObj.gameObject.SetActive(true);
            grid.EditCell(cell, hexObj.gameObject);
            RepositionPreviewObject(cell);
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
        SetRotationForObject(PreviewObj.gameObject);
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

    public void ResetPlacer()
    {
        if (PreviewObj != null)
        {
            PreviewObj.gameObject.SetActive(true);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(Vector3.zero, new Vector3(Mathf.Cos(defaultDirectionRotationOffset), 0, Mathf.Sin(defaultDirectionRotationOffset)));
    //}
}
