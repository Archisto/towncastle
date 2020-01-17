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

#pragma warning restore 0649

        private Image image;

        public ObjectPlacer.EditMode EditMode { get => editMode; }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            image = GetComponentInChildren<Image>();
        }

        public void SetActive(bool activate)
        {
            if (image == null)
                image = GetComponentInChildren<Image>();

            image.color = activate ? onColor : offColor;
        }
    }
}
