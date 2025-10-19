using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatistics : MonoBehaviour
{
    public float health = 100;
    public float stamina = 100f;

    public float staminaLoss = 20;
    public float jumpStaminaLoss = 5;
    public float staminaRegen = 5;

    public float regenDelay = 15f;
    public float regenInterval = 3f;
    public float regenAmount = 3f;

    private float timeSinceLastDamage;
    private float regenTimer;

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

    private void Update()
    {
        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= regenDelay && health < 100)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                health += regenAmount;
                if (health > 100) health = 100;
                regenTimer = 0f;
            }
        }
    }

    public void TakeDamage(float damage, bool flash = true)
    {
        health -= damage;
        timeSinceLastDamage = 0f;
        regenTimer = 0f;

        if (flash)
            PlayerUIManager.Instance.Flash(new Color(0.8f, 0f, 0f), 0.3f);
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}