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


    // TEE NÄMÄ:
    // 2. Input system - Hide, ShowAll
    // 3. Input system - kameran kierto (kursori reunalla)
    // 4. 3d - katulamppu


    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        cam = GameManager.Instance.Camera;
        mouse = GameManager.Instance.Mouse;
        objPlacer = FindObjectOfType<ObjectPlacer>();
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
        if (horizontalInput.PressedDown)
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
                    objPlacer.PickObject(mouse.Coordinates);
                }
                else if (hideObjInput.PressedDown)
                {
                    objPlacer.ToggleHideObject(mouse.Coordinates);
                }
                else
                {
                    objPlacer.TryPlaceObject(mouse.Coordinates, remove);
                }
            }
            //else
            //{
            //    objPlacer.TryPlaceObject(mouse.Position, remove);
            //}
        }

        //removeKey.Update();

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
}
