using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private UIManager ui;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            ui = GameManager.Instance.UI;
        }

        public void ResumeGame()
        {
            GameManager.Instance.PauseGame(false);
        }

        public void ActivateOptionsMenu()
        {
            // TODO

            ui.ToggleHelp();
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        public void ChangeToggleState(bool on)
        {
            // TODO: Move to options menu

            GameManager.Instance.Input.SetMouseCameraMoveActive(on);
        }
    }
}
