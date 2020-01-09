using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tests the world to screen space position conversion by showing the screen space coordinates in the editor.
/// </summary>
public class ScreenSpaceObject : MonoBehaviour
{
    [SerializeField]
    private Vector2 screenSpacePos;

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        screenSpacePos = GameManager.Instance.UI.
            GetScreenSpacePosition(transform.position, Vector3.zero);
    }
}
