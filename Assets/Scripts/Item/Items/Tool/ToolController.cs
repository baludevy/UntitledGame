using UnityEngine;

public class ToolController : MonoBehaviour
{
    public Tool currentTool;
    private float useTimer;
    private bool wasSwinging;
    private bool swingingThisFrame;
    
    public static ToolController Instance;
    private static readonly int Return = Animator.StringToHash("Return");

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (currentTool == null) return;

        bool isSwinging = Input.GetMouseButton(0);
        
        if (isSwinging && useTimer <= 0 && !swingingThisFrame)
        {
            currentTool.toolAnimator.Play("Swing", 0, 0f);
            UseTool();
            swingingThisFrame = true;
        }
        
        if (!isSwinging && wasSwinging)
            currentTool.toolAnimator.SetTrigger(Return);

        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        wasSwinging = isSwinging;
        swingingThisFrame = false;
    }

    void UseTool()
    {
        ToolItem toolData = (ToolItem)currentTool.instance.data;
        useTimer = toolData.cooldown;
        currentTool.Use();
    }

    public void SetTool(Tool tool)
    {
        currentTool = tool;
    }
}