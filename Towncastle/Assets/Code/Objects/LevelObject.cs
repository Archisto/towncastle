using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private bool showGizmos;

#pragma warning restore 0649

    public bool IsDestroyed { get; protected set; }

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
        if (!IsDestroyed)
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
        IsDestroyed = true;
    }

    /// <summary>
    /// Resets the object.
    /// </summary>
    public virtual void ResetObject()
    {
        IsDestroyed = false;
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
        if (!IsDestroyed)
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
