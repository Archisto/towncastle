using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : IGridObject
{
    private const int FloorIndex = 0;
    private const int WallIndex = 1;
    private const int RoomIndex = 2;
    private const int ContentIndex = 3;
    private const int RoofIndex = 4;
    private const int InvalidIndex = -1;

    public Vector2Int Coordinates { get; set; }

    public bool IsEmpty { get; private set; }

    public HexObject[] HexObjects { get; private set; }

    public int TopHeightLevel { get; private set; }

    public HexCell(int x, int y)
    {
        Coordinates = new Vector2Int(x, y);
        HexObjects = new HexObject[5];
        IsEmpty = true;
        TopHeightLevel = 1;
    }

    private int GetTypeIndex(HexObject.StructureType type)
    {
        switch (type)
        {
            case HexObject.StructureType.Floor:
                return FloorIndex;
            case HexObject.StructureType.Wall:
                return WallIndex;
            case HexObject.StructureType.Room:
                return RoomIndex;
            case HexObject.StructureType.Support:
            case HexObject.StructureType.Protrusion:
            case HexObject.StructureType.Object:
                return ContentIndex;
            case HexObject.StructureType.Roof:
                return RoofIndex;
            default:
                return InvalidIndex;
        }
    }

    public bool IsAvailableForType(HexObject.StructureType type)
    {
        if (IsEmpty)
            return true;

        // Roof must be the only object in the cell
        if (HexObjects[RoofIndex] != null)
            return false;

        switch (type)
        {
            // Floor cannot be in the same cell with a Room
            case HexObject.StructureType.Floor:
                return HexObjects[FloorIndex] == null
                    && HexObjects[RoomIndex] == null;
            // Wall cannot be in the same cell with a Room
            case HexObject.StructureType.Wall:
                return HexObjects[WallIndex] == null
                    && HexObjects[RoomIndex] == null;
            // Room cannot be in the same cell with either a Floor or a Wall
            case HexObject.StructureType.Room:
                return HexObjects[RoomIndex] == null
                    && HexObjects[FloorIndex] == null
                    && HexObjects[WallIndex] == null;
            case HexObject.StructureType.Support:
            case HexObject.StructureType.Protrusion:
            case HexObject.StructureType.Object:
                return HexObjects[ContentIndex] == null;
            // Roof must be the only object in the cell
            case HexObject.StructureType.Roof:
                return false;
            default:
                return false;
        }
    }

    public bool Has(HexObject hexObject)
    {
        if (IsEmpty || hexObject == null)
            return false;

        return hexObject == Get(hexObject.Type);
    }

    public bool Has(HexObject.StructureType type, bool acceptUnsupportedType)
    {
        if (IsEmpty)
            return false;

        int index = GetTypeIndex(type);
        if (index < 0)
            return acceptUnsupportedType;
        else
            return HexObjects[index] != null;
    }

    public HexObject Get(HexObject.StructureType type)
    {
        if (IsEmpty)
            return null;

        int index = GetTypeIndex(type);
        if (index < 0)
            return null;
        else
            return HexObjects[index];
    }

    public HexObject GetAny(bool acceptHidden)
    {
        for (int i = 0; i < HexObjects.Length; i++)
        {
            if (HexObjects[i] != null && (acceptHidden || !HexObjects[i].Hidden))
                return HexObjects[i];
        }

        return null;
    }

    public List<HexObject> GetAll(bool acceptHidden)
    {
        List<HexObject> hexObjectList = new List<HexObject>(5);

        for (int i = 0; i < HexObjects.Length; i++)
        {
            if (HexObjects[i] != null && (acceptHidden || !HexObjects[i].Hidden))
                hexObjectList.Add(HexObjects[i]);
        }

        return hexObjectList;
    }

    public bool PlaceObject(HexObject hexObject)
    {
        if (hexObject == null)
            return false;

        int index = GetTypeIndex(hexObject.Type);
        if (index < 0)
        {
            Debug.LogError(
                string.Format("Cannot add object to {0}; StructureType {1} not supported.",
                    Coordinates, hexObject.Type));
            return false;
        }
        else
        {
            if (HexObjects[index] == null)
            {
                HexObjects[index] = hexObject;
                IsEmpty = false;
                return true;
            }
            else
            {
                // TODO: Allow any object in any hex? Have to add all of them to a list.

                Debug.LogWarning(string.Format("Cell {0} occupied", Coordinates));
                return false;
            }
        }
    }

    public void ActionToAllObjects<T>(Action<HexObject> action)
    {
        if (IsEmpty)
            return;

        foreach (HexObject hexObj in HexObjects)
        {
            if (hexObj != null)
                action(hexObj);
        }
    }

    private bool CheckIfEmpty()
    {
        foreach (HexObject hexObj in HexObjects)
        {
            if (hexObj != null)
                return false;
        }

        return true;
    }

    public void RemoveObject(HexObject hexObject)
    {
        if (IsEmpty || hexObject == null || !Has(hexObject))
            return;

        hexObject.ResetObject();

        for (int i = 0; i < HexObjects.Length; i++)
        {
            if (hexObject == HexObjects[i])
            {
                HexObjects[i] = null;
                break;
            }
        }

        IsEmpty = CheckIfEmpty();
    }

    public void RemoveObject(HexObject.StructureType type)
    {
        if (IsEmpty)
            return;

        int index = GetTypeIndex(type);
        if (index >= 0 && HexObjects[index] != null)
        {
            HexObjects[index].ResetObject();
            HexObjects[index] = null;
            IsEmpty = CheckIfEmpty();
        }
    }

    public void RemoveAllObjects()
    {
        ActionToAllObjects<HexObject>(hexObj => hexObj.ResetObject());

        for (int i = 0; i < HexObjects.Length; i++)
        {
            HexObjects[i] = null;
        }

        IsEmpty = true;
    }
}
