using System;
using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public Transform heldItemHolder;
    public Animator heldItemAnimator;
    private GameObject currentItemObject;
    private ItemData lastItemData;
    private Tool currentTool;

    private float toolUseTimer;

    private void Update()
    {
        ItemInstance item = PlayerInventory.Instance?.GetActiveItem();

        if (item == null)
        {
            if (currentItemObject != null)
            {
                if (currentTool != null && currentTool.toolAnimator != null)
                {
                    AnimatorStateInfo stateInfo = currentTool.toolAnimator.GetCurrentAnimatorStateInfo(0);
                    bool isMidSwing = (stateInfo.IsName("Swing") || stateInfo.IsName("Swing 1")) && stateInfo.normalizedTime < 1f && !currentTool.toolAnimator.IsInTransition(0);
                    if ((stateInfo.IsName("Swing") || stateInfo.IsName("Swing 1")) && !isMidSwing)
                    {
                        currentTool.toolAnimator.ResetTrigger("Swing");
                        currentTool.toolAnimator.SetTrigger("Return");
                    }
                }
                Destroy(currentItemObject);
                currentItemObject = null;
                currentTool = null;
                lastItemData = null;
                toolUseTimer = 0f;
            }
        }
        else
        {
            bool newItemEquipped = UpdateHeldItem(item);

            if (newItemEquipped)
            {
                heldItemAnimator.SetTrigger("Equip");
                
                toolUseTimer = 0.33f;

                if (currentTool != null && currentTool.toolAnimator != null)
                {
                    currentTool.toolAnimator.ResetTrigger("Swing");
                    currentTool.toolAnimator.ResetTrigger("Return");
                }
            }

            bool isSwingingTool = Input.GetMouseButton(0) && item.data.Type == ItemType.tool;

            if (isSwingingTool)
            {
                if (currentTool != null && toolUseTimer <= 0f)
                {
                    AnimatorStateInfo stateInfo = currentTool.toolAnimator.GetCurrentAnimatorStateInfo(0);
                    bool isInSwing = stateInfo.IsName("Swing") || stateInfo.IsName("Swing 1");
                    bool swingFinished = isInSwing && stateInfo.normalizedTime >= 1f && !currentTool.toolAnimator.IsInTransition(0);

                    if (swingFinished || !isInSwing)
                    {
                        if (currentTool.data == null)
                        {
                            Debug.LogWarning("Tool data is null on " + item.data.name);
                            return;
                        }
                        currentTool.Use();
                        currentTool.toolAnimator.SetTrigger("Swing");
                        toolUseTimer = Mathf.Max(currentTool.data.cooldown, 0.1f);
                    }
                }
            }
            else if (item.data.Type == ItemType.tool && toolUseTimer <= 0f)
            {
                if (currentTool != null && currentTool.toolAnimator != null)
                {
                    AnimatorStateInfo stateInfo = currentTool.toolAnimator.GetCurrentAnimatorStateInfo(0);
                    bool isInSwing = stateInfo.IsName("Swing") || stateInfo.IsName("Swing 1");
                    bool isMidSwing = isInSwing && stateInfo.normalizedTime < 1f && !currentTool.toolAnimator.IsInTransition(0);
                    
                    if (isInSwing && !isMidSwing)
                    {
                        currentTool.toolAnimator.ResetTrigger("Swing");
                        currentTool.toolAnimator.SetTrigger("Return");
                    }
                }
            }

            // Separate input for consumables to avoid conflict with Mouse0
            if (!isSwingingTool && Input.GetButtonDown("Use") && item.data.Type == ItemType.consumable)
                PlayerInventory.Instance?.UseActiveItem(1);

            if (toolUseTimer > 0f)
                toolUseTimer -= Time.deltaTime;
        }
    }

    private bool UpdateHeldItem(ItemInstance item)
    {
        if (lastItemData == item.data) return false;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        lastItemData = item.data;

        if (item.data.heldPrefab != null && heldItemHolder.childCount > 0)
        {
            currentItemObject = Instantiate(item.data.heldPrefab, heldItemHolder.GetChild(0));
            currentItemObject.layer = LayerMask.NameToLayer("HeldItem");
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localRotation = Quaternion.identity;
            currentItemObject.transform.localScale = Vector3.one;

            // Cache Tool component
            currentTool = currentItemObject?.GetComponent<Tool>();
            if (currentTool == null)
                Debug.LogWarning("Tool component missing on prefab: " + item.data.name);

            return true;
        }
        else
        {
            Debug.LogWarning("HeldItemHolder has no children or prefab is null for: " + item.data.name);
            currentTool = null;
            return false;
        }
    }
}