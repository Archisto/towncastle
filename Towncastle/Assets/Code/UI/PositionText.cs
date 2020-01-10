using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionText : MonoBehaviour
{
    public GameObject go;

    private Text text;
    private HexGrid grid;

    public Vector3 Value { get; private set; }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        text = GetComponent<Text>();
        grid = FindObjectOfType<HexGrid>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!GameManager.Instance.GameOver)
        {
            /*
            Vector2Int coord = grid.GetCellFromWorldPos(go.transform.position);
            Value = grid.GetCellCenterWorld(coord, defaultYAxis: false);
            text.text = "CellPos: " + Value + "\nGrid: " + grid.GetCellFromWorldPos(Value);
            //text.text = seconds + "." + secondHundreths + " s";
            */

            Value = Input.mousePosition;
            text.text = "MousePos: " + Value;
        }
    }

    public void StopCounter()
    {
        Debug.Log("Time: " + Value + " s");
    }

    public void ResetCounter()
    {
        text.text = "0,00 s";
    }
}
