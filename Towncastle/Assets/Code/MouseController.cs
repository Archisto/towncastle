using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float mouseWorldY = 1f;

#pragma warning disable 0649

    // Testing
    [SerializeField]
    private GameObject testObj;

 #pragma warning restore 0649

    public Vector3 Position { get; private set; }

    public Vector3 LBDownPosition { get; private set; }

    public bool LeftButtonDown { get; private set; }

    public bool RightButtonDown { get; private set; }

    public bool LeftButtonReleased { get; private set; }

    public bool RightButtonReleased { get; private set; }

    public bool Dragging { get; set; }

    /// <summary>
    /// Updates the object once per frame.
    /// </summary>
    private void Update()
    {
        UpdatePosition();
        CheckInput();

        // Testing
        if (testObj)
        {
            testObj.transform.position = Position;
        }
    }

    /// <summary>
    /// Updates the mouse cursor's position in world space.
    /// </summary>
    private void UpdatePosition()
    {
        Vector3 position = Input.mousePosition;
        position.z = 1f;
        position = Camera.main.ScreenToWorldPoint(position);
        Vector3 camPos = Camera.main.transform.position;
        Vector3 dir = (position - camPos).normalized;
        Position = camPos + dir *
            ((camPos.y - mouseWorldY) / (-1f * dir.y));
    }

    private void CheckInput()
    {
        if (LeftButtonReleased)
        {
            Dragging = false;
        }

        LeftButtonReleased = false;
        RightButtonReleased = false;

        bool buttonHeld = LeftButtonDown;
        LeftButtonDown = Input.GetMouseButton(0);
        if (!buttonHeld && LeftButtonDown)
        {
            LBDownPosition = Position;
        }
        else if (buttonHeld && !LeftButtonDown)
        {
            LeftButtonReleased = true;
        }

        buttonHeld = RightButtonDown;
        RightButtonDown = Input.GetMouseButton(1);
        if (buttonHeld && !RightButtonDown)
        {
            RightButtonReleased = true;
        }
    }
}
