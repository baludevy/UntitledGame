using System;
using UnityEngine;

public class PlayerPrefabManager: MonoBehaviour
{
    public GameObject damageMarker;
    
    public static PlayerPrefabManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}