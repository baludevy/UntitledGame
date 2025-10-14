using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sunLight;

    [Header("Day-Night cycle settings")] 
    [SerializeField] private float startTime;

    [Tooltip("In Minutes")] 
    [SerializeField] private float cycleDuration;

    [Header("Lighting settings")] 
    [SerializeField] private float lightIntensityNight = 0f;
    [SerializeField] private float lightIntensityDay = 1f;
    
    [SerializeField] private float sunriseAngle = -90f;
    [SerializeField] private float sunsetAngle = 90f;

    private float currentTimeOfDay;
    private float timeOfDayNormalized;

    private void Start()
    {
        // convert day start time from minutes to seconds
        currentTimeOfDay = startTime * 60;
    }

    public void Update()
    {
        currentTimeOfDay += Time.deltaTime;
        
        // convert cycle duration time from minutes to seconds
        float cycleDurationSeconds = cycleDuration * 60;

        timeOfDayNormalized = currentTimeOfDay % cycleDurationSeconds / cycleDurationSeconds;
        
        float sunAngle = timeOfDayNormalized * 360f;
        sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 50f, 0f);
        
        float intensityFactor = Mathf.Clamp01(Mathf.Sin(timeOfDayNormalized * Mathf.PI * 2f));
        sunLight.intensity = Mathf.Lerp(lightIntensityNight, lightIntensityDay, intensityFactor);
    }
}