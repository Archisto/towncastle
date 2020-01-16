using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Towncastle.UI;

/// <summary>
/// Manages user input.
/// </summary>
public class InputManager : MonoBehaviour
{
    private UIManager ui;
    private CameraController cam;
    private MouseController mouse;
    private HexGrid grid;
    private ObjectPlacer objPlacer;
    private PlayerController player;

    private SingleInputHandler horizontalInput;
    private SingleInputHandler verticalInput;
    private SingleInputHandler scrollWheelInput;
    private SingleInputHandler changeObjInput;
    private SingleInputHandler turnObjInput;
    private SingleInputHandler pickObjInput;
    private SingleInputHandler hideObjInput;
    private SingleInputHandler alt3Input;
    private SingleInputHandler showAllInput;
    private SingleInputHandler resetInput;
    private SingleInputHandler pauseInput;
    private SingleInputHandler helpInput;

    private SingleInputHandler[] numberKeys;

    private Vector2 screenDimensions;
    private bool mouseCameraMoveActive; // Enabled by default if we get the screen dimensions

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        ui = GameManager.Instance.UI;
        cam = GameManager.Instance.Camera;
        mouse = GameManager.Instance.Mouse;
        grid = GameManager.Instance.Grid;
        objPlacer = GameManager.Instance.ObjectPlacer;
        player = GameManager.Instance.Player;

        horizontalInput = new SingleInputHandler("Horizontal");
        verticalInput = new SingleInputHandler("Vertical");
        scrollWheelInput = new SingleInputHandler("Mouse ScrollWheel");
        changeObjInput = new SingleInputHandler("Change Object");
        turnObjInput = new SingleInputHandler("Turn Object");
        pickObjInput = new SingleInputHandler("Alt Action 1");
        hideObjInput = new SingleInputHandler("Alt Action 2");
        alt3Input = new SingleInputHandler("Alt Action 3");
        showAllInput = new SingleInputHandler("Show All");
        resetInput = new SingleInputHandler("Reset");
        pauseInput = new SingleInputHandler("Pause");
        helpInput = new SingleInputHandler("Help");

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

        if (GameManager.Instance.UI != null)
        {
            screenDimensions = GameManager.Instance.UI.CanvasSize;
            mouseCameraMoveActive = true;
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!GameManager.Instance.GamePaused)
        {
            HandleGameInput();

            if (player != null)
                HandlePlayerMovement();

            HandlePauseInput();
        }
        else
        {
            HandleMenuInput();
        }

        HandleDebugInput();
    }

    /// <summary>
    /// Handles user input.
    /// </summary>
    private void HandleGameInput()
    {
        HandleCameraInput();

        pickObjInput.Update();
        hideObjInput.Update();
        alt3Input.Update();

        // Changing the object
        changeObjInput.Update();
        if (changeObjInput.JustPressedDown)
        {
            objPlacer.ChangeObject(changeObjInput.PositiveAxis);
        }

        // Turning the object
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
            // TESTING: Shows all objects, even untracked ones
            objPlacer.HideAllObjectsDebug(false);

            // Not part of testing but still needed:
            objPlacer.HideAllObjects(false);
        }

        resetInput.Update();
        if (resetInput.JustPressedDown)
        {
            GameManager.Instance.ResetGame();
        }

        HandleScrollWheelInput();
        HandleObjPlacingInput();
    }

    private void HandleCameraInput()
    {
        horizontalInput.Update();
        verticalInput.Update();

        if (mouseCameraMoveActive &&
            MouseCursorNearScreenEdgePercentage(Utils.Direction.Left, 0.05f))
        {
            cam.Move(Utils.Direction.Left, 1);
        }
        else if (mouseCameraMoveActive &&
                 MouseCursorNearScreenEdgePercentage(Utils.Direction.Right, 0.05f))
        {
            cam.Move(Utils.Direction.Right, 1);
        }
        else if (horizontalInput.PressedDown)
        {
            Utils.Direction direction =
                horizontalInput.PositiveAxis ? Utils.Direction.Right : Utils.Direction.Left;
            cam.Move(direction, 1);
        }

        if (verticalInput.PressedDown)
        {
            Utils.Direction direction =
                verticalInput.PositiveAxis ? Utils.Direction.Up : Utils.Direction.Down;
            cam.Move(direction, 1);
        }
    }

    private void HandleScrollWheelInput()
    {
        scrollWheelInput.Update();
        if (scrollWheelInput.PressedDown)
        {
            // Changing the object
            if (alt3Input.PressedDown)
            {
                objPlacer.ChangeObject(!scrollWheelInput.PositiveAxis);
            }
            // Turning the object
            else if (hideObjInput.PressedDown)
            {
                Utils.Direction direction =
                   scrollWheelInput.PositiveAxis ? Utils.Direction.Left : Utils.Direction.Right;
                objPlacer.ChangeRotationForNextObject(direction);
            }
            // Changing the height level
            else
            {
                float heightLevelChange = pickObjInput.PressedDown ? 0.5f : 1f;
                objPlacer.HeightLevel += (scrollWheelInput.PositiveAxis ? 1 : -1) * heightLevelChange;
            }
        }
    }

    private void HandleObjPlacingInput()
    {
        bool add = mouse.LeftButtonReleased;
        bool remove = mouse.RightButtonReleased;
        if (add || remove)
        {
            // If coordinates don't have to be actively selected,
            // the last valid coordinates remain
            bool activeSelectionRequired = false;

            if (!activeSelectionRequired || mouse.SelectingCoordinates)
            {
                if (pickObjInput.PressedDown)
                {
                    if (add)
                        objPlacer.PickObject(mouse.SelectedObject as HexObject);
                    else if (remove)
                        objPlacer.MatchRotation(mouse.SelectedObject as HexObject);
                }
                else if (hideObjInput.PressedDown)
                {
                    // TESTING: Hides all objects in cell, even untracked ones
                    bool somethingWasHidden = objPlacer.HideAllObjectsInCell(mouse.Coordinates, true);
                    if (somethingWasHidden)
                    {
                        HexBase hexBase = grid.GetHexBaseInCell(mouse.Coordinates.x, mouse.Coordinates.y);
                        if (hexBase != null)
                            hexBase.ObjectsHidden(true);
                    }

                    //grid.HideObjectsInCell(mouse.Coordinates, true);
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
    /// Handles user input in the menus.
    /// </summary>
    private void HandleMenuInput()
    {
        HandlePauseInput();
    }

    /// <summary>
    /// Handles user input for pausing and unpausing the game
    /// and going back in the menus.
    /// </summary>
    private void HandlePauseInput()
    {
        pauseInput.Update();
        if (pauseInput.JustPressedDown)
        {
            // Closes the help screen
            if (ui.HelpActive)
            {
                ui.ToggleHelp();
            }
            // Toggles between game and pause menu
            else
            {
                GameManager.Instance.TogglePause();
                mouse.ResetMouse();
            }
        }

        helpInput.Update();
        if (helpInput.JustPressedDown)
        {
            ui.ToggleHelp();
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

    public void SetMouseCameraMoveActive(bool active)
    {
        mouseCameraMoveActive = active;
    }
}
