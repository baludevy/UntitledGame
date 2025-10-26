using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    private float xp;
    private float xpToNextLevel = 50f;
    private int level = 1;

    public void GainXP(float amount)
    {
        xp += amount;
        while (xp >= xpToNextLevel)
            LevelUp();
    }

    private void LevelUp()
    {
        xp -= xpToNextLevel;
        xpToNextLevel = Mathf.Floor(xpToNextLevel * 1.2f);
        level += 1;
    }

    public float GetXP() => xp;
    public float GetXPNeededForNextLevel() => xpToNextLevel;
    public int GetLevel() => level;
}