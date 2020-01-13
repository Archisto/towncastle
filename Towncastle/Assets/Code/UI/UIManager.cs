using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class UIManager : MonoBehaviour
    {
        private Canvas canvas;
        private Camera cam;
        private Vector2 canvasSize;
        private Vector2 uiOffset;

        public Vector2 CanvasSize { get => canvasSize; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            canvas = GetComponent<Canvas>();
            UpdateCanvasSize();
            cam = FindObjectOfType<CameraController>().GetComponent<Camera>();
        }

        public void UpdateCanvasSize()
        {
            canvasSize = canvas.pixelRect.size;
            uiOffset = new Vector2(-0.5f * canvasSize.x, -0.5f * canvasSize.y);
        }

        public void MoveUIObjToWorldPoint(Image uiObj,
                                          Vector3 worldPosition,
                                          Vector2 screenSpaceOffset)
        {
            uiObj.transform.localPosition =
                GetScreenSpacePosition(worldPosition, screenSpaceOffset);
        }

        public Vector2 GetScreenSpacePosition(Vector3 worldPosition,
                                              Vector2 screenSpaceOffset)
        {
            Vector2 viewPortPos = cam.WorldToViewportPoint(worldPosition);
            Vector2 proportionalPosition = new Vector2
                (viewPortPos.x * canvasSize.x, viewPortPos.y * canvasSize.y);
            return proportionalPosition + uiOffset + screenSpaceOffset;
        }

        public void ResetUI()
        {
            
        }
    }
}
