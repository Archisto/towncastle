using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImg;

    [SerializeField]
    private HelperMethods.Direction barDirection = HelperMethods.Direction.Right;

    [SerializeField]
    private float maxFillSize = -200f;

    PlayerController player;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (player.MaxHitpoints <= 0)
            return;

        float hpRatio = (float) player.Hitpoints / player.MaxHitpoints;

        if (fillImg != null)
        {
            if (hpRatio == 0f && fillImg.enabled)
            {
                fillImg.enabled = false;
            }
            else if (hpRatio > 0f && !fillImg.enabled)
            {
                fillImg.enabled = true;
            }

            switch (barDirection)
            {
                case HelperMethods.Direction.Up:
                    fillImg.rectTransform.offsetMax = new Vector2(fillImg.rectTransform.offsetMax.x, -1 * hpRatio * maxFillSize);
                    break;

                case HelperMethods.Direction.Down:
                    fillImg.rectTransform.offsetMin = new Vector2(fillImg.rectTransform.offsetMin.x, hpRatio * maxFillSize);
                    break;

                case HelperMethods.Direction.Left:
                    fillImg.rectTransform.offsetMin = new Vector2(hpRatio * maxFillSize, fillImg.rectTransform.offsetMin.y);
                    break;

                case HelperMethods.Direction.Right:
                    fillImg.rectTransform.offsetMax = new Vector2(-1 * hpRatio * maxFillSize, fillImg.rectTransform.offsetMax.y);
                    break;
            }
        }
    }

    public void ResetHPBar()
    {
        fillImg.enabled = true;
    }
}
