using EZCameraShake;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health = 100f;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float regenDelay = 10f;
    [SerializeField] private float regenRate = 5f;

    private float regenTimer;

    private void Update()
    {
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (health < maxHealth)
        {
            health += regenRate * Time.deltaTime;
            if (health > maxHealth) health = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(health - amount, 0f);
        regenTimer = regenDelay;

        CameraShaker.Instance.ShakeOnce(2f, 2.5f, 0.2f, 0.2f);

        if (health <= 0f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        if (health > maxHealth) health = maxHealth;
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
    
    public float GetHealth() => health;
    public float GetMaxHealth() => maxHealth;
    public float GetRegenDelay() => regenDelay;
    public float GetRegenRate() => regenRate;
}