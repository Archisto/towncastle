using UnityEngine;

public class MovingItem : Item
{
    public AnimationScriptableObject idleAnim;
    public AnimationScriptableObject collectAnim;

    protected override void InitObject()
    {
        base.InitObject();

        animMover = GetComponent<AnimationMover>();
        animMover.anim = idleAnim;
        animMover.Play(transform);
    }

    protected override void UpdateObject()
    {
        base.UpdateObject();

        if (!isCollected && animMover.IsFinished)
        {
            animMover.ResetAnimation(false, false);
            animMover.Play(transform);
        }
    }

    protected override void Collect(PlayerController player)
    {
        if (animMover != null)
        {
            animMover.ResetAnimation(true, true);
            animMover.anim = collectAnim;
        }

        base.Collect(player);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        animMover.anim = idleAnim;
        animMover.Play(transform);
    }

    protected override void DrawGizmos()
    {
        if (animMover == null || !animMover.IsPlaying)
            return;

        if (animMover.Progress < 0.9f)
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * animMover.Progress * 1.5f);
    }
}
