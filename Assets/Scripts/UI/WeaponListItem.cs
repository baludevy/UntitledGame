using UnityEngine;
using UnityEngine.UI;

public class WeaponListItem : MonoBehaviour
{
    [SerializeField] private GameObject border;
    [SerializeField] private Image icon;

    public WeaponInstance instanceReference;

    public void Initialize(bool active, Sprite icon, WeaponInstance instance)
    {
        instanceReference = instance;
        this.icon.sprite = icon;
        SetState(active);
    }

    public bool Matches(WeaponInstance inst)
    {
        return instanceReference == inst;
    }

    public void SetState(bool active)
    {
        border.SetActive(active);
    }
}