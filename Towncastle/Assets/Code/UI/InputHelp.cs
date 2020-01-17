using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHelp : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private GameObject basicHelp;

    [SerializeField]
    private GameObject advancedHelp;

#pragma warning restore 0649

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        ActivateBasic();
    }

    public void ActivateBasic()
    {
        basicHelp.gameObject.SetActive(true);
        advancedHelp.gameObject.SetActive(false);
    }

    public void ActivateAdvanced()
    {
        basicHelp.gameObject.SetActive(false);
        advancedHelp.gameObject.SetActive(true);
    }
}
