using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMover : MonoBehaviour
{
    public AnimationScriptableObject anim;

    private Timer timer;
    private Transform target;
    private Vector3 targetStartPos;
    private Vector3 targetStartRot;
    private Vector3 direction;

    private float rotX;
    private float rotY;
    private float rotZ;

    public bool IsPlaying { get; private set; }

    public bool IsFinished { get; private set; }

    public float Progress { get => timer.Ratio; }

    public void Play(Transform targetTransform)
    {
        if (timer == null)
            timer = new Timer(anim.duration);
        else
            timer.Init(anim.duration);

        target = targetTransform;
        targetStartPos = target.position;
        targetStartRot = target.rotation.eulerAngles;
        InitDirection();
        InitRotation();
        IsPlaying = true;
        timer.Start();
    }

    private void InitDirection()
    {
        direction = anim.direction;

        /*if (anim.randomDirAngle > 0)
        {
            int halfRDA = anim.randomDirAngle / 2;
            direction.x = direction.x + Random.Range(-1 * halfRDA, halfRDA);
            direction.y = direction.y + Random.Range(-1 * halfRDA, halfRDA);
            direction.z = direction.z + Random.Range(-1 * halfRDA, halfRDA);
            
            direction.Normalize();
        }*/
    }
    private void InitRotation()
    {
        rotX = target.rotation.eulerAngles.x;
        rotY = target.rotation.eulerAngles.y;
        rotZ = target.rotation.eulerAngles.z;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (IsPlaying)
        {
            if (timer.Check())
            {
                Finish();
            }

            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        target.position = targetStartPos + direction * Progress * anim.distance;

        if (anim.spinSpeed != 0)
        {
            Vector3 rotChange = anim.spin * anim.spinSpeed;
            rotX += rotChange.x;
            rotY += rotChange.y;
            rotZ += rotChange.z;

            if (rotX > 180) rotX -= 360;
            if (rotX < -180) rotX += 360;
            if (rotY > 180) rotY -= 360;
            if (rotY < -180) rotY += 360;
            if (rotZ > 180) rotZ -= 360;
            if (rotZ < -180) rotZ += 360;

            target.rotation = Quaternion.Euler(rotX, rotY, rotZ);
        }
    }

    private void Finish()
    {
        IsPlaying = false;
        IsFinished = true;
    }

    public void ResetAnimation(bool resetPos, bool resetRot)
    {
        if (IsPlaying || IsFinished)
            timer.ResetTimer();

        IsPlaying = false;
        IsFinished = false;
        
        if (target != null)
        {
            if (resetPos)
                target.position = targetStartPos;
            if (resetRot)
                target.rotation = Quaternion.Euler(targetStartRot);
        }
    }
}
