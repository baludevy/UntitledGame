using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject hitEffect;
    public GameObject damageMarker;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
    }
}