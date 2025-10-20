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

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            IInteractable interactable = GetInteractable(hit.collider.transform);

            if (interactable != null)
            {
                PlayerUIManager.Instance.keyTip.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                    interactable.Interact();
            }
            else
            {
                PlayerUIManager.Instance.keyTip.SetActive(false);
            }
        }
        else
        {
            PlayerUIManager.Instance.keyTip.SetActive(false);
        }
    }

    private IInteractable GetInteractable(Transform t)
    {
        while (t != null)
        {
            var i = t.GetComponent<IInteractable>();
            if (i != null)
                return i;

            t = t.parent;
        }
        return null;
    }
}