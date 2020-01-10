using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tests the world to screen space position conversion by showing the screen space coordinates in the editor.
/// </summary>
public class ScreenSpaceObject : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private Vector2 screenSpacePos;

#pragma warning restore 0649

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        screenSpacePos = GameManager.Instance.UI.
            GetScreenSpacePosition(transform.position, Vector3.zero);
    }
}
