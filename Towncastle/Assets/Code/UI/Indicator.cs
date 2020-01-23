using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Towncastle.UI
{
    public class Indicator : MonoBehaviour
    {
#pragma warning disable 0649

        [SerializeField]
        private Color onColor = Color.blue;

        [SerializeField]
        private Color offColor = Color.gray;

        [SerializeField]
        private ObjectPlacer.EditMode editMode;

        [SerializeField]
        private Image timeBar;

#pragma warning restore 0649

        private Image image;

        public ObjectPlacer.EditMode EditMode { get => editMode; }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            image = GetComponentInChildren<Image>();

            if (timeBar != null)
                timeBar.fillAmount = 0;
        }

        public void SetActive(bool activate)
        {
            if (image == null)
                image = GetComponentInChildren<Image>();

            image.color = activate ? onColor : offColor;
        }

        public void SetActiveIfModeMatches(ObjectPlacer.EditMode mode)
        {
            SetActive(mode == EditMode);
        }

        public bool UpdateProgress(float progress)
        {
            if (timeBar == null)
                return false;

            timeBar.fillAmount = progress;
            return progress >= 1;
        }
    }
}
