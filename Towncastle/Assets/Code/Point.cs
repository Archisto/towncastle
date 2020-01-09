using UnityEngine;

public class Point : MonoBehaviour
{
    /// <summary>
    /// The gizmo color.
    /// </summary>
    [SerializeField, Tooltip("The gizmo color.")]
    private Color color = Color.black;

    /// <summary>
    /// The coordinates.
    /// </summary>
    public Vector3 Coordinates { get { return transform.position; } }

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(Coordinates + Vector3.up * 0.5f, Coordinates + Vector3.up * -0.5f);
        Gizmos.DrawLine(Coordinates + Vector3.right * 0.5f, Coordinates + Vector3.right * -0.5f);
    }
}
