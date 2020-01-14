using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages user input.
/// </summary>
public class InputManager : MonoBehaviour
{
    private CameraController cam;
    private MouseController mouse;
    private HexGrid grid;
    private ObjectPlacer objPlacer;
    private PlayerController player;

    private SingleInputHandler horizontalInput;
    private SingleInputHandler verticalInput;
    private SingleInputHandler cameraMoveHorizontalInput;
    private SingleInputHandler changeObjInput;
    private SingleInputHandler turnObjInput;
    private SingleInputHandler pickObjInput;
    private SingleInputHandler hideObjInput;
    private SingleInputHandler showAllInput;
    private SingleInputHandler resetInput;

    private SingleInputHandler[] numberKeys;

    private Vector2 screenDimensions;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        cam = GameManager.Instance.Camera;
        mouse = GameManager.Instance.Mouse;
        grid = GameManager.Instance.Grid;
        objPlacer = GameManager.Instance.ObjectPlacer;
        player = GameManager.Instance.Player;

        horizontalInput = new SingleInputHandler("Horizontal");
        verticalInput = new SingleInputHandler("Vertical");
        cameraMoveHorizontalInput = new SingleInputHandler("Mouse ScrollWheel");
        changeObjInput = new SingleInputHandler("Change Object");
        turnObjInput = new SingleInputHandler("Turn Object");
        pickObjInput = new SingleInputHandler("Alt Action 1");
        hideObjInput = new SingleInputHandler("Alt Action 2");
        showAllInput = new SingleInputHandler("Show All");
        resetInput = new SingleInputHandler("Reset");

        numberKeys = new SingleInputHandler[] {
            new SingleInputHandler(KeyCode.Alpha0),
            new SingleInputHandler(KeyCode.Alpha1),
            new SingleInputHandler(KeyCode.Alpha2),
            new SingleInputHandler(KeyCode.Alpha3),
            new SingleInputHandler(KeyCode.Alpha4),
            new SingleInputHandler(KeyCode.Alpha5),
            new SingleInputHandler(KeyCode.Alpha6),
            new SingleInputHandler(KeyCode.Alpha7),
            new SingleInputHandler(KeyCode.Alpha8),
            new SingleInputHandler(KeyCode.Alpha9)
        };

        screenDimensions = GameManager.Instance.UI.CanvasSize;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        HandleInput();

        if (player != null)
            HandlePlayerMovement();

        HandleDebugInput();
    }

    /// <summary>
    /// Handles user input.
    /// </summary>
    private void HandleInput()
    {
        HandleCameraInput();

        pickObjInput.Update();
        hideObjInput.Update();

        changeObjInput.Update();
        if (changeObjInput.JustPressedDown)
        {
            objPlacer.ChangeObject(changeObjInput.PositiveAxis);
        }

        turnObjInput.Update();
        if (turnObjInput.JustPressedDown)
        {
            Utils.Direction direction =
                turnObjInput.PositiveAxis ? Utils.Direction.Right : Utils.Direction.Left;
            objPlacer.ChangeRotationForNextObject(direction);
        }

        showAllInput.Update();
        if (showAllInput.JustPressedDown)
        {
            objPlacer.HideAllObjects(false);
        }

        resetInput.Update();
        if (resetInput.JustPressedDown)
        {
            GameManager.Instance.ResetGame();
        }

        HandleObjPlacingInput();
    }

    private void HandleCameraInput()
    {
        horizontalInput.Update();
        cameraMoveHorizontalInput.Update();

        if (MouseCursorNearScreenEdgePercentage(Utils.Direction.Left, 0.1f))
        {
            cam.Move(Utils.Direction.Left, 1);
        }
        else if (MouseCursorNearScreenEdgePercentage(Utils.Direction.Right, 0.1f))
        {
            cam.Move(Utils.Direction.Right, 1);
        }
        else if (horizontalInput.PressedDown)
        {
            Utils.Direction direction =
                horizontalInput.PositiveAxis ? Utils.Direction.Right : Utils.Direction.Left;
            cam.Move(direction, 1);
        }
        else if (cameraMoveHorizontalInput.PressedDown)
        {
            Utils.Direction direction =
                cameraMoveHorizontalInput.PositiveAxis ? Utils.Direction.Right : Utils.Direction.Left;
            cam.Move(direction, 8);
        }
    }

    private void HandleObjPlacingInput()
    {
        bool add = mouse.LeftButtonReleased;
        bool remove = mouse.RightButtonReleased;
        if (add || remove)
        {
            if (mouse.SelectingCoordinates)
            {
                if (pickObjInput.PressedDown)
                {
                    objPlacer.PickObject(mouse.SelectedObject as HexObject);
                    //objPlacer.PickObject(mouse.Coordinates);
                }
                else if (hideObjInput.PressedDown)
                {
                    grid.HideObjectsInCell(mouse.Coordinates, true);
                }
                else
                {
                    objPlacer.TryPlaceObject(mouse.Coordinates, remove);
                }
            }
        }

        // NUMBER KEYS

        //for (int i = 0; i < numberKeys.Length; i++)
        //{
        //    numberKeys[i].Update();
        //    if (numberKeys[i].JustPressedDown)
        //    {
        //        objPlacer.SetCoord(i, removeKey.PressedDown);
        //    }
        //}
    }

    /// <summary>
    /// Handles player movement user input.
    /// </summary>
    private void HandlePlayerMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction.z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction.x += 1;
        }

        if (direction != Vector3.zero)
        {
            player.Move(direction);
        }
    }

    /// <summary>
    /// Handles user input for debugging.
    /// </summary>
    private void HandleDebugInput()
    {
        
    }

    /// <summary>
    /// Determines whether the mouse cursor is near the edge of the screen.
    /// </summary>
    /// <param name="side">A specific side or None for any</param>
    /// <param name="maxDistance">How close to the edge should the cursor be (in pixels)</param>
    /// <returns>Is the mouse cursor near the edge of the screen</returns>
    private bool MouseCursorNearScreenEdge(Utils.Direction side, float maxDistance)
    {
        bool up = Input.mousePosition.y >= screenDimensions.y - maxDistance;
        bool down = Input.mousePosition.y <= maxDistance;
        bool left = Input.mousePosition.x <= maxDistance;
        bool right = Input.mousePosition.x >= screenDimensions.x - maxDistance;

        switch (side)
        {
            case Utils.Direction.Up:
                return up;
            case Utils.Direction.Down:
                return down;
            case Utils.Direction.Left:
                return left;
            case Utils.Direction.Right:
                return right;

            // Any side
            case Utils.Direction.None:
                Utils.Direction nearSide = Utils.Direction.None;
                if (up)
                    nearSide = Utils.Direction.Up;
                else if (down)
                    nearSide = Utils.Direction.Down;
                else if(left)
                    nearSide = Utils.Direction.Left;
                else if (right)
                    nearSide = Utils.Direction.Right;

                if (nearSide != Utils.Direction.None)
                {
                    Debug.Log("Mouse cursor near edge: " + nearSide + " " + Input.mousePosition);
                    return true;
                }

                break;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the mouse cursor is near the edge of the screen.
    /// </summary>
    /// <param name="side">A specific side or None for any</param>
    /// <param name="screenSizeRatio">How close to the edge should the cursor be
    /// (a percentage of the screen width/height)</param>
    /// <returns>Is the mouse cursor near the edge of the screen</returns>
    private bool MouseCursorNearScreenEdgePercentage(Utils.Direction side, float screenSizeRatio)
    {
        screenSizeRatio = Mathf.Clamp01(screenSizeRatio);

        float maxDistance = 0f;
        switch (side)
        {
            case Utils.Direction.Up:
            case Utils.Direction.Down:
                maxDistance = screenSizeRatio * screenDimensions.y;
                break;
            case Utils.Direction.Left:
            case Utils.Direction.Right:
                maxDistance = screenSizeRatio * screenDimensions.x;
                break;
        }

        return MouseCursorNearScreenEdge(side, maxDistance);
    }
}
