using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    private float deltaTime;
    private Vector3 playerPosition;
    private Vector3 playerVelocity;
    private float allocatedMemory;
    private float reservedMemory;
    private float monoMemory;
    private GUIStyle style;
    private AbilityController abilityController;

    private void Start()
    {
        Application.targetFrameRate = 144;
        style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        abilityController = FindObjectOfType<AbilityController>();
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        playerPosition = PlayerMovement.Instance.transform.position;
        playerVelocity = PlayerMovement.Instance.rb.velocity;
        allocatedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        reservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
        monoMemory = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        float frameTime = deltaTime * 1000.0f;

        GUI.Label(new Rect(10, 10, 800, 30), "fps: " + Mathf.Ceil(fps), style);
        GUI.Label(new Rect(10, 40, 800, 30), "frametime: " + frameTime.ToString("F2") + " ms", style);
        GUI.Label(new Rect(10, 70, 800, 30), "mem alloc: " + allocatedMemory.ToString("F2") + " mb", style);
        GUI.Label(new Rect(10, 100, 800, 30), "mem reserved: " + reservedMemory.ToString("F2") + " mb", style);
        GUI.Label(new Rect(10, 130, 800, 30), "mono mem: " + monoMemory.ToString("F2") + " mb", style);
        GUI.Label(new Rect(10, 160, 800, 30), "pos: " + playerPosition.ToString("F2"), style);
        GUI.Label(new Rect(10, 190, 800, 30), "vel: " + playerVelocity.ToString("F2"), style);

        if (abilityController != null)
        {
            GUI.Label(new Rect(10, 220, 800, 30),
                "p_ab cooldown: " + Mathf.Max(0, abilityController.primaryTimer).ToString("F2"), style);
            GUI.Label(new Rect(10, 250, 800, 30),
                "s_ab cooldown: " + Mathf.Max(0, abilityController.secondaryTimer).ToString("F2"), style);
        }
    }
}