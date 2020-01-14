using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlacerObject : MonoBehaviour
{
    // Child object and its components
    private GameObject childObj;
    private MeshRenderer meshRend;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (childObj == null)
            childObj = transform.GetChild(0).gameObject;

        if (meshRend == null)
            meshRend = childObj.GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material)
    {
        if (childObj == null)
            childObj = transform.GetChild(0).gameObject;

        if (meshRend == null)
            meshRend = childObj.GetComponent<MeshRenderer>();

        meshRend.material = material;
    }

    public void Hide(bool hide)
    {
        meshRend.enabled = hide;
    }
}
