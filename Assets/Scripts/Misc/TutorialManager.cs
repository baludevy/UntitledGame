using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text tutorialText;

    [Header("Settings")]
    public float warningTimeBeforeNight = 30f;

    private bool showedCampfireWarning;
    private bool showedLightCampfire;
    private bool startTipActive = true;
    private bool tutorialEnded;

    public static TutorialManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (DayNightCycle.Instance == null || tutorialEnded)
            return;

        int currentDay = DayNightCycle.Instance.currentDay;
        if (currentDay != 0)
        {
            tutorialText.text = "";
            tutorialEnded = true;
            return;
        }

        if (startTipActive)
        {
            tutorialText.text = "Chop down trees and prepare for nighttime";

            if (PlayerInventory.Instance.HasItem("Oak Wood") || PlayerInventory.Instance.HasItem("Birch Wood"))
            {
                tutorialText.text = "";
                startTipActive = false;
            }
        }

        float currentTime = DayNightCycle.Instance.currentTimeOfDay;
        float cycleDurationSeconds = DayNightCycle.Instance.cycleDuration * 60;
        float timeLeft = (1f - currentTime) * cycleDurationSeconds;

        if (!showedCampfireWarning && timeLeft <= warningTimeBeforeNight)
        {
            if (CampfireController.Instance.campfire == null)
            {
                tutorialText.text = "Place a campfire";
                showedCampfireWarning = true;
            }
        }

        bool isNight = DayNightCycle.Instance.IsNight();
        if (isNight && !showedLightCampfire)
        {
            var campfire = CampfireController.Instance.campfire;
            if (campfire != null && !campfire.lit)
            {
                tutorialText.text = "Light the campfire";
                showedLightCampfire = true;
            }
        }

        if (isNight && CampfireController.Instance.campfire != null && CampfireController.Instance.campfire.lit)
        {
            tutorialText.text = "";
            tutorialEnded = true;
        }
    }
}
