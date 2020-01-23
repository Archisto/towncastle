using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Towncastle.UI;

public class ObjectPlacer : MonoBehaviour
{
    public enum EditMode
    {
        None,
        Add,
        Remove,
        Hide
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
    private HexObject previewObject;

    [SerializeField]
    private PlacerObject placerObjMain;

    [SerializeField]
    private PlacerObject placerObjGround;

    [SerializeField]
    private PlacerObject placerObjAltPrefab;

    [SerializeField]
    private HexObject hexObjPrefab;

    [SerializeField]
    private Material previewObjMaterialMain;

    [SerializeField]
    private Material previewObjMaterialOccupied;

    [SerializeField]
    private int hexObjPoolSize = 20;

    [SerializeField]
    private Utils.HexDirection initialHexDirection = Utils.HexDirection.Right;

    [SerializeField]
    private Text objectsRemainingText;

#pragma warning restore 0649

    private UIManager ui;
    private MouseController mouse;
    private Settings settings;
    private HexGrid grid;
    private ObjectCatalog catalog;
    private LineRenderer line;

    private Pool<HexObject> hexObjPool;
    private List<HexObject> hexObjsInPool;
    private Pool<PlacerObject> placerObjAltPool;
    private PlacerObject[] activeAltPlacerObjs;

    private int currentHexMeshIndex = 0;
    private float objRotation = 0;
    private float heightLevel = 1;
    private float preferredHeightLevel = 1;
    private Vector2Int multiSelectionStartCell;

    public Utils.HexDirection CurrentDirection { get; private set; }

    public HexObject PreviewObj { get => previewObject; }

    public bool MultiSelectionActive { get; private set; }

    private HexMeshScriptableObject CurrentHexMesh { get => catalog.GetHexMesh(currentHexMeshIndex); }

    public Vector2Int Coordinates { get => PreviewObj.Coordinates; }

    public float HeightLevel
    {
        get
        {
            return heightLevel;
        }
        set
        {
            SetHeightLevel(value);
            RepositionPreviewObject(Coordinates);
        }
    }

    private float HexBaseHeightLevel { get; set; }

    public int HeightLevelRoundedUp { get => (int)(HeightLevel + 0.5f); }

    public int HeightLevelRoundedDown { get => (int)HeightLevel; }

    public int ObjectsLeft { get; private set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        ui = GameManager.Instance.UI;
        mouse = GameManager.Instance.Mouse;
        settings = GameManager.Instance.Settings;
        grid = GameManager.Instance.Grid;
        catalog = FindObjectOfType<ObjectCatalog>();
        InitLine();
        
        CurrentDirection = initialHexDirection;
        objRotation = Utils.AngleFromHexDirection(CurrentDirection);
        SetRotationForNextObject(CurrentDirection);
        ResetMultiSelection();

        if (hexObjPrefab != null)
        {
            hexObjPool = new Pool<HexObject>(hexObjPrefab, hexObjPoolSize, false);
            hexObjsInPool = hexObjPool.GetAllObjects();
            GameManager.Instance.AddLevelObjectsToList(hexObjsInPool);
            ObjectsLeft = hexObjPoolSize;
        }
        else
        {
            ObjectsLeft = 0;
        }

        if (catalog != null && PreviewObj != null)
        {
            InitPreviewObject();
        }
    }

    private void InitPreviewObject()
    {
        PreviewObj.SetHexMesh(CurrentHexMesh);
        SetRotationForObject(PreviewObj);

        if (placerObjMain == null)
            throw new MissingReferenceException("Main placer object");

        if (placerObjGround == null)
            throw new MissingReferenceException("Ground placer object");

        placerObjGround.gameObject.SetActive(false);
        placerObjAltPool = new Pool<PlacerObject>(placerObjAltPrefab, grid.MaxHeightLevel, false);
        activeAltPlacerObjs = new PlacerObject[grid.MaxHeightLevel - 1];
    }

    private void InitLine()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;

        Debug.Log("Line initialized");
    }

    private void SetHeightLevel(float heightLevel)
    {
        if (heightLevel < 1)
            this.heightLevel = 1;
        else if (heightLevel > grid.MaxHeightLevel)
            this.heightLevel = grid.MaxHeightLevel;
        else
            this.heightLevel = heightLevel;
    }

    public void ChangeHeightLevel(float change)
    {
        HeightLevel += change;
        UpdatePreferredHeight();
    }

    public void UpdatePreferredHeight()
    {
        preferredHeightLevel = HeightLevel;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!ui.EditMenuActive)
            UpdatePreviewObjectPosition();

        UpdateActiveObjectsCountText();
    }

    private void UpdateActiveObjectsCountText()
    {
        // TODO: Smarter way to count active objects
        if (objectsRemainingText)
        {
            int hexObjsInUse = 0;
            foreach (HexObject hexObj in hexObjsInPool)
            {
                if (hexObj.gameObject.activeSelf)
                    hexObjsInUse++;
            }

            ObjectsLeft = hexObjPoolSize - hexObjsInUse;

            objectsRemainingText.text =
                string.Format("Objects remaining\n{0}/{1}",
                    ObjectsLeft, hexObjPoolSize);
        }
    }

    /// <summary>
    /// Updates the preview object's position.
    /// </summary>
    private void UpdatePreviewObjectPosition()
    {
        Vector2Int previewCell = mouse.Coordinates;

        if (PreviewObj == null || !grid.CellExists(previewCell))
            return;

        if (Coordinates != previewCell)
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
            PreviewObj.SetHexMesh(CurrentHexMesh);
            RepositionPreviewObject(Coordinates);
            SetRotationForObject(PreviewObj);
        }
    }

    private void RepositionPreviewObject(Vector2Int previewCell)
    {
        PlacePreviewObject(previewCell, HeightLevel);

        if (placerObjMain != null)
        {
            DisableAllAltPlacerObjects();

            for (int i = 0; i < HeightLevelRoundedUp && i < activeAltPlacerObjs.Length + 1; i++)
            {
                // Top to bottom
                bool topLevel = (i == 0);

                int altPlacerHeightLevel = HeightLevelRoundedUp - i;

                if (topLevel)
                {
                    Material material;

                    // Any object in the cell makes the placer change color.
                    // This is helpful when the hex object has been hidden.
                    if (grid.CellIsEmpty(previewCell))
                    //else if (grid.CellIsAvailable(previewCell, hexMeshes[currentHexMesh].structureType))
                    {
                        material = previewObjMaterialMain;
                    }
                    else
                    {
                        material = previewObjMaterialOccupied;
                    }

                    SetupPlacerObject(placerObjMain, i, false);
                    placerObjMain.SetMaterial(material);
                }
                else if (altPlacerHeightLevel > HexBaseHeightLevel)
                {
                    activeAltPlacerObjs[i - 1] = placerObjAltPool.GetPooledObject(true);
                    if (activeAltPlacerObjs[i - 1] != null)
                        SetupPlacerObject(activeAltPlacerObjs[i - 1], i, true);
                }
            }

            // Ground placer object setup
            if (HexBaseHeightLevel < HeightLevel)
            {
                placerObjGround.gameObject.SetActive(true);
                SetupPlacerObjectOnGround(placerObjGround);
            }
        }
    }

    private void SetupPlacerObject(PlacerObject placerObject, float posY)
    {
        Vector3 newPosition = PreviewObj.transform.position;
        newPosition.y = posY;
        placerObject.transform.position = newPosition;

        SetSimpleRotationForObject(placerObject.gameObject);
    }

    private void SetupPlacerObject(PlacerObject placerObject, int distFromTop, bool snapToFullHeight)
    {
        // Top to bottom because when distFromTop increases, the position gets lower

        float posY = ((snapToFullHeight ?  HeightLevelRoundedUp : HeightLevel) - 1 - distFromTop)
            * grid.CellHeight;
        SetupPlacerObject(placerObject, posY);
    }

    private void SetupPlacerObjectOnGround(PlacerObject placerObject)
    {
        float posY = (HexBaseHeightLevel - 1) * grid.CellHeight;
        SetupPlacerObject(placerObject, posY);
    }

    private void DisableAllAltPlacerObjects()
    {
        placerObjGround.gameObject.SetActive(false);

        for (int i = activeAltPlacerObjs.Length - 1; i >= 0; i--)
        {
            if (activeAltPlacerObjs[i] != null)
            {
                placerObjAltPool.ReturnObject(activeAltPlacerObjs[i]);
                activeAltPlacerObjs[i] = null;
            }
        }
    }

    private void UpdatePreviewRotation()
    {
        SetRotationForObject(PreviewObj);
        SetSimpleRotationForObject(placerObjMain.gameObject);
        foreach (PlacerObject placerObject in activeAltPlacerObjs)
        {
            if (placerObject != null)
                SetSimpleRotationForObject(placerObject.gameObject);
        }
    }

    public void ChangeObject(int index)
    {
        if (catalog != null && catalog.HexMeshCount > 1)
        {
            if (index >= 0 && index < catalog.HexMeshCount)
                currentHexMeshIndex = index;

            catalog.CurrentIndex = currentHexMeshIndex;
            UpdatePreviewObjectHexMesh();
        }
    }

    public void ChangeObject(bool next)
    {
        if (catalog != null && catalog.HexMeshCount > 1)
        {
            currentHexMeshIndex += next ? 1 : -1;
            if (currentHexMeshIndex >= catalog.HexMeshCount)
                currentHexMeshIndex = 0;
            else if (currentHexMeshIndex < 0)
                currentHexMeshIndex = catalog.HexMeshCount - 1;

            catalog.CurrentIndex = currentHexMeshIndex;
            UpdatePreviewObjectHexMesh();
        }
    }

    public void SelectFavoriteHexMesh(int favoriteIndex)
    {
        int newIndex = catalog.GetFavoriteHexMeshFullCatalogIndex(favoriteIndex);
        if (newIndex >= 0)
        {
            if (currentHexMeshIndex != newIndex)
            {
                currentHexMeshIndex = newIndex;
                catalog.CurrentIndex = currentHexMeshIndex;
                UpdatePreviewObjectHexMesh();
            }
        }
        else
        {
            Debug.LogError("Couldn't select favorite hex mesh " + favoriteIndex);
        }
    }

    public bool SaveCurrentHexMeshToFavorites(int favoriteIndex)
    {
        return catalog.SaveCurrentHexMeshToFavorites(favoriteIndex);
    }

    public void PickObject(HexObject hexObject)
    {
        // TODO: Equal size hitboxes make picking the desired object difficult
        // (half height is a start)

        if (hexObject == null)
        {
            Debug.LogWarning("Cannot pick; the selected object is not a HexObject");
            return;
        }

        for (int i = 0; i < catalog.HexMeshCount; i++)
        {
            if (hexObject.HexMesh == catalog.GetHexMesh(i))
            {
                currentHexMeshIndex = i;
                break;
            }
        }

        UpdatePreviewObjectHexMesh();
    }

    public void PickObject(Vector2Int cell)
    {
        HexObject hexObject = grid.GetObjectInCell(cell, false);
        PickObject(hexObject);
    }

    public bool AddOrRemoveObject(Vector2Int cell,
                                  bool fullHeight,
                                  bool removeObj,
                                  BuildInstruction buildInstruction = null)
    {
        if (!grid.CellExists(cell))
            return false;

        // Add
        if (!removeObj)
        {
            if (ObjectsLeft > 0)
            {
                if (settings.AddingToOccupiedCellActive
                    || grid.CellIsEmpty(cell, HeightLevelRoundedDown))
                {
                    // Adds an object according to the build instruction
                    if (buildInstruction != null)
                    {
                        AddObjectToGridCell(buildInstruction);
                    }
                    // Adds objects to the cell in all height levels
                    // if on the ground level or all the way down if not
                    else if (fullHeight)
                        BuildTower(cell);

                    // Add a single object to the cell on the selected height level
                    else
                        AddObjectToGridCell(cell, HeightLevel);

                    return true;
                }
            }
            else
            {
                NotifyOutOfObjects();
            }
        }
        // Remove
        else if (removeObj && ObjectsLeft < hexObjPoolSize)
        {
            bool success = false;

            // Removes all objects in cell from all height levels
            if (fullHeight)
            {
                success = grid.EditCell(cell, null, 0);
            }
            // Removes all objects in cell from the selected rounded height level
            else
            {
                success = grid.EditCell(cell, null, HeightLevelRoundedDown);
            }

            if (success)
                RepositionPreviewObject(Coordinates);
            //else
            //    Debug.LogWarning(
            //        string.Format("Cannot remove from cell {0} (height level: {1})", cell, HeightLevel));

            return success;
        }

        return false;
    }

    public void AddOrRemoveObject(Vector3 position, bool removeObj)
    {
        Vector2Int cell = grid.GetCellFromWorldPos(position);
        AddOrRemoveObject(cell, fullHeight: false, removeObj);
    }

    public void AddOrRemoveObjectInSelectedCell(bool fullHeight, bool removeObj)
    {
        AddOrRemoveObject(Coordinates, fullHeight, removeObj);
    }

    public void AddObject(BuildInstruction buildInstruction)
    {
        AddOrRemoveObject(Vector2Int.zero, false, false, buildInstruction);
    }

    private void BuildTower(Vector2Int cell)
    {
        HexMeshScriptableObject currentHexMesh = CurrentHexMesh;

        float height;
        float heightStep = currentHexMesh.halfHeight ? 0.5f : 1f;

        if (HeightLevel > 1)
        {
            bool stopAtOneAndHalf = (HeightLevel % 1 != 0) && !currentHexMesh.halfHeight;
            for (height = HeightLevel; height >= (stopAtOneAndHalf ? 1.5f : 1f); height -= heightStep)
            {
                if (!AddObjectToGridCell(cell, height))
                    break;
            }
        }
        else
        {
            for (height = 1; height <= grid.MaxHeightLevel; height += heightStep)
            {
                if (!AddObjectToGridCell(cell, height))
                    break;
            }
        }
    }

    private bool AddObjectToGridCell(Vector2Int cell, float heightLevel)
    {
        HexObject newObj = hexObjPool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.SetHexMesh(CurrentHexMesh);
            PlaceObject(newObj, cell, heightLevel);
            return true;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
            return false;
        }
    }

    private bool AddObjectToGridCell(BuildInstruction buildInstruction)
    {
        HexObject newObj = hexObjPool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.SetHexMesh(buildInstruction.HexMesh);
            PlaceObjectUsingBuildInstruction(newObj, buildInstruction);
            return true;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
            return false;
        }
    }

    private void PlaceObject(HexObject hexObj,
                             Vector2Int cell,
                             float rotationY,
                             float heightLevel,
                             bool preview)
    {
        Vector2Int oldCoordinates = hexObj.Coordinates;
        bool changedHeightLevel = heightLevel != hexObj.HeightLevel;
        hexObj.Coordinates = cell;
        hexObj.HeightLevel = heightLevel;

        Vector3 newPosition = grid.GetCellCenterWorld(cell, defaultYAxis: false);

        // The default Y position for hex bases is 0 so newPosition.y starts there
        newPosition.y =
            hexObj.HexMesh.defaultPositionY + (heightLevel - 1) * grid.CellHeight;
        hexObj.transform.position = newPosition;

        if (preview)
        {
            HexBaseHeightLevel = grid.GetHexBaseInCell(cell.x, cell.y).HeightLevel;

            bool seekPreferredHeightLevel = settings.KeepSameHeightLevelOnUnevenTerrainActive;

            //if (cellYAxis > PreviewObj.transform.position.y)

            // Rises to the same height level
            if (HexBaseHeightLevel > HeightLevel)
            {
                SetHeightLevel(HexBaseHeightLevel);
                heightLevel = HeightLevel;
            }
            else if (HeightLevel > HexBaseHeightLevel)
            {
                // Seeks the preferred height level
                if (!changedHeightLevel && seekPreferredHeightLevel
                    && preferredHeightLevel >= HexBaseHeightLevel)
                {
                    SetHeightLevel(preferredHeightLevel);
                    heightLevel = preferredHeightLevel;
                }
                // Lowers to match the difference in height level
                // with the old coordinates' hex base
                else
                {
                    SetHeightLevel(HexBaseHeightLevel
                                   + HeightLevel
                                   - grid.GetHexBaseInCell(oldCoordinates.x, oldCoordinates.y).HeightLevel);
                    heightLevel = HeightLevel;
                }
            }
        }
        else
        {
            hexObj.Direction = CurrentDirection;
            SetRotationForObject(hexObj, rotationY);
            hexObj.gameObject.SetActive(true);
            grid.EditCell(cell, hexObj, (int)heightLevel); // Height level is rounded down
            RepositionPreviewObject(cell);
            ObjectsLeft--;
        }
    }

    private void PlaceObject(HexObject hexObj, Vector2Int cell, float heightLevel)
    {
        PlaceObject(hexObj, cell, objRotation, heightLevel, false);
    }

    private void PlacePreviewObject(Vector2Int cell, float heightLevel)
    {
        PlaceObject(PreviewObj, cell, objRotation, heightLevel, true);
    }

    private void PlaceObjectUsingBuildInstruction(HexObject hexObj, BuildInstruction buildInstruction)
    {
        PlaceObject(hexObj,
                    buildInstruction.Cell,
                    Utils.AngleFromHexDirection(buildInstruction.Direction),
                    buildInstruction.HeightLevel,
                    false);
    }

    public void ChangeRotationForNextObject(Utils.Direction direction)
    {
        bool left = direction == Utils.Direction.Left;
        switch (CurrentDirection)
        {
            case Utils.HexDirection.Left:
                CurrentDirection = (left ? Utils.HexDirection.DownLeft : Utils.HexDirection.UpLeft);
                break;
            case Utils.HexDirection.Right:
                CurrentDirection = (left ? Utils.HexDirection.UpRight : Utils.HexDirection.DownRight);
                break;
            case Utils.HexDirection.UpLeft:
                CurrentDirection = (left ? Utils.HexDirection.Left : Utils.HexDirection.UpRight);
                break;
            case Utils.HexDirection.UpRight:
                CurrentDirection = (left ? Utils.HexDirection.UpLeft : Utils.HexDirection.Right);
                break;
            case Utils.HexDirection.DownLeft:
                CurrentDirection = (left ? Utils.HexDirection.DownRight : Utils.HexDirection.Left);
                break;
            case Utils.HexDirection.DownRight:
                CurrentDirection = (left ? Utils.HexDirection.Right : Utils.HexDirection.DownLeft);
                break;
        }

        objRotation = Utils.AngleFromHexDirection(CurrentDirection);

        UpdatePreviewRotation();
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

    public void SetRotationForObject(HexObject hexObj, float rotationY)
    {
        Vector3 newRotation = hexObj.transform.rotation.eulerAngles;

        float rotY = rotationY +
                     hexObj.HexMesh.defaultRotationY +
                     Utils.AngleFromHexDirectionToAnother
                        (Utils.HexDirection.Right, hexObj.HexMesh.mainDirection); // Right is the world main direction

        newRotation.y = rotY;
        hexObj.transform.rotation = Quaternion.Euler(newRotation);
    }

    public void SetRotationForObject(HexObject hexObj)
    {
        SetRotationForObject(hexObj, objRotation);
    }

    public void SetSimpleRotationForObject(GameObject obj)
    {
        Vector3 newRotation = obj.transform.rotation.eulerAngles;
        float rotY = Utils.AngleFromHexDirection(CurrentDirection);
        newRotation.y = rotY;
        obj.transform.rotation = Quaternion.Euler(newRotation);
    }

    public void MatchRotation(HexObject hexObject)
    {
        if (hexObject == null)
        {
            Debug.LogWarning("Cannot match rotation; the selected object is not a HexObject");
            return;
        }

        CurrentDirection = hexObject.Direction;
        objRotation = Utils.AngleFromHexDirection(CurrentDirection);

        UpdatePreviewRotation();
    }

    public void StartMultiSelection(Vector2Int cell)
    {
        if (MultiSelectionActive)
            return;

        if (cell != multiSelectionStartCell && multiSelectionStartCell.x >= 0)
        {
            MultiSelectionActive = true;
            line.enabled = true;
            UpdateMultiSelection(cell);
        }
        else
        {
            multiSelectionStartCell = cell;
        }
    }

    public void UpdateMultiSelection(Vector2Int mouseCoordinates)
    {
        line.SetPositions(GetMultiSelectionCornerPoints(mouseCoordinates));
    }

    public void FinishMultiSelection(Vector2Int endCell, EditMode function)
    {
        if (!MultiSelectionActive)
            return;

        int width = Mathf.Abs(endCell.x - multiSelectionStartCell.x) + 1;
        int height = Mathf.Abs(endCell.y - multiSelectionStartCell.y) + 1;

        int smallerX = Mathf.Min(multiSelectionStartCell.x, endCell.x);
        int smallerY = Mathf.Min(multiSelectionStartCell.y, endCell.y);

        HexMeshScriptableObject hexMesh = CurrentHexMesh;

        BuildInstruction instruction = null;
        if (function == EditMode.Add)
            instruction = BuildInstruction.Default();

        for (int y = smallerY; y < smallerY + height; y++)
        {
            for (int x = smallerX; x < smallerX + width; x++)
            {
                switch (function)
                {
                    case EditMode.Add:
                        if (ObjectsLeft <= 0)
                        {
                            NotifyOutOfObjects();
                            goto End;
                        }

                        if (instruction != null)
                        {
                            instruction.HexMesh = hexMesh;
                            instruction.Cell = new Vector2Int(x, y);
                            instruction.Direction = CurrentDirection;
                            instruction.HeightLevel = HeightLevel;
                            AddObject(instruction);
                        }
                        break;
                    case EditMode.Remove:
                        AddOrRemoveObject(new Vector2Int(x, y), fullHeight: true, removeObj: true);
                        break;
                    case EditMode.Hide:
                        grid.HideObjectsInCell(new Vector2Int(x, y), true);
                        break;
                }
            }
        }

    End:
        ResetMultiSelection();
    }

    private Vector3[] GetMultiSelectionCornerPoints(Vector2Int mouseCoordinates)
    {
        Vector3[] points = new Vector3[4];
        Vector3 start = grid.GetCellCenterWorld(multiSelectionStartCell, true);
        start.y = 0.1f;
        Vector3 end = grid.GetCellCenterWorld(mouseCoordinates, true);
        end.y = 0.1f;
        points[0] = start;
        points[1] = new Vector3(end.x, start.y, start.z);
        points[2] = end;
        points[3] = new Vector3(start.x, start.y, end.z);
        return points;
    }

    public void ResetMultiSelection()
    {
        MultiSelectionActive = false;
        line.enabled = false;

        if (multiSelectionStartCell.x >= 0)
            multiSelectionStartCell = new Vector2Int(-1, -1);
    }

    public void ResetPlacer()
    {
        if (PreviewObj != null)
        {
            PreviewObj.gameObject.SetActive(true);
            RepositionPreviewObject(Coordinates);
        }

        // How big of a reset are we talking about?
        //HeightLevel = 1;
        //preferredHeightLevel = 1;
    }

    public void NotifyOutOfObjects()
    {
        Debug.LogWarning("No more objects to add");
    }

    public void NotifyStageReset()
    {
        Debug.LogWarning("Stage reset (TODO: Undo)");
    }

    public string GetPlacementInfo()
    {
        if (catalog == null || catalog.HexMeshCount == 0)
            return "Can't access object catalog or it is empty.";

        HexMeshScriptableObject hexMesh = CurrentHexMesh;

        return string.Format(
            "Selected item: {0} ({1})\n" +
            "Direction: {2}\n" +
            "Height level: {3}\n" +
            "Distance to ground: {4}",
            hexMesh.name,
            hexMesh.structureType,
            CurrentDirection,
            HeightLevel,
            HeightLevel - HexBaseHeightLevel);
    }

    //private void OnDrawGizmos()
    //{
    //    if (MultiSelectionActive)
    //    {
    //        Gizmos.color = Color.blue;

    //    }
    //}
}
