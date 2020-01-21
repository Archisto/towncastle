using System.Collections;
using UnityEngine;

public class OrbitTransform : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    protected Transform orbitPoint;

    [SerializeField]
    protected bool staticOrbitPoint = true;

    [SerializeField]
    protected bool oppositeSide;

    [SerializeField]
    protected bool move = true;

    [SerializeField]
    protected bool rotate = true;

#pragma warning restore 0649

    protected Vector3 leveledOrbitPoint;
    protected float orbitRadius;
    protected Vector3 startRotation;

    /// <summary>
    /// The orbit angle in radians.
    /// </summary>
    public virtual float OrbitAngle { get; set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected virtual void Start()
    {
        if (orbitPoint == null)
            throw new MissingReferenceException("Orbit point");

        leveledOrbitPoint = orbitPoint.position;
        leveledOrbitPoint.y = transform.position.y;
        orbitRadius = Vector3.Distance(transform.position, leveledOrbitPoint);
        startRotation = transform.rotation.eulerAngles;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (move)
            OrbitY();

        if (rotate)
            RotateY();
    }

    protected void OrbitY()
    {
        if (orbitPoint == null)
            return;

        Vector3 origin;
        if (staticOrbitPoint)
        {
            origin = leveledOrbitPoint;
        }
        else
        {
            origin = orbitPoint.position;
            origin.y = transform.position.y;
        }

        transform.position = origin +
            Utils.GetHorizontalOrbitDirection
                (OrbitAngle + (oppositeSide ? Mathf.PI : 0)) * orbitRadius;
    }

    protected void RotateY()
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.y = startRotation.y + 360 * (OrbitAngle / (2 * Mathf.PI)) + (oppositeSide ? 0 : 180);
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
