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

    private Shooter shooter;
    private TimeCounter counter;
    private LevelObject[] levelObjects;

    private Vector3 playerStartPos;

    public UIManager UI { get; private set; }

    public MouseController Mouse { get; private set; }

    public HexGrid Grid { get; private set; }

    public PlayerController Player { get; private set; }

    public float ElapsedTime { get; private set; }

    public bool PlayReady { get { return !GamePaused && !GameOver; } }

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
        Mouse = FindObjectOfType<MouseController>();
        Grid = FindObjectOfType<HexGrid>();
        Player = FindObjectOfType<PlayerController>();
        shooter = FindObjectOfType<Shooter>();
        counter = FindObjectOfType<TimeCounter>();
        levelObjects = FindObjectsOfType<LevelObject>();

        if (Player != null)
            playerStartPos = Player.transform.position;

        Debug.Log("GameManager initialized.");
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!GamePaused)
        {
            if (!GameOver)
            {
                UpdateTime();
            }
        }
    }

    private void UpdateTime()
    {
        ElapsedTime += Time.deltaTime;
    }

    public void EndGame()
    {
        GameOver = true;

        if (counter != null)
            counter.StopCounter();
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game.");

        GameOver = false;
        ElapsedTime = 0f;

        if (Player != null)
        {
            Player.transform.position = playerStartPos;
            Player.ResetPlayer();
        }

        if (shooter != null)
            shooter.ResetShooter();

        if (counter != null)
            counter.ResetCounter();

        if (levelObjects != null)
        {
            foreach (LevelObject lo in levelObjects)
            {
                lo.ResetObject();
            }
        }
    }
}
