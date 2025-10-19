using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public float health = 100;
    public float stamina = 100f;
    
    public float staminaLoss = 20;
    public float jumpStaminaLoss = 5;
    public float staminaRegen = 5;
    
    public static PlayerStatistics Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}