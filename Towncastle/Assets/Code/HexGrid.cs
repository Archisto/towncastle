using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexBase hexBase;

#pragma warning disable 0649

    [SerializeField]
    private int gridSizeX = 10;

    [SerializeField]
    private int gridSizeY = 10;

    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private List<HexBase> hexBases;

    [SerializeField]
    private Color gridColor = Color.black;

#pragma warning restore 0649

    private float cellGapZ;

    private MouseController mouse;
    private List<GameObject[]> cellContents;

    public int GridSizeX { get { return gridSizeX; } }

    public int GridSizeY { get { return gridSizeY; } }

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
        UpdateCellGapZ();
        InitFilledCells();
    }

    private void UpdateCellGapZ()
    {
        cellGapZ = Mathf.Sqrt(Mathf.Pow(cellSize, 2) - Mathf.Pow(cellSize / 2, 2));
    }

    private void InitFilledCells()
    {
        //filledCells = new bool[gridSizeY][]; // Supports only symmetrical arrays (x size == y size)
        cellContents = new List<GameObject[]>();
        for (int i = 0; i < GridSizeY; i++)
        {
            cellContents.Add(new GameObject[GridSizeX]);

            //for (int j = 0; j < filledCells[1].Length; j++)
            //{
            //    filledCells[i][j] = false;
            //}
        }
    }

    public void PopulateHexBases()
    {
        if (hexBase == null)
        {
            return;
        }

        hexBases = new List<HexBase>();

        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                HexBase newHexBase = Instantiate(hexBase, transform);
                newHexBase.transform.position = GetCellCenterWorld(x, y, defaultYAxis: true);
                newHexBase.Coordinates = new Vector2Int(x, y);
                hexBases.Add(newHexBase);
            }
        }
    }

    public void DestroyHexBases()
    {
        if (hexBases == null)
        {
            return;
        }

        for (int i = hexBases.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(hexBases[i].gameObject);
        }

        hexBases.Clear();
    }

    public bool CellExists(int x, int y)
    {
        return !(x < 0 || x >= GridSizeX || y < 0 || y >= GridSizeY);
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
            Debug.Log("Cell " + coordinates + " is now empty");
        }
        else if (obj != null)
        {
            Debug.Log("Cell " + coordinates + ": " + obj.name);
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
            yAxis = GetHexBaseInHex(x, y).Y;
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
            Debug.LogError("Invalid coordinates: (" + x + ", " + y + ")");
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                if (mouse != null && GetCellFromWorldPos(mouse.Position) == coordinates)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = gridColor;
                }

                //Gizmos.DrawSphere(GetCellCenterWorld(coordinates), 0.3f);
                Gizmos.DrawWireSphere(GetCellCenterWorld(coordinates, defaultYAxis: false), cellSize / 2);
            }
        }
    }
}
