using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sunLight;

    [Header("Day-Night cycle settings")] 
    [SerializeField] private float startTime;
    [SerializeField] private float speed;
    [Tooltip("In Minutes")] [SerializeField] private float cycleDuration;

    [Header("Lighting settings")] 
    [SerializeField] private float lightIntensityNight = 0f;
    [SerializeField] private float lightIntensityDay = 1f;
    [SerializeField] private float sunriseAngle = -90f;
    [SerializeField] private float sunsetAngle = 90f;

    private float currentTimeOfDay;
    private float timeOfDayNormalized;
    private bool nightSpawnActive;
    public int currentDay;
    private float lastTimeOfDay;

    public static DayNightCycle Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentTimeOfDay = startTime * 60;
        currentDay = 0;
        lastTimeOfDay = 0f;
    }

    public void Update()
    {
        currentTimeOfDay += Time.deltaTime * speed;
        float cycleDurationSeconds = cycleDuration * 60;
        timeOfDayNormalized = currentTimeOfDay % cycleDurationSeconds / cycleDurationSeconds;

        float sunAngle = timeOfDayNormalized * 360f;
        sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 50f, 0f);

        float intensityFactor = Mathf.Clamp01(Mathf.Sin(timeOfDayNormalized * Mathf.PI * 2f));
        sunLight.intensity = Mathf.Lerp(lightIntensityNight, lightIntensityDay, intensityFactor);

        if (IsNight() && !nightSpawnActive)
        {
            float sessionDuration = cycleDurationSeconds / 2f;

            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.StartSpawn(sessionDuration, ProgressiveDifficulty.enemiesPerNight);
                nightSpawnActive = true;
            }
        }
        else if (!IsNight() && nightSpawnActive)
        {
            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.StopSpawn();

            nightSpawnActive = false;
        }
        
        if (timeOfDayNormalized < lastTimeOfDay)
        {
            currentDay++;
            Debug.Log("new day " + currentDay);
        }

        lastTimeOfDay = timeOfDayNormalized;
    }

    public bool IsNight()
    {
        float nightStart = 0.5f;
        float nightEnd = 0.99f;
        return timeOfDayNormalized >= nightStart && timeOfDayNormalized < nightEnd;
    }
}
