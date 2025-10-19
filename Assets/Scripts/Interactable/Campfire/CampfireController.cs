using UnityEngine;

public class CampfireController : MonoBehaviour
{
    public Campfire campfire;
    public float proximityRadius = 10f;
    public float damageInterval = 5f;
    public float gracePeriod = 10f;
    public static float timerCap = 30f;

    private float campfireTimer;
    private float damageTimer;
    private float graceTimer;
    private bool isExposed;
    private bool wasNight;

    public static CampfireController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        campfireTimer = timerCap;
    }

    private void Update()
    {
        if (!DayNightCycle.Instance || !PlayerUIManager.Instance || !PlayerStatistics.Instance)
            return;

        bool isNight = DayNightCycle.Instance.IsNight();

        if (isNight && !wasNight)
        {
            ResetCampfireTimer();
            graceTimer = gracePeriod;
        }

        if (campfire)
        {
            if (campfire.lit)
            {
                if (campfireTimer > 0)
                    campfireTimer -= Time.deltaTime;
                else
                {
                    PlayerStatistics.Instance.health -= 100f;
                    campfire.Extinguish();
                }

                campfire.UpdateUI(campfireTimer, timerCap);
            }

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
            if (graceTimer > 0)
            {
                graceTimer -= Time.deltaTime;
            }
            else
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

    public void AddCampfireTime(float amount)
    {
        campfireTimer += amount;
        if (campfireTimer > timerCap) campfireTimer = timerCap;
    }

    public void ResetCampfireTimer()
    {
        campfireTimer = timerCap;
    }

    public float GetCampfireTimer()
    {
        return campfireTimer;
    }
}
