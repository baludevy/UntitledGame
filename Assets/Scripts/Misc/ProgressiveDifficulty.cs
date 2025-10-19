using System;
using UnityEngine;

public class ProgressiveDifficulty : MonoBehaviour
{
    public static int enemiesPerNight = 4;
    private int lastDay = 0;

    private void Update()
    {
        int currentDay = DayNightCycle.Instance.currentDay;

        if (currentDay != lastDay)
        {
            enemiesPerNight += GetIncrementForDay(currentDay);
            lastDay = currentDay;
            Debug.Log(enemiesPerNight);
        }
    }

    private int GetIncrementForDay(int day)
    {
        switch (day)
        {
            case 1: return 1;
            case 2: return 2;
            case 3: return 4;
            case 4: return 6;
            case 5: return 10;
            default:
                return Mathf.RoundToInt(10 * Mathf.Pow(1.2f, day - 5));
        }
    }
}