using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject[] testObjs;
    public GameObject[] testObjs2;

    private HexGrid grid;
    private Vector2Int coord;

    private bool useObject1 = true;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        grid = GameManager.Instance.Grid;
        coord = new Vector2Int(-1, -1);
    }

    public void SetCoord(int i)
    {
        if (coord.x < 0)
        {
            coord.x = i;
            Debug.Log("X: " + i);
        }
        else if (coord.y < 0)
        {
            coord.y = i;
            Debug.Log("Y: " + i);
            AddObjectToGridCell(coord);
            coord = new Vector2Int(-1, -1);
        }
    }

    private void AddObjectToGridCell(Vector2Int cell)
    {
        GameObject[] objs = (useObject1 ? testObjs : testObjs2);

        foreach (GameObject obj in objs)
        {
            if (!obj.activeSelf)
            {
                Vector3 newPosition = grid.GetCellCenterWorld(cell);
                if (newPosition.x >= 0)
                {
                    obj.transform.position = newPosition;
                    obj.SetActive(true);
                    Debug.Log("Cell: " + cell);
                }
                else
                {
                    Debug.Log("Unreachable Cell: " + cell);
                }

                return;
            }
        }
    }

    public void ChangeObject()
    {
        useObject1 = !useObject1;
    }
}
