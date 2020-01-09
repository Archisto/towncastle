using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private int gridSizeX = 10;

    [SerializeField]
    private int gridSizeY = 10;

    [SerializeField]
    private float cellSize = 1f;


    [SerializeField]
    private Vector3 bottomLeftCorner;

    [SerializeField]
    private Vector3 topRightCorner;

    [SerializeField]
    private Color gridColor = Color.black;

    [SerializeField]
    private Transform testTf;

#pragma warning restore 0649

    private float cellGapZ;

    /// <summary>
    /// OnValidate is called when the script is loaded or a value is changed.
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
        UpdateCellGapZ();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        
    }

    private void UpdateCellGapZ()
    {
        cellGapZ = Mathf.Sqrt(Mathf.Pow(cellSize, 2) - Mathf.Pow(cellSize / 2, 2));
    }

    public Vector3 GetCellCenterWorld(int x, int y)
    {
        if (cellSize <= 0 || x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
            return new Vector3(-1, -1, -1);
        }

        Vector3 result = transform.position + new Vector3(x * cellSize, 0, y * cellGapZ);

        if (y % 2 != 0)
        {
            result.x += 0.5f * cellSize;
        }

        return result;
    }

    public Vector3 GetCellCenterWorld(Vector2Int coordinates)
    {
        return GetCellCenterWorld(coordinates.x, coordinates.y);
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

        if (result.x < 0 || result.x >= gridSizeX || result.y < 0 || result.y >= gridSizeY)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int i = 0; i < gridSizeY; i++)
        {
            for (int j = 0; j < gridSizeX; j++)
            {
                Vector2Int coordinates = new Vector2Int(j, i);

                if (testTf != null && GetCellFromWorldPos(testTf.position) == coordinates)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = gridColor;
                }

                //Gizmos.DrawSphere(GetCellCenterWorld(coordinates), 0.3f);
                Gizmos.DrawWireSphere(GetCellCenterWorld(coordinates), cellSize / 2);
            }
        }
    }
}
