using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Image bar;
    
    public bool wasLit;
    public bool lit { get; private set; }

    private PlayerInventory inventory;

    private void Start()
    {
        inventory = PlayerInventory.Instance;
        CampfireController.Instance.campfire = this;
        CampfireController.Instance.ResetCampfireTimer();
        UpdateUI(CampfireController.Instance.GetCampfireTimer(), CampfireController.timerCap);
    }

    public void Interact()
    {
        if (inventory.ActiveItem?.data is ResourceItem item && item.resourceType == ResourceTypes.Wood &&
            DayNightCycle.IsNight())
        {
            inventory.SubtractAmountFromItem(inventory.ActiveItem, 1);

            if (!lit)
                Light();

            CampfireController.Instance.AddCampfireTime(item.value);
        }
    }

    public void Light()
    {
        lit = true;
        infoText.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);
        bar.transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BaseMineable>().canBeMined = false;
    }

    public void Extinguish()
    {
        lit = false;
        infoText.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        bar.transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BaseMineable>().canBeMined = true;
    }

    public void UpdateUI(float current, float max)
    {
        infoText.text = Mathf.FloorToInt(current).ToString();
        bar.fillAmount = current / max;
    }

    private void OnDestroy()
    {
        if (CampfireController.Instance != null)
            CampfireController.Instance.campfire = null;
    }
}