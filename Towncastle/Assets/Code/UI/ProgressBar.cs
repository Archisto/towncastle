using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private Image fillImg;

    [SerializeField]
    private Utils.Direction barDirection = Utils.Direction.Right;

    [SerializeField]
    private float maxFillSize = -200f;

#pragma warning restore 0649


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        // TODO: Abstract parent class

        return;

        float ratio = 0;
        //float ratio = (float) player.Hitpoints / player.MaxHitpoints;

        if (fillImg != null)
        {
            if (ratio == 0f && fillImg.enabled)
            {
                fillImg.enabled = false;
            }
            else if (ratio > 0f && !fillImg.enabled)
            {
                fillImg.enabled = true;
            }

            switch (barDirection)
            {
                case Utils.Direction.Up:
                    fillImg.rectTransform.offsetMax = new Vector2(fillImg.rectTransform.offsetMax.x, -1 * ratio * maxFillSize);
                    break;

                case Utils.Direction.Down:
                    fillImg.rectTransform.offsetMin = new Vector2(fillImg.rectTransform.offsetMin.x, ratio * maxFillSize);
                    break;

                case Utils.Direction.Left:
                    fillImg.rectTransform.offsetMin = new Vector2(ratio * maxFillSize, fillImg.rectTransform.offsetMin.y);
                    break;

                case Utils.Direction.Right:
                    fillImg.rectTransform.offsetMax = new Vector2(-1 * ratio * maxFillSize, fillImg.rectTransform.offsetMax.y);
                    break;
            }
        }
    }

    public void ResetHPBar()
    {
        fillImg.enabled = true;
    }
}
