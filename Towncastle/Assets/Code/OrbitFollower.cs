using System.Collections;
using UnityEngine;

public class OrbitFollower : OrbitTransform
{
#pragma warning disable 0649

    [SerializeField]
    private GameObject orbitObject;

#pragma warning restore 0649

    private IPublicFloat publicFloatObject;

    /// <summary>
    /// The orbit angle in radians.
    /// </summary>
    public override float OrbitAngle { get => GetOrbitAngleFromOrbitObject(); }

    private float GetOrbitAngleFromOrbitObject()
    {
        if (publicFloatObject != null)
            return publicFloatObject.FloatValue;
        else
            return 0;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        if (orbitObject == null)
            throw new MissingReferenceException("Orbit object");

        publicFloatObject = orbitObject.GetComponent<IPublicFloat>();
    }
}
