using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public float cooldown = 1f;
    public List<Ability> opposingAbilities = new List<Ability>();
    public abstract bool Activate();
    public abstract void Cancel();

}