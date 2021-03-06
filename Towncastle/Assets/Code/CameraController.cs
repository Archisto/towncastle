﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, IPublicFloat
{
#pragma warning disable 0649

    [SerializeField]
    private Transform orbitPoint;

    [SerializeField]
    private float zoomSpeed = 1f;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private float initialZoomRatio = 1f;

    [SerializeField]
    private float minRadius = 10f;

    [SerializeField]
    private float maxRadius;

    [SerializeField]
    private float minY = 10f;

    [SerializeField]
    private float maxY;

    [SerializeField]
    private float orbitAngle = Mathf.PI; // radians

#pragma warning restore 0649

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 leveledOrbitPoint;
    private float zoomRatio;
    private float orbitRadius;

    private float startOrbitAngle;

    /// <summary>
    /// The orbit angle (in radians).
    /// </summary>
    public float FloatValue
    {
        get => orbitAngle;
        set => orbitAngle = value;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (orbitPoint == null)
            throw new MissingReferenceException("Orbit point");

        leveledOrbitPoint = orbitPoint.position;
        leveledOrbitPoint.y = transform.position.y;
        orbitRadius = Vector3.Distance(transform.position, leveledOrbitPoint);

        if (minY < 0 || minY < orbitPoint.position.y)
            minY = orbitPoint.position.y + 1;

        if (maxY <= minY)
            maxY = transform.position.y;

        if (minRadius < 0)
            minRadius = orbitRadius - 1;

        if (maxRadius <= minRadius)
            maxRadius = orbitRadius;

        zoomRatio = initialZoomRatio;

        startPosition = transform.position;
        startRotation = transform.rotation;
        startOrbitAngle = orbitAngle;

        UpdateZoom();
        UpdatePosition();
        LookAt(orbitPoint.position);
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
            case Utils.Direction.Up:
                Zoom(-1 * speedMultiplier);
                break;
            case Utils.Direction.Down:
                Zoom(speedMultiplier);
                break;
            case Utils.Direction.Left:
                Orbit(speedMultiplier);
                break;
            case Utils.Direction.Right:
                Orbit(-1 * speedMultiplier);
                break;
        }

        LookAt(orbitPoint.position);
    }

    private void Zoom(float speedMultiplier)
    {
        zoomRatio += speedMultiplier * zoomSpeed * Time.deltaTime;
        zoomRatio = Mathf.Clamp01(zoomRatio);
        UpdateZoom();
        UpdatePosition();
    }

    private void Orbit(float speedMultiplier)
    {
        orbitAngle += speedMultiplier * rotationSpeed * Time.deltaTime;

        if (speedMultiplier > 0 && orbitAngle > 2 * Mathf.PI)
            orbitAngle -= 2 * Mathf.PI;
        else if (speedMultiplier < 0 && orbitAngle < 0)
            orbitAngle += 2 * Mathf.PI;

        UpdatePosition();
    }

    private void UpdateZoom()
    {
        orbitRadius = Utils.ValueFromRatio(zoomRatio, minRadius, maxRadius);
        leveledOrbitPoint.y = Utils.ValueFromRatio(zoomRatio, minY, maxY);
    }

    private void UpdatePosition()
    {
        transform.position =
            leveledOrbitPoint
            + Utils.GetHorizontalOrbitDirection(orbitAngle) * orbitRadius;
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

    public void ResetCamera()
    {
        orbitAngle = startOrbitAngle;
        zoomRatio = initialZoomRatio;
        UpdateZoom();
        UpdatePosition();
        LookAt(orbitPoint.position);
    }
}
