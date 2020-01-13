using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField]
    private Transform orbitPoint;

    [SerializeField]
    private float speed = 1f;

#pragma warning restore 0649

    private Vector3 startPosition;
    private Quaternion startRotation;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
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

    public void Move(Utils.Direction direction)
    {
        Vector3 movement = Vector3.zero;

        switch (direction)
        {
            case Utils.Direction.Left:
                movement = Vector3.left;
                break;
            case Utils.Direction.Right:
                movement = Vector3.right;
                break;
        }

        movement = movement * speed * Time.deltaTime;
        transform.position = transform.position + movement;
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
