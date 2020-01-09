using UnityEngine;

[CreateAssetMenu(fileName = "Animation", menuName = "Scriptable Objects/Animation", order = 1)]
public class AnimationScriptableObject : ScriptableObject
{
    public float duration;
    public float distance;
    public float spinSpeed;
    public int randomDirAngle;
    public Vector3 direction;
    public Vector3 spin;
    public bool dissolve;
}