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

    public void ActionToAllObjects<T>(Action<HexObject> action)
    {
        if (IsEmpty)
            return;

        foreach (List<HexObject> list in objectList)
        {
            foreach (HexObject obj in list)
            {
                if (obj != null)
                    action(obj);
            }
        }
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
