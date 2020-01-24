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
    private Settings settings;

    private SingleInputHandler horizontalInput;
    private SingleInputHandler verticalInput;
    private SingleInputHandler scrollWheelInput;
    private SingleInputHandler saveFavoriteInput;

    private SingleInputHandler changeObjInput;
    private SingleInputHandler turnObjInput;

    private SingleInputHandler leftShiftInput;
    private SingleInputHandler leftCtrlInput;
    private SingleInputHandler leftAltInput;

    private SingleInputHandler matchHeightLevelInput;

    private SingleInputHandler addModeInput;
    private SingleInputHandler removeModeInput;
    private SingleInputHandler hideModeInput;

    private SingleInputHandler toggleGridObjSnap;
    private SingleInputHandler toggleAddingToOccupiedCell;
    private SingleInputHandler toggleAutoselectAboveHighestOccupied;
    private SingleInputHandler toggleKeepSameHeightLevel;

    private SingleInputHandler showAllInput;
    private SingleInputHandler resetCamInput;
    private SingleInputHandler pauseInput;
    private SingleInputHandler helpInput;

    private SingleInputHandler[] numberKeys;

    private Vector2 screenDimensions;
    private bool cancelNextMouseClick;

    private ObjectPlacer.EditMode heldMode = ObjectPlacer.EditMode.None;

    private Indicator addModeIndicator;
    private Indicator removeModeIndicator;
    private Indicator hideModeIndicator;

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
        settings = GameManager.Instance.Settings;
        settings.ResetSettings();

        InitIndicators();
        SetIndicatorStates(settings.EditMode);

        horizontalInput = new SingleInputHandler("Horizontal");
        verticalInput = new SingleInputHandler("Vertical");
        scrollWheelInput = new SingleInputHandler("Mouse ScrollWheel");
        changeObjInput = new SingleInputHandler("Change Object");
        turnObjInput = new SingleInputHandler("Turn Object");
        leftShiftInput = new SingleInputHandler("Alt Action 1");
        leftCtrlInput = new SingleInputHandler("Alt Action 2");
        leftAltInput = new SingleInputHandler("Alt Action 3");
        matchHeightLevelInput = new SingleInputHandler("Match Height Level");
        addModeInput = new SingleInputHandler("Add Mode");
        removeModeInput = new SingleInputHandler("Remove Mode");
        hideModeInput = new SingleInputHandler("Hide Mode");
        toggleGridObjSnap = new SingleInputHandler("Toggle Grid Object Snap");
        toggleAddingToOccupiedCell = new SingleInputHandler("Toggle Adding to Occupied Cell");
        toggleAutoselectAboveHighestOccupied = new SingleInputHandler("Toggle Autoselect Above Highest Occupied Cell");
        toggleKeepSameHeightLevel = new SingleInputHandler("Toggle Keep Same Height Level On Uneven Terrain");
        saveFavoriteInput = new SingleInputHandler("Save Favorite");
        showAllInput = new SingleInputHandler("Show All");
        resetCamInput = new SingleInputHandler("Reset Camera");
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
            settings.MouseCameraMoveActive = true;
        }
    }

    private void InitIndicators()
    {
        Indicator[] indicators = FindObjectsOfType<Indicator>();
        foreach (Indicator indicator in indicators)
        {
            switch (indicator.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    addModeIndicator = indicator;
                    break;
                case ObjectPlacer.EditMode.Remove:
                    removeModeIndicator = indicator;
                    break;
                case ObjectPlacer.EditMode.Hide:
                    hideModeIndicator = indicator;
                    break;
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!Application.isFocused)
        {
            if (!cancelNextMouseClick)
                cancelNextMouseClick = true;

            return;
        }

        if (!GameManager.Instance.GamePaused)
        {
            HandleGameInput();
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

        leftShiftInput.Update();
        leftCtrlInput.Update();
        leftAltInput.Update();

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
            grid.HideAllObjects(false);
        }

        if (ui.EditMenuActive)
        {
            HandleRightClickCancelInput();
        }
        else if (!objPlacer.MultiSelectionActive)
        {
            HandleScrollWheelInput();
            HandleMouseAndModifierInput();
            HandleQuickSettingsInput();

            if (ModeButtonHeldDown())
            {
                UpdateModeHold(settings.EditMode, instant: false);
            }
            else
            {
                ResetModeHold();
                HandleMultiSelectionInput();
                HandleFavoritesInput();
            }
        }
        else
        {
            HandleMultiSelectionInput();
        }
    }

    private void HandleMouseAndModifierInput()
    {
        if (!cancelNextMouseClick)
            HandleObjPlacingInput();
        else if (mouse.LeftButtonReleased || mouse.RightButtonReleased)
            cancelNextMouseClick = false;
    }

    private void HandleCameraInput()
    {
        horizontalInput.Update();
        verticalInput.Update();
        resetCamInput.Update();

        if (settings.MouseCameraMoveActive &&
            MouseCursorNearScreenEdgePercentage(Utils.Direction.Left, 0.05f))
        {
            cam.Move(Utils.Direction.Left, 1);
        }
        else if (settings.MouseCameraMoveActive &&
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

        // The camera can zoom in and out regardless of whether it moves horizontally or not
        if (verticalInput.PressedDown)
        {
            Utils.Direction direction =
                verticalInput.PositiveAxis ? Utils.Direction.Up : Utils.Direction.Down;
            cam.Move(direction, 1);
        }

        if (resetCamInput.JustPressedDown)
        {
            cam.ResetCamera();
        }
    }

    private void HandleRightClickCancelInput()
    {
        if (mouse.RightButtonDown)
        {
            if (cancelNextMouseClick)
            {
                cancelNextMouseClick = false;
            }
            else if (ui.EditMenuActive)
            {
                ui.ActivateEditMenu(false);
                cancelNextMouseClick = true; // Cancels removing
            }
        }
    }

    private void HandleScrollWheelInput()
    {
        scrollWheelInput.Update();
        if (scrollWheelInput.PressedDown)
        {
            // Changing the object
            if (leftAltInput.PressedDown)
            {
                objPlacer.ChangeObject(!scrollWheelInput.PositiveAxis);
            }
            // Turning the object
            else if (leftCtrlInput.PressedDown)
            {
                Utils.Direction direction =
                   scrollWheelInput.PositiveAxis ? Utils.Direction.Left : Utils.Direction.Right;
                objPlacer.ChangeRotationForNextObject(direction);
            }
            // Changing the height level
            else
            {
                float heightLevelChange = leftShiftInput.PressedDown ? 0.5f : 1f;
                heightLevelChange = (scrollWheelInput.PositiveAxis ? 1 : -1) * heightLevelChange;
                objPlacer.ChangeHeightLevel(heightLevelChange);
            }
        }
    }

    private void HandleObjPlacingInput()
    {
        bool add = mouse.LeftButtonReleased;
        bool remove = mouse.RightButtonReleased;

        matchHeightLevelInput.Update();

        if (add || remove)
        {
            if (leftShiftInput.PressedDown)
            {
                if (add)
                    objPlacer.PickObject(mouse.SelectedObject as HexObject);
                else if (remove)
                    objPlacer.MatchRotation(mouse.SelectedObject as HexObject);
            }
            else if (leftCtrlInput.PressedDown)
            {
                grid.HideObjectsInCell(objPlacer.Coordinates, true, objPlacer.HeightLevelRoundedDown);
            }
            else if (leftAltInput.PressedDown)
            {
                if (add)
                    objPlacer.AddOrRemoveObjectInSelectedCell(true, false);
                else if (remove)
                    objPlacer.AddOrRemoveObjectInSelectedCell(true, true);
            }
            else
            {
                objPlacer.AddOrRemoveObjectInSelectedCell(false, remove);
            }
        }
        // Match height level
        else if (matchHeightLevelInput.JustPressedDown)
        {
            // TODO: Annoyingly preview movement and height change happen in different frames

            if (mouse.SelectedObject is HexObject)
            {
                objPlacer.HeightLevel = mouse.SelectedObject.HeightLevel;
                objPlacer.UpdatePreferredHeight();
            }
        }
    }

    private void HandleQuickSettingsInput()
    {
        addModeInput.Update();
        removeModeInput.Update();
        hideModeInput.Update();
        toggleGridObjSnap.Update();
        toggleAddingToOccupiedCell.Update();
        toggleAutoselectAboveHighestOccupied.Update();
        toggleKeepSameHeightLevel.Update();

        // Add mode
        if (addModeInput.JustPressedDown)
        {
            settings.EditMode = ObjectPlacer.EditMode.Add;
            UpdateModeHold(settings.EditMode, instant: leftShiftInput.PressedDown);
            SetIndicatorStates(settings.EditMode);
        }
        // Remove mode
        else if (removeModeInput.JustPressedDown)
        {
            settings.EditMode = ObjectPlacer.EditMode.Remove;
            UpdateModeHold(settings.EditMode, instant: leftShiftInput.PressedDown);
            SetIndicatorStates(settings.EditMode);
        }
        // Hide mode
        else if (hideModeInput.JustPressedDown)
        {
            settings.EditMode = ObjectPlacer.EditMode.Hide;
            UpdateModeHold(settings.EditMode, instant: leftShiftInput.PressedDown);
            SetIndicatorStates(settings.EditMode);
        }
        // Grid object snap toggle
        else if (toggleGridObjSnap.JustPressedDown)
        {
            settings.GridObjSnapActive = !settings.GridObjSnapActive;
            Debug.Log("GridObjSnapActive: " + settings.GridObjSnapActive);
        }
        // Adding to occupied cell toggle
        else if (toggleAddingToOccupiedCell.JustPressedDown)
        {
            settings.AddingToOccupiedCellActive =
                !settings.AddingToOccupiedCellActive;
            Debug.Log("AddingToOccupiedCellActive: " +
                settings.AddingToOccupiedCellActive);
        }
        // Autoselecting above highest occupied cell toggle
        else if (toggleAutoselectAboveHighestOccupied.JustPressedDown)
        {
            settings.AutoselectAboveHighestOccupiedCellActive =
                !settings.AutoselectAboveHighestOccupiedCellActive;

            if (settings.AutoselectAboveHighestOccupiedCellActive)
                objPlacer.RepositionPreviewObject(mouse.Coordinates);

            Debug.Log("AutoselectAboveHighestOccupiedCellActive: " +
                settings.AutoselectAboveHighestOccupiedCellActive);
        }
        // Keep same height level toggle
        else if (toggleKeepSameHeightLevel.JustPressedDown)
        {
            settings.KeepSameHeightLevelOnUnevenTerrainActive =
                !settings.KeepSameHeightLevelOnUnevenTerrainActive;

            if (settings.KeepSameHeightLevelOnUnevenTerrainActive)
                objPlacer.UpdatePreferredHeight();

            Debug.Log("KeepSameHeightLevelOnUnevenTerrainActive: "
                + settings.KeepSameHeightLevelOnUnevenTerrainActive);
        }
    }

    private bool ModeButtonHeldDown()
    {
        switch (settings.EditMode)
        {
            case ObjectPlacer.EditMode.Add:
                return addModeInput.PressedDown;
            case ObjectPlacer.EditMode.Remove:
                return removeModeInput.PressedDown;
            case ObjectPlacer.EditMode.Hide:
                return hideModeInput.PressedDown;
        }

        return false;
    }

    private void UpdateModeHold(ObjectPlacer.EditMode mode, bool instant)
    {
        if (mode != heldMode)
        {
            ResetModeHold();
            heldMode = mode;

            if (instant)
                ActivateEditMenu();
        }
        else
        {
            settings.EditMenuOpeningElapsedTime += Time.deltaTime;
            float progress = Utils.Ratio
                (settings.EditMenuOpeningElapsedTime, 0, settings.EditMenuOpeningHoldTime);

            if (instant || progress >= 1)
            {
                ActivateEditMenu();
            }
            else
            {
                SetIndicatorProgress(heldMode, progress);
            }
        }
    }

    private void ActivateEditMenu()
    {
        ResetModeHold();
        ui.ActivateEditMenu(true);
    }

    private void SetIndicatorStates(ObjectPlacer.EditMode mode)
    {
        addModeIndicator.SetActiveIfModeMatches(mode);
        removeModeIndicator.SetActiveIfModeMatches(mode);
        hideModeIndicator.SetActiveIfModeMatches(mode);
    }

    private void SetIndicatorProgress(ObjectPlacer.EditMode mode, float progress)
    {
        addModeIndicator.UpdateProgress(mode == ObjectPlacer.EditMode.Add ? progress : 0);
        removeModeIndicator.UpdateProgress(mode == ObjectPlacer.EditMode.Remove ? progress : 0);
        hideModeIndicator.UpdateProgress(mode == ObjectPlacer.EditMode.Hide ? progress : 0);
    }

    private void HandleFavoritesInput()
    {
        // Selecting a favorite
        for (int i = 0; i < numberKeys.Length; i++)
        {
            numberKeys[i].Update();
            if (numberKeys[i].JustPressedDown)
            {
                // Saving a favorite to the selected number key
                if (settings.SavingFavoriteActive)
                {
                    settings.SavingFavoriteActive = false;
                    objPlacer.SaveCurrentHexMeshToFavorites(i);
                }
                // Changing to the favorited hex mesh
                else
                {
                    objPlacer.SelectFavoriteHexMesh(i);
                }
            }
        }

        // De/activating the favorite saving mode
        saveFavoriteInput.Update();
        if (saveFavoriteInput.JustPressedDown)
        {
            settings.SavingFavoriteActive = !settings.SavingFavoriteActive;

            if (settings.SavingFavoriteActive)
                Debug.Log("Saving favorite...");
            else
                Debug.Log("Saving favorite cancelled");
        }
    }

    private void HandleMultiSelectionInput()
    {
        if (mouse.LeftButtonDown)
        {
            // Start
            if (leftCtrlInput.PressedDown && !objPlacer.MultiSelectionActive)
            {
                objPlacer.StartMultiSelection(mouse.Coordinates);
            }
            // Update
            else if (leftCtrlInput.PressedDown && objPlacer.MultiSelectionActive)
            {
                objPlacer.UpdateMultiSelection(mouse.Coordinates);
            }
            // Cancel
            else if (objPlacer.MultiSelectionActive && !leftCtrlInput.PressedDown)
            {
                objPlacer.ResetMultiSelection();
                cancelNextMouseClick = true;
            }
        }
        else
        {
            // Finish
            if (objPlacer.MultiSelectionActive)
            {
                objPlacer.FinishMultiSelection(mouse.Coordinates, settings.EditMode);
            }
            // Prevent from starting
            else if (leftCtrlInput.PressedDown)
            {
                objPlacer.ResetMultiSelection();
            }
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
            // Cancels saving a favorite hex mesh
            if (settings.SavingFavoriteActive)
            {
                settings.SavingFavoriteActive = false;
                Debug.Log("Saving favorite cancelled");
            }
            // Closes the editing menu
            else if (ui.EditMenuActive)
            {
                ui.ActivateEditMenu(false);
            }
            // Closes the help screen
            else if (ui.HelpActive)
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
        if (helpInput.JustPressedDown && !ui.EditMenuActive)
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
        settings.MouseCameraMoveActive = active;
    }

    private void ResetModeHold()
    {
        if (heldMode != ObjectPlacer.EditMode.None)
        {
            settings.ResetModeHold();
            SetIndicatorProgress(ObjectPlacer.EditMode.None, 0);
            heldMode = ObjectPlacer.EditMode.None;
        }
    }

    public void ResetInput()
    {
        ResetModeHold();
        cancelNextMouseClick = false;
        settings.SavingFavoriteActive = false;
    }
}
