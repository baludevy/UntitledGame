using EZCameraShake;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public BaseTool currentBaseTool;
    private float useTimer;
    private bool wasSwinging;
    private bool swingingThisFrame;
    private bool usedDuringSwing;
    private bool checkAfterFrame;
    private readonly float swingTrigger = 0.2f;

    public static ToolController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!PlayerInventory.Instance.inventoryOpen)
            ToolUseInput();

        if (currentBaseTool == null) return;

        if (checkAfterFrame && !usedDuringSwing)
        {
            var info = currentBaseTool.toolAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Swing") && info.normalizedTime >= swingTrigger)
            {
                UseTool();
                usedDuringSwing = true;
            }
        }

        checkAfterFrame = true;
    }

    private void ToolUseInput()
    {
        if (currentBaseTool == null) return;

        bool isSwinging = Input.GetMouseButton(0);

        if (isSwinging && useTimer <= 0 && !swingingThisFrame)
        {
            ToolItem toolData = (ToolItem)currentBaseTool.instance.data;
            float baseLength = 1f;
            float speedMultiplier = baseLength / toolData.cooldown;
            currentBaseTool.toolAnimator.speed = speedMultiplier;
            currentBaseTool.toolAnimator.Play("Swing", 0, 0f);

            usedDuringSwing = false;
            checkAfterFrame = false;
            useTimer = toolData.cooldown;
            swingingThisFrame = true;
        }

        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        wasSwinging = isSwinging;
        swingingThisFrame = false;
    }

    private void UseTool()
    {
        CameraShaker.Instance.ShakeOnce(2f, 2f, 0.15f, 0.2f);
        currentBaseTool.Use();
    }


    public void SetTool(BaseTool baseTool)
    {
        currentBaseTool = baseTool;
    }
}