using System;
using UnityEngine;

public class CampfireController : MonoBehaviour
{
    public Campfire campfire;

    public static float timerCap = 45f;
    
    public static CampfireController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}