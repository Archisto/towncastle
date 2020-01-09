using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    /// <summary>
    /// Cardinal direction.
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        None
    }

    public static Vector3 VectorFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            case Direction.Forward:
                return Vector3.forward;
            case Direction.Back:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }
}
