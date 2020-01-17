using UnityEngine;

public interface IGridObject
{
    Vector2Int Coordinates { get; set; }

    float HeightLevel { get; set; }
}
