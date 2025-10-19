using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text tutorialText;

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
            tutorialText.text = "Pick up your items, chop down trees and prepare for nighttime";

            if (PlayerInventory.Instance.HasItem("Oak Wood") || PlayerInventory.Instance.HasItem("Birch Wood"))
            {
                tutorialText.text = "";
                startTipActive = false;
            }
        }

        if (!showedCampfireWarning && DayNightCycle.Instance.IsNight())
        {
            if (CampfireController.Instance.campfire == null)
            {
                tutorialText.text = "Place a campfire with your left mouse button";
                showedCampfireWarning = true;
            }
        }

        bool isNight = DayNightCycle.Instance.IsNight();
        if (isNight && !showedLightCampfire)
        {
            Campfire campfire = CampfireController.Instance.campfire;
            if (campfire != null && !campfire.lit)
            {
                tutorialText.text =
                    "Light the campfire with your wood make sure it doesn't extinguish, and don't go too far from it so you don't get too cold";
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