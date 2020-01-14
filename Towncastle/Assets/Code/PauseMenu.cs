using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public void ResumeGame()
        {
            GameManager.Instance.PauseGame(false);
        }

        public void ActivateOptionsMenu()
        {
            // TODO
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
