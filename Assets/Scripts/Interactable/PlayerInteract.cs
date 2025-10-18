using System;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool blocked;
    
    public static PlayerInteract Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Update()
    {
        if (blocked || PlayerUIManager.Instance.containerOpen)
        {
            PlayerUIManager.Instance.keyTip.SetActive(false);
            return;
        }

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