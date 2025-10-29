using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public float cooldown = 1f;
    public abstract bool Activate();
}