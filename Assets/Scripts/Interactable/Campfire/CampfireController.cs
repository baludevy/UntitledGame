using UnityEngine;

public class CampfireController : MonoBehaviour
{
    public Campfire campfire;
    public float proximityRadius = 10f;
    public float damageInterval = 5f;

    private float damageTimer;
    private bool isExposed;
    private bool wasNight;

    public static float timerCap = 30f;
    public static CampfireController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!DayNightCycle.Instance || !PlayerUIManager.Instance || !PlayerStatistics.Instance)
            return;

        bool isNight = DayNightCycle.Instance.IsNight();

        if (campfire)
        {
            if (!isNight && wasNight)
            {
                campfire.Extinguish();
                campfire.wasLit = false;
            }
        }

        bool nearCampfire = campfire && campfire.lit &&
                            Vector3.Distance(PlayerMovement.Instance.transform.position, campfire.transform.position) <=
                            proximityRadius;

        if (isNight)
        {
            if (!nearCampfire)
            {
                if (!isExposed)
                {
                    PlayerUIManager.Instance.StartPeriodicFlash(Color.cyan, damageInterval);
                    PlayerStatistics.Instance.TakeDamage(5, false);
                    isExposed = true;
                    damageTimer = 0f;
                }

                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    PlayerStatistics.Instance.TakeDamage(5, false);
                    damageTimer = 0f;
                }
            }
            else if (isExposed)
            {
                PlayerUIManager.Instance.StopPeriodicFlash();
                isExposed = false;
                damageTimer = 0f;
            }
        }
        else
        {
            if (isExposed)
            {
                PlayerUIManager.Instance.StopPeriodicFlash();
                isExposed = false;
            }

            damageTimer = 0f;
        }

        wasNight = isNight;
    }
}