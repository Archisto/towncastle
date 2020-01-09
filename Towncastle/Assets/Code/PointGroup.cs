using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGroup : MonoBehaviour
{
    /// <summary>
    /// The points.
    /// </summary>
    [Tooltip("The points.")]
    public List<Point> points;

    /// <summary>
    /// The gizmo color.
    /// </summary>
    [SerializeField, Tooltip("The gizmo color.")]
    private Color color = Color.black;

    /// <summary>
    /// Draws gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (points != null && points.Count >= 2)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i].Coordinates, points[i + 1].Coordinates);
            }
        }
    }
}
