using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class EditMenu : MonoBehaviour
    {
        private const string AddModeTitle = "Edit Menu: Add";
        private const string RemoveModeTitle = "Edit Menu: Remove";
        private const string HideModeTitle = "Edit Menu: Hide";

        //private UIManager ui;
        private Settings settings;
        private HexGrid grid;
        private Text title;

        /// <summary>
        /// Initializes the menu.
        /// </summary>
        public void Init()
        {
            //ui = GameManager.Instance.UI;
            settings = GameManager.Instance.Settings;
            grid = GameManager.Instance.Grid;
            title = GetComponentInChildren<Text>();
        }

        public void UpdateName()
        {
            switch (settings.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    title.text = AddModeTitle;
                    break;
                case ObjectPlacer.EditMode.Remove:
                    title.text = RemoveModeTitle;
                    break;
                case ObjectPlacer.EditMode.Hide:
                    title.text = HideModeTitle;
                    break;
            }
        }

        public void AffectCellFullHeight()
        {
            switch (settings.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    break;
                case ObjectPlacer.EditMode.Remove:
                    break;
                case ObjectPlacer.EditMode.Hide:
                    break;
            }
        }

        public void AffectHeightLevel()
        {
            switch (settings.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    break;
                case ObjectPlacer.EditMode.Remove:
                    break;
                case ObjectPlacer.EditMode.Hide:
                    break;
            }
        }

        public void AffectAll()
        {
            switch (settings.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    break;
                case ObjectPlacer.EditMode.Remove:
                    GameManager.Instance.ResetGame();
                    break;
                case ObjectPlacer.EditMode.Hide:
                    grid.HideAllObjects(true);
                    break;
            }
        }
    }
}
