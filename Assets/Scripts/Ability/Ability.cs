using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public float cooldown;
    public abstract bool Activate();
}