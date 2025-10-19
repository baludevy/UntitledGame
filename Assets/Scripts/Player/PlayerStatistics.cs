using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void TakeDamage(float damage, bool flash = true)
    {
        health -= damage;

        if (flash)
            PlayerUIManager.Instance.Flash(new Color(0.8f, 0f, 0f), 0.3f);
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}