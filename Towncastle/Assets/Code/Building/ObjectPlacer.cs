﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour
{
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
    private HexObject previewObject;

    [SerializeField]
    private PlacerObject placerObjMain;

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

    private HexGrid grid;
    private ObjectCatalog catalog;
    private Vector2Int coord;

    private Pool<HexObject> hexObjPool;
    private List<HexObject> hexObjsInPool;
    private Pool<PlacerObject> placerObjAltPool;
    private PlacerObject[] activeAltPlacerObjs;

    private int currentHexMeshIndex = 0;
    private float objRotation = 0;

    public Utils.HexDirection CurrentDirection { get; private set; }

    public HexObject PreviewObj { get => previewObject; }

    private float heightLevel = 1;
    public float HeightLevel
    {
        get
        {
            return heightLevel;
        }
        set
        {
            if (value < 1)
                heightLevel = 1;
            else if (value > grid.MaxHeightLevel)
                heightLevel = grid.MaxHeightLevel;
            else
                heightLevel = value;

            RepositionPreviewObject(PreviewObj.Coordinates);
        }
    }

    public int HeightLevelRoundedUp { get => (int)(HeightLevel + 0.5f); }

    public int HeightLevelRoundedDown { get => (int)HeightLevel; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        grid = GameManager.Instance.Grid;
        catalog = FindObjectOfType<ObjectCatalog>();
        coord = new Vector2Int(-1, -1);
        CurrentDirection = initialHexDirection;
        objRotation = Utils.AngleFromHexDirection(CurrentDirection);
        SetRotationForNextObject(CurrentDirection);

        if (hexObjPrefab != null)
        {
            hexObjPool = new Pool<HexObject>(hexObjPrefab, hexObjPoolSize, false);
            hexObjsInPool = hexObjPool.GetAllObjects();
            GameManager.Instance.AddLevelObjectsToList(hexObjsInPool);
        }

        if (catalog != null && PreviewObj != null)
        {
            InitPreviewObject();
        }
    }

    private void InitPreviewObject()
    {
        PreviewObj.SetHexMesh(catalog.GetHexMesh(currentHexMeshIndex));
        SetRotationForObject(PreviewObj);

        if (placerObjMain != null)
        {
            placerObjAltPool = new Pool<PlacerObject>(placerObjAltPrefab, grid.MaxHeightLevel, false);
            activeAltPlacerObjs = new PlacerObject[grid.MaxHeightLevel - 1];
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        UpdatePreviewObjectPosition();

        // TODO: Smarter way to count active objects
        if (objectsRemainingText)
        {
            int hexObjsInUse = 0;
            foreach (HexObject hexObj in hexObjsInPool)
            {
                if (hexObj.gameObject.activeSelf)
                    hexObjsInUse++;
            }

            objectsRemainingText.text =
                string.Format("Objects remaining\n{0}/{1}",
                    hexObjPoolSize - hexObjsInUse, hexObjPoolSize);
        }
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
            PreviewObj.SetHexMesh(catalog.GetHexMesh(currentHexMeshIndex));
            RepositionPreviewObject(PreviewObj.Coordinates);
            SetRotationForObject(PreviewObj);
        }
    }

    private void RepositionPreviewObject(Vector2Int previewCell)
    {
        PlaceObject(PreviewObj, previewCell, HeightLevel, false);

        if (placerObjMain != null)
        {
            DisableAllAltPlacerObjects();

            for (int i = 0; i < HeightLevelRoundedUp && i < activeAltPlacerObjs.Length + 1; i++)
            {
                // Top to bottom
                bool topLevel = (i == 0);

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
                else
                {
                    activeAltPlacerObjs[i - 1] = placerObjAltPool.GetPooledObject(true);
                    if (activeAltPlacerObjs[i - 1] != null)
                        SetupPlacerObject(activeAltPlacerObjs[i - 1], i, true);
                }
            }
        }
    }

    private void SetupPlacerObject(PlacerObject placerObject, int distFromGround, bool snapToFullHeight)
    {
        // Top to bottom
        Vector3 newPosition = PreviewObj.transform.position;
        newPosition.y = ((snapToFullHeight ?  HeightLevelRoundedUp : HeightLevel) - (distFromGround + 1)) * grid.CellHeight;
        placerObject.transform.position = newPosition;

        SetSimpleRotationForObject(placerObject.gameObject);
    }

    private void DisableAllAltPlacerObjects()
    {
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

    public void ChangeObject(bool next)
    {
        if (catalog != null && catalog.HexMeshCount > 1)
        {
            currentHexMeshIndex += next ? 1 : -1;
            if (currentHexMeshIndex >= catalog.HexMeshCount)
                currentHexMeshIndex = 0;
            else if (currentHexMeshIndex < 0)
                currentHexMeshIndex = catalog.HexMeshCount - 1;

            UpdatePreviewObjectHexMesh();
        }
    }

    public void PickObject(HexObject hexObject)
    {
        // TODO: Equal size hitboxes make picking the desired object difficult

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

    public bool TryPlaceObject(Vector2Int cell,
                               bool fullHeight,
                               bool removeObj,
                               BuildInstruction buildInstruction = null)
    {
        if (grid.CellExists(cell))
        {
            if (!removeObj)
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

                // Add a single objects to the cell on the selected height level
                else
                    AddObjectToGridCell(cell, HeightLevel);

                return true;
            }
            // Remove
            else if (removeObj)
            {
                // Removes all objects in cell from all height levels
                if (fullHeight)
                {
                    grid.EditCell(cell, null, 0);
                }
                // Removes all objects in cell from the selected rounded height level
                else
                {
                    grid.EditCell(cell, null, HeightLevelRoundedDown);
                }

                RepositionPreviewObject(cell);
                return true;
            }
            else
            {
                // TODO: Make it possible to remove [heightLevel > 1] objects

                Debug.LogWarning("Cannot perform action to cell: " + cell);
            }
        }

        return false;
    }

    public void TryPlaceObject(Vector3 position, bool removeObj)
    {
        Vector2Int cell = grid.GetCellFromWorldPos(position);
        TryPlaceObject(cell, false, removeObj);
    }

    public void TryPlaceObject(BuildInstruction buildInstruction)
    {
        TryPlaceObject(Vector2Int.zero, false, false, buildInstruction);
    }

    private void BuildTower(Vector2Int cell)
    {
        HexMeshScriptableObject currentHexMesh = catalog.GetHexMesh(currentHexMeshIndex);

        float height;
        float heightStep = currentHexMesh.halfHeight ? 0.5f : 1f;

        if (HeightLevel > 1)
        {
            bool stopAtOneAndHalf = (HeightLevel % 1 != 0) && !currentHexMesh.halfHeight;
            for (height = HeightLevel; height >= (stopAtOneAndHalf ? 1.5f : 1f); height -= heightStep)
            {
                AddObjectToGridCell(cell, height);
            }
        }
        else
        {
            for (height = 1; height <= grid.MaxHeightLevel; height += heightStep)
            {
                AddObjectToGridCell(cell, height);
            }
        }
    }

    private void AddObjectToGridCell(Vector2Int cell, float heightLevel)
    {
        HexObject newObj = hexObjPool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.SetHexMesh(catalog.GetHexMesh(currentHexMeshIndex));
            PlaceObject(newObj, cell, heightLevel, true);

            return;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
        }
    }

    private void AddObjectToGridCell(BuildInstruction buildInstruction)
    {
        HexObject newObj = hexObjPool.GetPooledObject(false);

        if (newObj != null)
        {
            newObj.SetHexMesh(buildInstruction.HexMesh);
            PlaceObjectUsingBuildInstruction(newObj, buildInstruction);

            return;
        }
        else
        {
            Debug.LogWarning("No more objects to add");
        }
    }

    private void PlaceObject(HexObject hexObj,
                             Vector2Int cell,
                             float rotationY,
                             float heightLevel,
                             bool build)
    {
        // TODO: Just use whatever position and rotation the preview object has?

        hexObj.Coordinates = cell;
        hexObj.HeightLevel = heightLevel;

        Vector3 newPosition = grid.GetCellCenterWorld(cell, defaultYAxis: false);
        newPosition.y +=
            hexObj.HexMesh.defaultPositionY + (heightLevel - 1) * grid.CellHeight;
        hexObj.transform.position = newPosition;

        if (build)
        {
            hexObj.Direction = CurrentDirection;
            SetRotationForObject(hexObj, rotationY);
            hexObj.gameObject.SetActive(true);
            grid.EditCell(cell, hexObj, (int)heightLevel); // Height level is rounded down
            RepositionPreviewObject(cell);
        }
    }

    private void PlaceObject(HexObject hexObj, Vector2Int cell, float heightLevel, bool build)
    {
        PlaceObject(hexObj, cell, objRotation, heightLevel, build);
    }

    private void PlaceObjectUsingBuildInstruction(HexObject hexObj, BuildInstruction buildInstruction)
    {
        PlaceObject(hexObj,
                    buildInstruction.Cell,
                    Utils.AngleFromHexDirection(buildInstruction.Direction),
                    buildInstruction.HeightLevel,
                    true);
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

    public string GetPlacementInfo()
    {
        if (catalog == null || catalog.HexMeshCount == 0)
            return "Can't access object catalog or it is empty.";

        HexMeshScriptableObject hexMesh = catalog.GetHexMesh(currentHexMeshIndex);

        return string.Format("Selected item: {0} ({1})\nDirection: {2}\nHeight level: {3}",
            hexMesh.name, hexMesh.structureType, CurrentDirection, HeightLevel);
    }

    public void ResetPlacer()
    {
        if (PreviewObj != null)
        {
            PreviewObj.gameObject.SetActive(true);
            RepositionPreviewObject(PreviewObj.Coordinates);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(Vector3.zero, new Vector3(Mathf.Cos(defaultDirectionRotationOffset), 0, Mathf.Sin(defaultDirectionRotationOffset)));
    //}
}
