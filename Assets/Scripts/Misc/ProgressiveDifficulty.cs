using System;
using UnityEngine;

public class ProgressiveDifficulty : MonoBehaviour
{
    public static int enemiesPerNight = 5;
    public int lastDay;

    private void OnEnable()
    {
        DayNightCycle.Instance.OnDay += IncreaseDifficulty;
    }
    
    private void OnDisable()
    {
        DayNightCycle.Instance.OnDay -= IncreaseDifficulty;
    }
    
    private void IncreaseDifficulty()
    {
        int currentDay = DayNightCycle.CurrentDay;

        if (currentDay != lastDay)
        {
            switch (currentDay)
            {
                case 1: enemiesPerNight = 6; break;
                case 2: enemiesPerNight = 10; break;
                case 3: enemiesPerNight = 12; break;
                case 4: enemiesPerNight = 17; break; 
                case 5: enemiesPerNight = 20; break;
                default:
                    enemiesPerNight = Mathf.RoundToInt(10 * Mathf.Pow(1.2f, currentDay - 5));
                    break;
            }
            
            lastDay = currentDay;
            Debug.Log(enemiesPerNight);
        }
    }
}