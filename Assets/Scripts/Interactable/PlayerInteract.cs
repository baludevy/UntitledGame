using System;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool blocked;
    public static PlayerInteract Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}