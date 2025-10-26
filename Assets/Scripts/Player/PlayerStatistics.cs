using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static PlayerStatistics Instance;

    public PlayerHealth Health { get; private set; }
    public PlayerStamina Stamina { get; private set; }
    public PlayerExperience Experience { get; private set; }
    public PlayerCombatStats Combat { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Health = GetComponent<PlayerHealth>();
        Stamina = GetComponent<PlayerStamina>();
        Experience = GetComponent<PlayerExperience>();
        Combat = GetComponent<PlayerCombatStats>();
    }
}