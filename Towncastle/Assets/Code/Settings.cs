using System;
using System.Collections;
using UnityEngine;

public class Settings
{
    private MouseController mouse;
    private CameraController camera;

    private LayerMask placerSnapToAll = LayerMask.GetMask("Environment", "LevelObject");
    private LayerMask placerSnapToGridOnly = LayerMask.GetMask("Environment");

    #region Input Settings

    private bool gridObjSnapActive;

    public ObjectPlacer.EditMode EditMode { get; set; }

    public bool MouseCameraMoveActive { get; set; }
    public bool SavingFavoriteActive { get; set; }

    public bool GridObjSnapActive
    {
        get => gridObjSnapActive;
        set
        {
            gridObjSnapActive = value;
            mouse.SetMask(value ? placerSnapToAll : placerSnapToGridOnly);
        }
    }
    public bool AddingToOccupiedCellActive { get; set; }
    public bool SelectAboveHighestOccupiedCellActive { get; set; }
    public bool KeepSameHeightLevelOnUnevenTerrainActive { get; set; }

    #endregion Input Settings

    public Settings(MouseController mouse, CameraController camera)
    {
        this.mouse = mouse;
        this.camera = camera;
    }

    public void ResetSettings()
    {
        EditMode = ObjectPlacer.EditMode.Add;
        SavingFavoriteActive = false;
        GridObjSnapActive = true;
        AddingToOccupiedCellActive = true;
    }
}
