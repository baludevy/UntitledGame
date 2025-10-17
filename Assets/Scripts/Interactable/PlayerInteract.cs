using System;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public void Update()
    {
        if (PlayerUIManager.Instance.containerOpen) return;

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                PlayerUIManager.Instance.keyTip.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    var interactable = hit.collider.GetComponent<IInteractable>();
                    switch (interactable)
                    {
                        case Chest chest:
                            chest.Interact();
                            break;
                    }
                }
            }
        }
        else
        {
            PlayerUIManager.Instance.keyTip.SetActive(false);
        }
    }
}