﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Towncastle.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
        set
        {
            if (instance == null)
            {
                instance = value;
            }
        }
    }

    #endregion Singleton

    private List<LevelObject> levelObjects;

    private Vector3 playerStartPos;

    public UIManager UI { get; private set; }

    public InputManager Input { get; private set; }

    public MouseController Mouse { get; private set; }

    public CameraController Camera { get; private set; }

    public HexGrid Grid { get; private set; }

    public ObjectPlacer ObjectPlacer { get; private set; }

    public Settings Settings { get; private set; }

    public bool PlayReady { get => !GamePaused && !GameOver; }

    public bool GameOver { get; private set; }

    public bool GamePaused { get; private set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one GameManager found in the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Init()
    {
        Debug.Log("Initializing GameManager...");

        UI = FindObjectOfType<UIManager>();
        Input = FindObjectOfType<InputManager>();
        Mouse = FindObjectOfType<MouseController>();
        Camera = FindObjectOfType<CameraController>();
        Grid = FindObjectOfType<HexGrid>();
        ObjectPlacer = FindObjectOfType<ObjectPlacer>();
        Settings = new Settings(Mouse, Camera);

        InitLevelObjects();

        Debug.Log("GameManager initialized.");
    }

    private void InitLevelObjects()
    {
        levelObjects = new List<LevelObject>();
        LevelObject[] loArray = FindObjectsOfType<LevelObject>();
        foreach (LevelObject lo in loArray)
        {
            AddLevelObjectToList(lo);
        }
    }

    public void AddLevelObjectsToList<T>(List<T> objs)
        where T : LevelObject
    {
        foreach (LevelObject lo in objs)
        {
            AddLevelObjectToList(lo);
        }
    }

    public void AddLevelObjectToList<T>(T obj)
        where T : LevelObject
    {
        levelObjects.Add(obj);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    //private void Update()
    //{
    //    if (!GamePaused)
    //    {
    //        if (!GameOver)
    //        {
    //            // TODO?
    //        }
    //    }
    //}

    public void PauseGame(bool pause)
    {
        if (GamePaused != pause)
        {
            GamePaused = pause;
            UI.ActivatePauseMenu(pause);
        }
    }

    public void TogglePause()
    {
        PauseGame(!GamePaused);
    }

    public void EndGame()
    {
        GameOver = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game.");

        GameOver = false;

        if (levelObjects != null)
        {
            foreach (LevelObject lo in levelObjects)
            {
                lo.ResetObject();
            }
        }

        if (Grid != null)
        {
            Grid.ResetGrid();
            ObjectPlacer.ResetPlacer();
        }

        ObjectPlacer.NotifyStageReset();
    }
}
