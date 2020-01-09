using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
    [SerializeField]
    private bool showGizmos;

    protected bool isDestroyed;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitObject();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (!isDestroyed)
        {
            UpdateObject();
        }
    }

    /// <summary>
    /// Initializes the object.
    /// </summary>
    protected virtual void InitObject() { }

    /// <summary>
    /// Updates the object.
    /// </summary>
    protected virtual void UpdateObject() { }

    /// <summary>
    /// Destroys the object.
    /// </summary>
    public virtual void DestroyObject()
    {
        isDestroyed = true;
    }

    /// <summary>
    /// Resets the object.
    /// </summary>
    public virtual void ResetObject()
    {
        isDestroyed = false;
    }

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        DrawGizmos();
    }

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    protected virtual void DrawGizmos()
    {
        if (!isDestroyed)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.5f);
    }
}
