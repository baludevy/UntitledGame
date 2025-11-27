using UnityEngine;
using UnityEngine.UI;

public class AbilityListItem : MonoBehaviour
{
    [SerializeField] private GameObject border;
    [SerializeField] private Image icon;

    public void Initialize(bool active, Sprite icon, WeaponInstance instance)
    {
        this.icon.sprite = icon;
        SetState(active);
    }

    public void SetState(bool active)
    {
        border.SetActive(active);
    }
}