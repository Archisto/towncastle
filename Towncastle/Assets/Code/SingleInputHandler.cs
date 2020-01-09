using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInputHandler
{
    public KeyCode key;

    // TODO: Double press
    //private float doublePressTime = 0.5f;

    private bool wasPressedDown;

    public bool Released { get; private set; }

    public bool PressedDown
    {
        get
        {
            return Input.GetKey(key);
        }
    }

    public bool JustPressedDown { get; private set; }

    //public bool DoublePressed { get; private set; }

    /// <summary>
    /// Creates the SingleInputHandler.
    /// </summary>
    /// <param name="keyCode">The key code</param>
    public SingleInputHandler(KeyCode keyCode)
    {
        key = keyCode;
    }

    /// <summary>
    /// Updates the input's state.
    /// </summary>
    public void Update()
    {
        if (PressedDown)
        {
            JustPressedDown = !wasPressedDown;
            wasPressedDown = true;
            Released = false;
        }
        else if (wasPressedDown)
        {
            JustPressedDown = false;
            wasPressedDown = false;
            Released = true;
        }
        else
        {
            Released = false;
        }
    }
}
