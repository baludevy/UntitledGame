using System;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Serialization;

public class ToolController : MonoBehaviour
{
    public Tool currentTool;
    [NonSerialized] public float useTimer;

    public static ToolController Instance;

    public int maxTools = 6;
    public List<Tool> startingTools = new();
    private List<Tool> tools = new();

    [SerializeField] private Transform heldToolTransform;
    [SerializeField] private Animator itemRootAnimator;
    private GameObject currentHeldObject;

    [FormerlySerializedAs("test")] public Tool testTool;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (heldToolTransform != null)
            itemRootAnimator = heldToolTransform.GetComponent<Animator>();

        foreach (Tool tool in startingTools)
            AddTool(tool);

        if (tools.Count > 0)
            SwitchToTool(tools[0]);
    }

    private void Update()
    {
        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        if (currentTool != null)
        {
            currentTool.HandleInput();
            currentTool.UpdateTool();
        }

        for (int i = 0; i < maxTools; i++)
        {
            if (i < tools.Count && Input.GetKeyDown(KeyCode.Alpha1 + i) && tools[i] != null &&
                tools[i].data != currentTool?.data)
                SwitchToTool(tools[i]);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            AddTool(testTool);
        }
    }

    private void SwitchToTool(Tool targetTool)
    {
        if (targetTool == null || targetTool.data == currentTool?.data) return;

        if (currentHeldObject != null)
            Destroy(currentHeldObject);

        if (targetTool.data != null && targetTool.data.toolPrefab != null)
        {
            itemRootAnimator?.SetTrigger("Equip");

            currentHeldObject = Instantiate(targetTool.data.toolPrefab, heldToolTransform);

            SetHeldItemLayers(currentHeldObject.transform);

            currentTool = currentHeldObject.GetComponent<Tool>();
        }
        else
        {
            currentTool = null;
        }

        PlayerUIManager.Instance.SetActiveTool(currentTool?.data);
    }

    private void SetHeldItemLayers(Transform obj)
    {
        foreach (Transform child in obj)
        {
            child.gameObject.layer = LayerMask.NameToLayer("HeldItem");
        }
    }

    public void AddTool(Tool tool)
    {
        if (tool == null || tools.Contains(tool) || tools.Count == maxTools) return;
        tools.Insert(0, tool);
        PlayerUIManager.Instance.AddToolToToolbar(tool);
    }
}