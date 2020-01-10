using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
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

    /// <summary>
    /// Hexagon side directions.
    /// </summary>
    public enum HexDirection
    {
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        None
    }

    /// <summary>
    /// Axis.
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// Returns string "{objectName} is not set."
    /// </summary>
    /// <param name="obj">An object</param>
    /// <returns>A string</returns>
    public static string GetFieldNullString(string obj)
    {
        return string.Format("{0} is not set.", obj);
    }

    /// <summary>
    /// Returns string
    /// "An instance of {objectName} could not be found in the scene."
    /// </summary>
    /// <param name="obj">An object</param>
    /// <returns>A string</returns>
    public static string GetObjectMissingString(string obj)
    {
        return string.Format("An instance of {0} could not be found in the scene.", obj);
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
