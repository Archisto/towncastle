using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private Transform orbitPoint;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private float orbitAngle = Mathf.PI; // radians

#pragma warning restore 0649

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 leveledOrbitPoint;
    private float orbitRadius;
    //private float orbitAngle;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (orbitPoint != null)
        {
            leveledOrbitPoint = orbitPoint.position;
            leveledOrbitPoint.y = transform.position.y;
            orbitRadius = Vector3.Distance(transform.position, leveledOrbitPoint);
            LookAt(orbitPoint.position);
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    /// <summary>
    /// LateUpdate is called once per frame, after Update.
    /// </summary>
    private void LateUpdate()
    {
        if (GameManager.Instance.PlayReady)
        {
            // TODO
        }
    }

    /// <summary>
    /// Makes the camera match the position and rotation of the given transform.
    /// </summary>
    /// <param name="t">The transform</param>
    public void GoTo(Transform t)
    {
        transform.position = t.position;
        transform.rotation = t.rotation;
    }

    public void Move(Utils.Direction direction, float speedMultiplier)
    {
        if (orbitPoint == null)
            return;

        switch (direction)
        {
            case Utils.Direction.Left:
                orbitAngle += rotationSpeed * speedMultiplier * Time.deltaTime;
                if (orbitAngle > 0)
                    orbitAngle -= 2 * Mathf.PI;
                break;
            case Utils.Direction.Right:
                orbitAngle += -1 * rotationSpeed * speedMultiplier * Time.deltaTime;
                if (orbitAngle < 2 * Mathf.PI)
                    orbitAngle += 2 * Mathf.PI;
                break;
        }

        Vector3 orbitDirection = new Vector3(Mathf.Sin(orbitAngle), 0, Mathf.Cos(orbitAngle));
        transform.position = leveledOrbitPoint + orbitDirection * orbitRadius;

        LookAt(orbitPoint.position);
    }

    public void LookAt(Vector3 position)
    {
        transform.rotation =
            Quaternion.LookRotation(position - transform.position, Vector3.up);
    }

    private void UpdatePositionBetweenObjects(List<LevelObject> levelObjects)
    {
        Vector3 newPosition = Vector3.zero;
        int intactObjects = 0;

        for (int i = 0; i < levelObjects.Count; i++)
        {
            if (!levelObjects[i].IsDestroyed)
            {
                newPosition += levelObjects[i].transform.position;
                intactObjects++;
            }
        }

        if (intactObjects > 0)
        {
            newPosition = newPosition / intactObjects;
            newPosition.y = startPosition.y;
            transform.position = newPosition;
        }
    }

    public Vector3 GetCameraViewPosition(Vector3 camPosOffset)
    {
        Vector3 worldOffset = transform.right * camPosOffset.x +
                              transform.up * camPosOffset.y +
                              transform.forward * camPosOffset.z;
        return transform.position + worldOffset;
    }

    public Quaternion GetRotationTowardsCamera()
    {
        // TODO: Quaternion.Inverse?

        return Quaternion.Euler(
            transform.rotation.eulerAngles.x * -1,
            transform.rotation.eulerAngles.y + 180,
            transform.rotation.eulerAngles.z * -1);
    }
}
