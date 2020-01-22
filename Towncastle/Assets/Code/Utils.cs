using System;
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

    /// <summary>
    /// Gets how many values does an enumerator have.
    /// </summary>
    /// <param name="enumType">An enum type</param>
    /// <returns>The length of the enum; -1 if not an enum</returns>
    public static int GetEnumLength(Type enumType)
    {
        if (enumType.IsEnum)
            return Enum.GetValues(enumType).Length;
        else
            return -1;
    }

    public static bool CanBeRoundedUp(float f)
    {
        return (int)f < (int)(f + 0.5f);
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

    public static float AngleFromHexDirection(HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.Left:
                return 180;
            case HexDirection.Right:
                return 0;
            case HexDirection.UpLeft:
                return -120;
            case HexDirection.UpRight:
                return -60;
            case HexDirection.DownLeft:
                return 120;
            case HexDirection.DownRight:
                return 60;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Gets the angle between two hex directions.
    /// Right is the world main direction so its angle is 0.
    /// </summary>
    /// <param name="direction1">A hex direction</param>
    /// <param name="direction2">Another hex direction</param>
    /// <returns>An angle in degrees</returns>
    public static float AngleFromHexDirectionToAnother(HexDirection direction1, HexDirection direction2)
    {
        return AngleFromHexDirection(direction1) - AngleFromHexDirection(direction2);
    }

    public static Vector3 GetHorizontalOrbitDirection(float angle)
    {
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    public static Vector3 GetCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // Bézier curve:
        // B(t) = (1 - t) * [(1 - t) * p0 + t * p1] + t * [(1 - t) * p1 + t * p2]
        // 0 <= t <= 1

        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    public static float Ratio(float value, float lowBound, float highBound)
    {
        if (value <= lowBound)
            return 0;
        else if (value >= highBound)
            return 1;
        else
            return ((value - lowBound) / (highBound - lowBound));
    }

    public static float ValueFromRatio(float ratio, float lowBound, float highBound)
    {
        if (ratio <= 0f)
            return lowBound;
        else if (ratio >= 1f)
            return highBound;
        else
            return lowBound + ratio * (highBound - lowBound);
    }
}
