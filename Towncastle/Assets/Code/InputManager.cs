﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages user input.
/// </summary>
public class InputManager : MonoBehaviour
{
    private MouseController mouse;
    private ObjectPlacer objPlacer;
    private PlayerController player;

    private SingleInputHandler changeObjKey;
    private SingleInputHandler resetKey;

    private SingleInputHandler[] numberKeys;


    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        mouse = GameManager.Instance.Mouse;
        objPlacer = FindObjectOfType<ObjectPlacer>();
        player = GameManager.Instance.Player;

        changeObjKey = new SingleInputHandler(KeyCode.E);
        resetKey = new SingleInputHandler(KeyCode.R);

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
        changeObjKey.Update();
        if (changeObjKey.JustPressedDown)
        {
            objPlacer.ChangeObject();
        }

        resetKey.Update();
        if (resetKey.JustPressedDown)
        {
            GameManager.Instance.ResetGame();
        }

        numberKeys[1].Update();
        if (numberKeys[1].JustPressedDown)
        {
            objPlacer.ChangeRotationForNextObject(Utils.Direction.Left);
        }

        numberKeys[2].Update();
        if (numberKeys[2].JustPressedDown)
        {
            objPlacer.ChangeRotationForNextObject(Utils.Direction.Right);
        }

        HandleObjPlacingInput();
    }

    private void HandleObjPlacingInput()
    {
        bool add = mouse.LeftButtonReleased;
        bool remove = mouse.RightButtonReleased;
        if (add || remove)
        {
            objPlacer.TryPlaceObject(mouse.Position, remove);
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
