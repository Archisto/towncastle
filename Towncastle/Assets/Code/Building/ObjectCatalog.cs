using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class ObjectCatalog : MonoBehaviour
{
    private const int FavoriteHexMeshCount = 10;

#pragma warning disable 0649

    [SerializeField]
    private HexMeshScriptableObject[] hexMeshes;

#pragma warning restore 0649

    private int[] favoriteHexMeshIndices;

    public int HexMeshCount
    {
        get
        {
            if (hexMeshes == null)
                return 0;
            else
                return hexMeshes.Length;
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitFavorites();
    }

    /// <summary>
    /// Initializes the favorite hex mesh indices array.
    /// By default, the 10 favorites are the first 10 hex meshes.
    /// </summary>
    private void InitFavorites()
    {
        favoriteHexMeshIndices = new int[FavoriteHexMeshCount];
        int favoriteIndex;
        for (int i = 0; i < favoriteHexMeshIndices.Length; i++)
        {
            // Fav indices are organized like the alphanumeric num keys on the keyboard
            favoriteIndex = (i < favoriteHexMeshIndices.Length - 1 ? i + 1 : 0);
            favoriteHexMeshIndices[favoriteIndex] = i;
        }
    }

    public HexMeshScriptableObject GetHexMesh(int index)
    {
        if (index >= 0 && index < HexMeshCount)
            return hexMeshes[index];

        return null;
    }

    public HexMeshScriptableObject GetFavoriteHexMesh(int favoriteIndex)
    {
        if (favoriteIndex >= 0 && favoriteIndex < FavoriteHexMeshCount)
            return hexMeshes[favoriteHexMeshIndices[favoriteIndex]];

        return null;
    }

    public int GetFavoriteHexMeshFullCatalogIndex(int favoriteIndex)
    {
        if (favoriteIndex < 0 || favoriteIndex >= FavoriteHexMeshCount)
            return -1;

        return favoriteHexMeshIndices[favoriteIndex];
    }

    public bool SaveCurrentHexMeshToFavorites(int favoriteIndex, int catalogIndex)
    {
        if (favoriteIndex < 0 && favoriteIndex >= FavoriteHexMeshCount)
            return false;

        favoriteHexMeshIndices[favoriteIndex] = catalogIndex;

        return true;
    }

    public HexMeshScriptableObject GetRandomHexMesh()
    {
        return GetHexMesh(Random.Range(0, HexMeshCount));
    }

    public void Sort()
    {
        SortByType();
    }

    public void SortByType()
    {
        /*
        0. Floor = 1,
        1. Wall = 4,
        2. Room = 0,
        3. Roof = 6,
        4. Support = 3,
        5. Protrusion = 5,
        6. Object = 2,
        7. Undefined = 7
        */

        int typeCount = Utils.GetEnumLength(typeof(HexObject.StructureType));

        if (typeCount <= 0)
            return;

        HexMeshScriptableObject hexMesh;
        int hexMeshIndex = 0;
        int[] priority = new int[] { 2, 0, 6, 4, 1, 5, 3, 7 };

        for (int i = 0; i < typeCount; i++)
        {
            HexObject.StructureType type = (HexObject.StructureType)priority[i];
            //HexObject.StructureType type = (HexObject.StructureType)i;
            for (int j = hexMeshIndex; j < HexMeshCount; j++)
            {
                if (hexMeshes[j].structureType == type)
                {
                    hexMesh = hexMeshes[j];
                    hexMeshes[j] = hexMeshes[hexMeshIndex];
                    hexMeshes[hexMeshIndex] = hexMesh;
                    hexMeshIndex++;
                }
            }
        }
    }

    public void SortRandom()
    {
        HexMeshScriptableObject hexMesh;
        for (int i = 0; i < HexMeshCount; i++)
        {
            hexMesh = hexMeshes[i];
            int randomIndex = Random.Range(0, HexMeshCount);
            hexMeshes[i] = hexMeshes[randomIndex];
            hexMeshes[randomIndex] = hexMesh;
        }
    }
}
