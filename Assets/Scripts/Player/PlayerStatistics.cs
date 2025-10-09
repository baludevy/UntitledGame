using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public float health = 100;
    public float stamina = 100f;
    public float hunger = 100f;
    
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