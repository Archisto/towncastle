using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class UIManager : MonoBehaviour
    {
#pragma warning disable 0649

        [SerializeField]
        private PauseMenu pauseMenu;

        [SerializeField]
        private GameObject helpMenu;

#pragma warning restore 0649

        private Canvas canvas;
        private Camera cam;

        private Vector2 canvasSize;
        private Vector2 uiOffset;

        public Vector2 CanvasSize { get => canvasSize; }

        public bool HelpActive { get; private set; }

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

        public void ActivatePauseMenu(bool activate)
        {
            pauseMenu.gameObject.SetActive(activate);

            if (activate && HelpActive)
                ToggleHelp();
        }

        public void ToggleHelp()
        {
            HelpActive = !HelpActive;
            helpMenu.gameObject.SetActive(HelpActive);

            if (HelpActive && GameManager.Instance.GamePaused)
                GameManager.Instance.PauseGame(false);
        }

        public void ResetUI()
        {
            ActivatePauseMenu(false);
        }
    }
}
