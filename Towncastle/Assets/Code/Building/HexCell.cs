using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : IGridObject
{
    private List<HexObject>[] objectList;

    private int totalObjectCount;

    public Vector2Int Coordinates { get; set; }

    public float HeightLevel { get => 1; set { } }

    public bool IsEmpty
    {
        get => totalObjectCount == 0;
    }

    public int MaxHeightLevel { get; private set; }

    public HexCell(int x, int y, int maxHeightLevel)
    {
        Coordinates = new Vector2Int(x, y);
        MaxHeightLevel = maxHeightLevel;
        InitObjectList();
    }

    private void InitObjectList()
    {
        objectList = new List<HexObject>[MaxHeightLevel];
        for (int i = 0; i < objectList.Length; i++)
        {
            objectList[i] = new List<HexObject>();
        }
    }

    public int GetObjectCount(int heightLevelRounded = 0)
    {
        if (heightLevelRounded <= 0)
        {
            return totalObjectCount;
        }
        else if (heightLevelRounded <= objectList.Length)
        {
            return objectList[heightLevelRounded - 1].Count;
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    public bool HasSomethingOnHeightLevel(int heightLevel)
    {
        return objectList[heightLevel - 1].Count > 0;
    }

    public bool Has(HexObject hexObject)
    {
        if (IsEmpty || hexObject == null)
            return false;

        foreach (List<HexObject> list in objectList)
        {
            foreach (HexObject obj in list)
            {
                if (obj == hexObject)
                    return true;
            }
        }

        return false;
    }

    public HexObject GetAny(bool acceptHidden)
    {
        foreach (List<HexObject> list in objectList)
        {
            foreach (HexObject obj in list)
            {
                if (obj != null && (acceptHidden || !obj.Hidden))
                    return obj;
            }
        }

        return null;
    }

    public List<HexObject> GetAll(bool acceptHidden)
    {
        List<HexObject> hexObjectList = new List<HexObject>();

        foreach (List<HexObject> list in objectList)
        {
            foreach (HexObject obj in list)
            {
                if (obj != null && (acceptHidden || !obj.Hidden))
                    hexObjectList.Add(obj);
            }
        }

        return hexObjectList;
    }

    /// <summary>
    /// Gets the highest occupied height level.
    /// Returns 0 if the cell is empty.
    /// </summary>
    /// <returns>Highest occupied height level; 0 if empty</returns>
    public float GetHighestOccupiedHeightLevel()
    {
        if (IsEmpty)
            return 0;

        float result = 1;

        // Iterates from the highest height level down
        for (int i = objectList.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < objectList[i].Count; j++)
            {
                if (objectList[i][j].HeightLevel > result)
                {
                    result = objectList[i][j].HeightLevel;
                }
            }

            // The next iteration would be a full height level lower
            // so there's no need to continue if the result has been increased
            // from the default
            if (result > 1)
                break;
        }

        return result;
    }

    public bool PlaceObject(HexObject hexObject, int heightLevelRounded)
    {
        if (hexObject == null
            || heightLevelRounded < 1
            || heightLevelRounded > objectList.Length)
            return false;

        objectList[heightLevelRounded - 1].Add(hexObject);
        totalObjectCount++;
        return true;
    }

    public bool ActionToObjectsInHeightLevel<T>(Action<HexObject> action, int heightLevelRounded)
    {
        if (IsEmpty)
            return false;

        bool success = false;

        foreach (HexObject obj in objectList[heightLevelRounded - 1])
        {
            if (obj != null)
            {
                action(obj);
                success = true;
            }
        }

        return success;
    }

    public bool ActionToAllObjects<T>(Action<HexObject> action)
    {
        if (IsEmpty)
            return false;

        bool success = false;

        foreach (List<HexObject> list in objectList)
        {
            foreach (HexObject obj in list)
            {
                if (obj != null)
                {
                    action(obj);
                    success = true;
                }
            }
        }

        return success;
    }

    public bool RemoveObject(HexObject hexObject)
    {
        if (IsEmpty || hexObject == null)
            return false;

        for (int i = 0; i < objectList.Length; i++)
        {
            for (int j = 0; j < objectList[i].Count; j++)
            {
                if (hexObject == objectList[i][j])
                {
                    objectList[i].RemoveAt(j);
                    goto Success;
                }
            }
        }

        return false;

    Success:
        hexObject.ResetObject();
        totalObjectCount--;
        return true;
    }

    public bool RemoveObjects(int heightLevelRounded)
    {
        // TODO: Half height level removing

        if (objectList[heightLevelRounded - 1].Count > 0)
        {
            totalObjectCount -= objectList[heightLevelRounded - 1].Count;

            for (int i = 0; i < objectList[heightLevelRounded - 1].Count; i++)
            {
                objectList[heightLevelRounded - 1][i].ResetObject();
            }

            objectList[heightLevelRounded - 1].Clear();
            return true;
        }

        return false;
    }

    public void RemoveAllObjects()
    {
        ActionToAllObjects<HexObject>(hexObj => hexObj.ResetObject());

        for (int i = 0; i < objectList.Length; i++)
        {
            objectList[i].Clear();
        }

        totalObjectCount = 0;
    }
}
