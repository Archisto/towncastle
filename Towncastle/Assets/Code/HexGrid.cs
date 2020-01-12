using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexBase hexBasePrefab;

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
    private List<HexBase> hexBases;

    [SerializeField]
    public List<HexBase> terrainShapingHexBases;

    [SerializeField]
    private Color gridColor = Color.black;

#pragma warning restore 0649

    private float cellGapZ;

    private MouseController mouse;
    private List<GameObject[]> cellContents;

    public int GridSizeX { get { return gridSizeX; } }

    public int GridSizeY { get { return gridSizeY; } }

    public float CellHeight { get { return cellHeight; } }

    public float HexBaseCount { get { return hexBases.Count; } }

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
        InitHexBases();
        UpdateCellGapZ();
        InitCellContentList();
    }

    private void UpdateCellGapZ()
    {
        cellGapZ = Mathf.Sqrt(Mathf.Pow(cellSize, 2) - Mathf.Pow(cellSize / 2, 2));
    }

    private void InitCellContentList()
    {
        cellContents = new List<GameObject[]>();
        for (int i = 0; i < GridSizeY; i++)
        {
            cellContents.Add(new GameObject[GridSizeX]);
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
                HexBase newHexBase = Instantiate(hexBasePrefab, transform);
                newHexBase.HexGrid = this;
                Vector3 newPosition = GetCellCenterWorld(x, y, defaultYAxis: true);

                // Rises until hits something (use a terrain shaper)
                newPosition.y = GetYWhereHitAbove(newPosition);

                newHexBase.transform.position = newPosition;
                newHexBase.Coordinates = new Vector2Int(x, y);
                newHexBase.name = string.Format("HexBase ({0}, {1})", x, y);
                hexBases.Add(newHexBase);
            }
        }
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
            newPosition.y = GetYWhereHitAbove(newPosition);
            hexBase.transform.position = newPosition;
        }
    }

    public float GetYWhereHitAbove(Vector3 origin)
    {
        float maxDistance = 20f;

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.up, out hit, maxDistance))
        {
            return hit.point.y;
        }

        return origin.y;
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

    public bool CellIsAvailable(int x, int y)
    {
        if (!CellExists(x, y))
            return false;

        return cellContents[y][x] == null;
    }

    public bool CellIsAvailable(Vector2Int coordinates)
    {
        return CellIsAvailable(coordinates.x, coordinates.y);
    }

    public void EditCell(Vector2Int coordinates, GameObject obj)
    {
        if (obj == null && cellContents[coordinates.y][coordinates.x] != null)
        {
            cellContents[coordinates.y][coordinates.x].SetActive(false);
            //Debug.Log("Cell " + coordinates + " is now empty");
        }
        else if (obj != null)
        {
            //Debug.Log("Cell " + coordinates + ": " + obj.name);
        }

        cellContents[coordinates.y][coordinates.x] = obj;
    }

    public Vector3 GetCellCenterWorld(int x, int y, bool defaultYAxis)
    {
        if (cellSize <= 0 || !CellExists(x, y))
        {
            return new Vector3(-1, -1, -1);
        }

        float yAxis = 0;
        if (!defaultYAxis && hexBases != null && hexBases.Count > 0)
        {
            HexBase hexBase = GetHexBaseInHex(x, y);

            if (hexBase != null)
                yAxis = hexBase.Y;
        }

        Vector3 result = transform.position + new Vector3(x * cellSize, yAxis, y * cellGapZ);

        if (y % 2 != 0)
        {
            result.x += 0.5f * cellSize;
        }

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
        float y = (position.z - transform.position.z + 0.5f * cellGapZ) / cellGapZ;

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

    public LevelObject GetObjectInHex(int x, int y)
    {
        return null;
    }

    private HexBase GetHexBaseInHex(int x, int y)
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

    public void ResetGrid()
    {
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                cellContents[y][x] = null;
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
