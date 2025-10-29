using System;
using UnityEngine;

public class PlaceableController : MonoBehaviour
{
    public Placeable currentPlaceable;
    private GameObject previewObject;
    public static PlaceableController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (currentPlaceable == null) return;
        UpdatePreview();
        HandlePlacementInput();
    }

    public void SetPlaceable(Placeable placeable)
    {
        currentPlaceable = placeable;

        if (previewObject != null)
            Destroy(previewObject);

        if (placeable != null)
        {
            PlaceableData data = (PlaceableData)placeable.instance.data;
            previewObject = Instantiate(data.previewPrefab);
        }
    }

    private void UpdatePreview()
    {
        if (previewObject == null) return;

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 10f))
        {
            previewObject.transform.position = hit.point + previewObject.transform.up * 2.5f;

            Vector3 direction = PlayerMovement.Instance.transform.position - previewObject.transform.position;
            direction.y = 0;
            previewObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }


    private void HandlePlacementInput()
    {

    }
}