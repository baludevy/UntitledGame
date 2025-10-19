using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlaceableController : MonoBehaviour
{
    private PlayerInventory inventory;

    private Transform preview;

    private void Start()
    {
        inventory = PlayerInventory.Instance;
    }

    public void Update()
    {
        if (inventory.ActiveItem?.data is PlaceableItem item)
        {
            if(item.Name == "Campfire" && CampfireController.Instance.campfire != null) return;  
            
            LayerMask ground = LayerMask.GetMask("Ground");

            if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 6.7f, ground))
            {
                Vector3 pos = hit.point + new Vector3(0, item.previewPrefab.transform.position.y, 0);
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

                if (preview == null)
                    preview = Instantiate(item.previewPrefab, pos, rot).transform;

                preview.position = pos;
                preview.rotation = rot;

                if (Input.GetMouseButtonDown(0))
                {
                    GameObject placed = Instantiate(item.placedPrefab, pos, rot);
                    
                    if(preview != null)
                        Destroy(preview.gameObject);

                    placed.transform.SetParent(null, true);
                    
                    inventory.RemoveItemByID(inventory.ActiveItem.id);
                }
            }
            else
            {
                if (preview != null)
                    Destroy(preview.gameObject);
            }
        }
        else
        {
            if (preview != null)
                Destroy(preview.gameObject);
        }
    }
}