using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInputHandler
{
    public string inputName;
    public KeyCode key;

    private bool wasPressedDown;
    private bool usesInputName;

    public float Value
    {
        get
        {
            if (usesInputName)
                return Input.GetAxisRaw(inputName);
            else
                return Input.GetKey(key) ? 1 : 0;
        }
    }

    public bool Released { get; private set; }

    public bool PressedDown { get => Value != 0; }

    public bool JustPressedDown { get; private set; }

    /// <summary>
    /// Determines whether the input used the positive axis when it was last pressed down.
    /// The value remains unchanged if the input is released.
    /// </summary>
    public bool PositiveAxis { get; private set; }

    /// <summary>
    /// Creates the SingleInputHandler. Uses the Unity Input system.
    /// </summary>
    /// <param name="inputName">The input name</param>
    public SingleInputHandler(string inputName)
    {
        this.inputName = inputName;
        usesInputName = true;
    }

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
            PositiveAxis = Value > 0;

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
