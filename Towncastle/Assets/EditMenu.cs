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
        private ObjectPlacer objPlacer;
        private Text title;

        /// <summary>
        /// Initializes the menu.
        /// </summary>
        public void Init()
        {
            //ui = GameManager.Instance.UI;
            settings = GameManager.Instance.Settings;
            grid = GameManager.Instance.Grid;
            objPlacer = GameManager.Instance.ObjectPlacer;
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

        public void AffectObjectGroup()
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

        public void AffectCellFullHeight()
        {
            switch (settings.EditMode)
            {
                case ObjectPlacer.EditMode.Add:
                    objPlacer.HeightLevel = 1;
                    objPlacer.AddOrRemoveObjectInSelectedCell(true, false);
                    break;
                case ObjectPlacer.EditMode.Remove:
                    objPlacer.AddOrRemoveObjectInSelectedCell(true, true);
                    break;
                case ObjectPlacer.EditMode.Hide:
                    grid.HideObjectsInCell(objPlacer.Coordinates, true, 0);
                    break;
            }
        }

        public void AffectHeightLevel()
        {
            int heightLevelRounded = objPlacer.HeightLevelRoundedDown;

            for (int y = 0; y < grid.GridSizeY; y++)
            {
                for (int x = 0; x < grid.GridSizeX; x++)
                {
                    switch (settings.EditMode)
                    {
                        case ObjectPlacer.EditMode.Add:
                            if (objPlacer.ObjectsLeft <= 0)
                            {
                                objPlacer.NotifyOutOfObjects();
                                return;
                            }
                            else
                            {
                                objPlacer.AddOrRemoveObject(new Vector2Int(x, y), false, false);
                            }
                            break;
                        case ObjectPlacer.EditMode.Remove:
                            objPlacer.AddOrRemoveObject(new Vector2Int(x, y), false, true);
                            break;
                        case ObjectPlacer.EditMode.Hide:
                            grid.HideObjectsInCell(new Vector2Int(x, y), true, heightLevelRounded);
                            break;
                    }
                }
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
