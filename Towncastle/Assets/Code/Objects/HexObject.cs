using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexObject : LevelObject
{
    private ObjectPlacer objPlacer;

    public void Init(ObjectPlacer objPlacer)
    {
        this.objPlacer = objPlacer;
    }

    public override void ResetObject()
    {
        base.ResetObject();
    }
}
