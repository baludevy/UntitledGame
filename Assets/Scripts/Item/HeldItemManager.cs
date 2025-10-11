using System.Collections;
using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public Transform heldItemHolder;
    public Animator heldItemAnimator;
    private GameObject currentItemObject;
    private ItemData lastItemData;
    private Tool currentTool;

    private float toolUseTimer;
    private bool isSwinging = false;
    private bool swingInProgress = false;
    private float lastClickTime;
    private const float HOLD_THRESHOLD = 0.2f;

    private void Update()
    {
        ItemInstance item = PlayerInventory.Instance?.GetActiveItem();

        if (item == null)
        {
            ClearHeldItem();
        }
        else
        {
            bool newItemEquipped = UpdateHeldItem(item);

            if (newItemEquipped)
            {
                heldItemAnimator.SetTrigger("Equip");
                toolUseTimer = 0.33f;
                ResetToolAnimationState();
            }

            HandleToolUsage(item);
            HandleConsumableUsage(item);
            
            if (toolUseTimer > 0f)
                toolUseTimer -= Time.deltaTime;
        }
    }

    private void ClearHeldItem()
    {
        if (currentItemObject != null)
        {
            ResetToolAnimationState();
            Destroy(currentItemObject);
            currentItemObject = null;
            currentTool = null;
            lastItemData = null;
            toolUseTimer = 0f;
            isSwinging = false;
            swingInProgress = false;
        }
    }

    private void HandleToolUsage(ItemInstance item)
    {
        if (item.data.Type != ItemType.tool) 
        {
            if (isSwinging)
            {
                ResetToolAnimationState();
            }
            return;
        }

        bool mouseDown = Input.GetMouseButtonDown(0);
        bool mouseHeld = Input.GetMouseButton(0);
        bool canUseTool = !PlayerInventory.Instance.inventoryOpen;

        if (mouseDown)
        {
            lastClickTime = Time.time;
        }

        bool isHolding = mouseHeld && (Time.time - lastClickTime >= HOLD_THRESHOLD);
        bool quickClick = mouseDown && !isHolding;

        if ((quickClick || isHolding) && toolUseTimer <= 0f && !swingInProgress && canUseTool)
        {
            if (currentTool != null)
            {
                StartToolSwing();
            }
        }
        else if (!mouseHeld && isSwinging && !swingInProgress)
        {
            ReturnToolToIdle();
        }

        if (swingInProgress && currentTool != null && currentTool.toolAnimator != null)
        {
            AnimatorStateInfo stateInfo = currentTool.toolAnimator.GetCurrentAnimatorStateInfo(0);
            bool isInSwingState = stateInfo.IsName("Swing") || stateInfo.IsName("Swing 1");
            
            if (isInSwingState && stateInfo.normalizedTime >= 1f)
            {
                SwingCompleted();
            }
        }
    }

    private void StartToolSwing()
    {
        if (currentTool.data == null) return;

        swingInProgress = true;
        isSwinging = true;
        
        currentTool.toolAnimator.ResetTrigger("Return");
        currentTool.toolAnimator.SetTrigger("Swing");
        
        StartCoroutine(UseTool(currentTool));
        toolUseTimer = Mathf.Max(currentTool.data.cooldown, 0.1f);
    }

    private void SwingCompleted()
    {
        swingInProgress = false;
        
        bool mouseStillHeld = Input.GetMouseButton(0) && !PlayerInventory.Instance.inventoryOpen;
        
        if (!mouseStillHeld)
        {
            ReturnToolToIdle();
        }
    }

    private void ReturnToolToIdle()
    {
        if (currentTool != null && currentTool.toolAnimator != null)
        {
            isSwinging = false;
            currentTool.toolAnimator.ResetTrigger("Swing");
            currentTool.toolAnimator.SetTrigger("Return");
        }
    }

    private void ResetToolAnimationState()
    {
        if (currentTool != null && currentTool.toolAnimator != null)
        {
            currentTool.toolAnimator.ResetTrigger("Swing");
            currentTool.toolAnimator.ResetTrigger("Return");
            currentTool.toolAnimator.Play("Idle", 0, 0f);
        }
        isSwinging = false;
        swingInProgress = false;
    }

    private void HandleConsumableUsage(ItemInstance item)
    {
        if (!Input.GetButtonDown("Use") || item.data.Type != ItemType.consumable) return;
        PlayerInventory.Instance?.UseActiveItem(1);
    }

    private IEnumerator UseTool(Tool tool)
    {
        yield return new WaitForSeconds(0.1f);
        tool.Use();
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

            currentTool = currentItemObject?.GetComponent<Tool>();

            isSwinging = false;
            swingInProgress = false;

            return true;
        }
        else
        {
            currentTool = null;
            return false;
        }
    }
}