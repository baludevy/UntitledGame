using UnityEngine;

public class CampfireController : MonoBehaviour
{
    public Campfire campfire;
    public float proximityRadius = 10f;
    public float damageInterval = 5f;

    private float damageTimer;
    private bool isExposed;

    public static float timerCap = 30f;
    public static CampfireController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (DayNightCycle.Instance == null || PlayerUIManager.Instance == null || PlayerStatistics.Instance == null)
            return;

        bool isNight = DayNightCycle.Instance.IsNight();

        bool nearCampfire = false;

        if (campfire != null)
        {
            float dist = Vector3.Distance(PlayerMovement.Instance.transform.position, campfire.transform.position);
            if (dist <= proximityRadius && campfire.lit)
                nearCampfire = true;
            
            Debug.Log(dist);
        }
        else
        {
            nearCampfire = false;
        }

        if (isNight && !nearCampfire)
        {
            if (!isExposed)
            {
                PlayerUIManager.Instance.StartPeriodicFlash(Color.cyan, damageInterval);
                isExposed = true;
                damageTimer = 0f;
            }

            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                PlayerStatistics.Instance.TakeDamage(5);
                damageTimer = 0f;
            }
        }
        else
        {
            if (isExposed)
            {
                PlayerUIManager.Instance.StopPeriodicFlash();
                isExposed = false;
                damageTimer = 0f;
            }
        }
    }
}