using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : LevelObject
{
    protected AnimationMover animMover;
    protected bool isCollected;

    private MeshRenderer rend;

    protected override void InitObject()
    {
        base.InitObject();

        rend = GetComponent<MeshRenderer>();
        animMover = GetComponent<AnimationMover>();
    }

    protected override void UpdateObject()
    {
        base.UpdateObject();

        if (isCollected && animMover != null && animMover.IsFinished)
        {
            DestroyObject();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCollected || isDestroyed)
            return;

        Debug.Log("HIT: " + collision.gameObject.name);

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            Collect(player);
        }
    }

    protected virtual void Collect(PlayerController player)
    {
        isCollected = true;

        if (animMover != null)
        {
            animMover.Play(transform);
        }
        else
        {
            DestroyObject();
        }
    }

    public override void DestroyObject()
    {
        base.DestroyObject();

        if (rend != null)
        {
            rend.enabled = false;
        }
    }

    public override void ResetObject()
    {
        base.ResetObject();

        isCollected = false;
        animMover.ResetAnimation(true, true);

        if (rend != null)
        {
            rend.enabled = true;
        }
    }
}
