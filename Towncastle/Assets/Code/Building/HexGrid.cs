using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexBase hexBasePrefab;
    public Transform hexBaseParent;

#pragma warning disable 0649

    [SerializeField]
    private int gridSizeX = 10;

    [SerializeField]
    private int gridSizeY = 10;

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private float cellHeight = 2f;

    [SerializeField]
    private int maxHeightLevel = 5;

    [SerializeField]
    private List<HexBase> hexBases;

    [SerializeField]
    public List<HexBase> terrainShapingHexBases;

    [SerializeField]
    private Material hexBaseMaterial1;

    [SerializeField]
    private Material hexBaseMaterial2;

    [SerializeField]
    private Material hexBaseMaterial3;
    
    [SerializeField]
    private Material containsHiddenObjsMaterial;

    [SerializeField]
    private Color gridColor = Color.black;

#pragma warning restore 0649

    private MouseController mouse;
    private List<HexCell[]> cells;

    public int GridSizeX { get => gridSizeX; }

    public int GridSizeY { get => gridSizeY; }

    public float CellSize { get => cellSize; }

    public float CellHeight { get => cellHeight; }

    public float CellGapZ { get; private set; }

    public int MaxHeightLevel { get => maxHeightLevel; }

    public float HexBaseCount { get => hexBases.Count; }

    /// <summary>
    /// OnValidate is called when the script is loaded or a value is changed. (Editor only)
    /// </summary>
    private void OnValidate()
    {
        UpdateCellGapZ();
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        mouse = GameManager.Instance.Mouse;
        InitCells();
        InitHexBases();
        UpdateCellGapZ();
    }

    private void UpdateCellGapZ()
    {
        CellGapZ = Mathf.Sqrt(Mathf.Pow(cellSize, 2) - Mathf.Pow(cellSize / 2, 2));
    }

    private void InitCells()
    {
        cells = new List<HexCell[]>();
        for (int y = 0; y < GridSizeY; y++)
        {
            HexCell[] row = new HexCell[GridSizeX];
            for (int x = 0; x < row.Length; x++)
            {
                row[x] = new HexCell(x, y, MaxHeightLevel);
            }

            cells.Add(row);
        }
    }

    /// <summary>
    /// Finds all hex bases in the hex grid's children
    /// in the case that hex base object references are lost
    /// (making the hex grid a prefab causes this).
    /// </summary>
    public void InitHexBases()
    {
        if (hexBases == null)
            hexBases = new List<HexBase>();

        if (hexBases.Count == 0)
        {
            HexBase[] hbArray = GetComponentsInChildren<HexBase>();
            foreach (HexBase hb in hbArray)
            {
                hexBases.Add(hb);
            }
        }
    }

    public void PopulateHexBases()
    {
        if (hexBasePrefab == null)
            return;

        hexBases = new List<HexBase>();

        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                HexBase newHexBase;
                if (hexBaseParent != null)
                    newHexBase = Instantiate(hexBasePrefab, hexBaseParent);
                else
                    newHexBase = Instantiate(hexBasePrefab);

                newHexBase.SetHexGrid(this);
                Vector3 newPosition = GetCellCenterWorld(x, y, defaultYAxis: true);

                // Rises until hits something (use a terrain shaper)
                // and settles on a height level or its half
                newPosition.y = GetYWhereHitAboveRoundedUp(newPosition);

                newHexBase.transform.position = newPosition;
                newHexBase.Coordinates = new Vector2Int(x, y);
                newHexBase.HeightLevel = GetHeightLevelFromPosY(newPosition.y);
                newHexBase.name = string.Format("HexBase ({0}, {1})", x, y);
                SetHexBaseMaterial(newHexBase);
                hexBases.Add(newHexBase);
            }
        }
    }

    private void SetHexBaseMaterial(HexBase hexBase)
    {
        MeshRenderer meshRend = hexBase.GetComponentInChildren<MeshRenderer>();
        if (hexBase.Coordinates.y % 2 == 0)
        {
            if (hexBase.Coordinates.x % 3 == 0)
            {
                meshRend.material = hexBaseMaterial1;
            }
            else if (hexBase.Coordinates.x % 3 == 1)
            {
                meshRend.material = hexBaseMaterial2;
            }
            else
            {
                meshRend.material = hexBaseMaterial3;
            }
        }
        else
        {
            if (hexBase.Coordinates.x % 3 == 0)
            {
                meshRend.material = hexBaseMaterial3;
            }
            else if (hexBase.Coordinates.x % 3 == 1)
            {
                meshRend.material = hexBaseMaterial1;
            }
            else
            {
                meshRend.material = hexBaseMaterial2;
            }
        }

        hexBase.mainMaterial = meshRend.sharedMaterial;
        hexBase.containsHiddenObjsMaterial = containsHiddenObjsMaterial;
    }

    /// <summary>
    /// All hex bases in terrainShapingHexBases list rise
    /// until they hit something (use a terrain shaper).
    /// </summary>
    public void ShapeTerrainWithSelectedHexBases()
    {
        if (terrainShapingHexBases == null)
            return;

        Vector3 newPosition;
        foreach (HexBase hexBase in terrainShapingHexBases)
        {
            if (hexBase.frozen)
                continue;

            newPosition = hexBase.transform.position;
            newPosition.y = GetYWhereHitAboveRoundedUp(newPosition);
            hexBase.transform.position = newPosition;
            hexBase.HeightLevel = GetHeightLevelFromPosY(newPosition.y);
        }
    }

    public int GetYWhereHitAboveRoundedUp(Vector3 origin)
    {
        float maxDistance = 20f;

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.up, out hit, maxDistance))
        {
            return (int)(hit.point.y + 0.5f);
        }

        return (int)(origin.y + 0.5f);
    }

    public float GetHeightLevelFromPosY(float posY)
    {
        float rawHeightLevel = posY / CellHeight;
        bool halfHeight = Utils.CanBeRoundedUp(rawHeightLevel);
        return 1 + (int)rawHeightLevel + (halfHeight ? 0.5f : 0);
    }

    /// <summary>
    /// All hex bases in terrainShapingHexBases are destroyed.
    /// </summary>
    public void DestroySelectedHexBases()
    {
        if (terrainShapingHexBases == null)
            return;

        foreach (HexBase hexBase in terrainShapingHexBases)
        {
            if (hexBase.frozen)
                continue;

            // The hex base will be null but still occupy the space in the list
            // NO -> hexBases.Remove(hexBase);

            DestroyImmediate(hexBase.gameObject);
        }

        terrainShapingHexBases.Clear();
    }

    public void DestroyAllHexBases()
    {
        if (hexBases == null)
            return;

        for (int i = hexBases.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(hexBases[i].gameObject);
        }

        hexBases.Clear();
        terrainShapingHexBases.Clear();
    }

    public bool CellExists(int x, int y)
    {
        return !(x < 0 || x >= GridSizeX || y < 0 || y >= GridSizeY);
    }

    public bool CellExists(Vector2Int coordinates)
    {
        return CellExists(coordinates.x, coordinates.y);
    }

    public bool CellIsEmpty(int x, int y, int heightLevel)
    {
        if (!CellExists(x, y))
            return false;

        if (heightLevel >= 1)
            return !cells[y][x].HasSomethingOnHeightLevel(heightLevel);
        else
            return cells[y][x].IsEmpty;
    }

    public bool CellIsEmpty(Vector2Int coordinates)
    {
        return CellIsEmpty(coordinates.x, coordinates.y, 0);
    }

    public bool CellIsEmpty(Vector2Int coordinates, int heightLevel)
    {
        return CellIsEmpty(coordinates.x, coordinates.y, heightLevel);
    }

    /// <summary>
    /// Adds an object to or removes an object from a cell.
    /// The rounded height level determines to which
    /// height level list does the object belong.
    /// </summary>
    /// <param name="coordinates">The grid coordinates</param>
    /// <param name="hexObject">The object to add or null to remove</param>
    /// <param name="heightLevelRounded">The rounded height level.
    /// Can be 0 only when removing (removes all height levels).</param>
    /// <returns>Was editing the cell successful</returns>
    public bool EditCell(Vector2Int coordinates, HexObject hexObject, int heightLevelRounded)
    {
        // Add
        if (hexObject != null)
        {
            return cells[coordinates.y][coordinates.x].PlaceObject(hexObject, heightLevelRounded);
        }
        // Remove if not hidden
        else if (hexObject == null && !cells[coordinates.y][coordinates.x].IsEmpty)
        {
            return RemoveObjects(coordinates, false, heightLevelRounded);
        }

        return false;
    }

    private bool RemoveObjects(Vector2Int coordinates, bool removeHidden, int heightLevelRounded = 0)
    {
        HexBase hexBase = GetHexBaseInCell(coordinates.x, coordinates.y);

        // TODO: Remove if not hidden
        // Won't remove if any object in the cell (all height levels) is hidden
        if (!removeHidden && hexBase.ObjectsHidden)
            return false;

        // TODO: Remove all not hidden in cell
        // Full height
        if (heightLevelRounded == 0)
        {
            cells[coordinates.y][coordinates.x].RemoveAllObjects();
            hexBase.ObjectsHidden = false;
            //Debug.Log("Cell " + coordinates + " is now empty");
            return true;
        }
        // Selected height
        else
        {
            //bool success = 
            return cells[coordinates.y][coordinates.x].RemoveObjects(heightLevelRounded);

            //if (success)
            //    Debug.Log(string.Format("Cell {0} (heightLevel: {1}) is now empty", coordinates, heightLevelRounded));
            //else
            //    Debug.LogWarning(string.Format("There are no objects in cell {0} (heightLevel: {1})", coordinates, heightLevelRounded));
        }
    }

    public Vector3 GetCellCenterWorld(int x, int y, bool defaultYAxis)
    {
        if (cellSize <= 0 || !CellExists(x, y))
            return new Vector3(-1, -1, -1);

        float yAxis = 0;
        if (!defaultYAxis && hexBases != null && hexBases.Count > 0)
            yAxis = GetCellY(x, y);

        Vector3 result = transform.position + new Vector3(x * cellSize, yAxis, y * CellGapZ);

        if (y % 2 != 0)
            result.x += 0.5f * cellSize;

        return result;
    }

    public Vector3 GetCellCenterWorld(Vector2Int coordinates, bool defaultYAxis)
    {
        return GetCellCenterWorld(coordinates.x, coordinates.y, defaultYAxis);
    }

    public Vector3 GetCellCenterWorldIfAvailable(Vector2Int coordinates, bool defaultYAxis)
    {
        return GetCellCenterWorld(coordinates.x, coordinates.y, defaultYAxis);
    }

    public Vector2Int GetCellFromWorldPos(Vector3 position)
    {
        if (cellSize <= 0)
            return new Vector2Int(-1, -1);

        float x = (position.x - transform.position.x + 0.5f * cellSize) / cellSize;
        float y = (position.z - transform.position.z + 0.5f * CellGapZ) / CellGapZ;

        if (x < 0 || y < 0)
        {
            return new Vector2Int(-1, -1);
        }

        Vector2Int result = new Vector2Int((int) x, (int) y);

        if (result.y % 2 != 0)
        {
            x = (position.x - transform.position.x) / cellSize;
            if (x < 0f)
            {
                x = -1;
            }

            result.x = (int) x;
        }

        if (!CellExists(result.x, result.y))
        {
            //Debug.Log("Faulty cell: " + result);
            return new Vector2Int(-1, -1);
        }
        else
        {
            //Debug.Log("Cell: " + result);
            return result;
        }
    }

    public float GetCellY(int x, int y)
    {
        HexBase hexBase = GetHexBaseInCell(x, y);
        if (hexBase != null)
            return hexBase.transform.position.y;
        else
            return 0;
    }

    public float GetCellY(Vector2Int coordinates)
    {
        return GetCellY(coordinates.x, coordinates.y);
    }

    public HexObject GetObjectInCell(Vector2Int cell, bool acceptHidden)
    {
        if (CellExists(cell) && !cells[cell.y][cell.x].IsEmpty)
        {
            HexObject hexObject = cells[cell.y][cell.x].GetAny(acceptHidden);
            return hexObject;
        }

        return null;
    }

    public List<HexObject> GetObjectsInCell(Vector2Int cell, bool acceptHidden)
    {
        if (CellExists(cell) && !cells[cell.y][cell.x].IsEmpty)
        {
            List<HexObject> hexObjects = cells[cell.y][cell.x].GetAll(acceptHidden);
            return hexObjects;
        }

        return null;
    }

    public HexBase GetHexBaseInCell(int x, int y)
    {
        if (hexBases == null || hexBases.Count == 0)
        {
            return null;
        }

        int index = y * GridSizeX + x;
        if (index >= 0 && index < hexBases.Count)
        {
            return hexBases[index];
        }
        else
        {
            //Debug.LogError("Invalid coordinates: (" + x + ", " + y + ")");
            return null;
        }
    }

    public void HideObjectsInCell(Vector2Int cell, bool hide, int heightLevelRounded = 0)
    {
        if (CellIsEmpty(cell))
            return;

        // Height level 0 means full height
        if (heightLevelRounded < 1)
            cells[cell.y][cell.x].ActionToAllObjects<HexObject>(hexObj => hexObj.Hide(hide));
        else
            cells[cell.y][cell.x].ActionToObjectsInHeightLevel<HexObject>
                (hexObj => hexObj.Hide(hide), heightLevelRounded);

        GetHexBaseInCell(cell.x, cell.y).ObjectsHidden = hide;
    }

    public void HideAllObjects(bool hide)
    {
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                bool hideSuccess = cells[y][x].ActionToAllObjects<HexObject>(hexObj => hexObj.Hide(hide));

                if (hideSuccess)
                    GetHexBaseInCell(x, y).ObjectsHidden = hide;
            }
        }
    }

    public void ResetGrid()
    {
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                cells[y][x].RemoveAllObjects();
                GetHexBaseInCell(x, y).ObjectsHidden = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                Gizmos.color = gridColor;

                Vector2Int coordinates = new Vector2Int(x, y);

                if (mouse != null)
                {
                    if (mouse.SelectingCoordinates && mouse.Coordinates == coordinates)
                    {
                        Gizmos.color = Color.blue;
                    }
                    //else if (!mouse.SelectingCoordinates && GetCellFromWorldPos(mouse.Position) == coordinates)
                    //{
                    //    Gizmos.color = Color.yellow;
                    //}
                }

                Gizmos.DrawWireSphere(GetCellCenterWorld(coordinates, defaultYAxis: false), cellSize / 2);
            }
        }
    }
}
