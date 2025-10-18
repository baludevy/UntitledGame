using UnityEngine;

[CreateAssetMenu(menuName = "Items/Placeable")]
public class PlaceableItem : ItemData
{
    public GameObject previewPrefab;
    public GameObject placedPrefab;
}