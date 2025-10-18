using Mono.Cecil;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Resource")]
public class ResourceItem : ItemData
{
    public ResourceTypes resourceType;
    public int value;
}