using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    private Text text;

    public float Value { get; private set; }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        text = GetComponent<Text>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!GameManager.Instance.GameOver)
        {
            int seconds = (int) GameManager.Instance.ElapsedTime;
            int secondHundreths = (int) ((GameManager.Instance.ElapsedTime - seconds) * 100);
            Value = seconds + ((float)secondHundreths) / 100;
            text.text = Value + " s";
            //text.text = seconds + "." + secondHundreths + " s";
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
