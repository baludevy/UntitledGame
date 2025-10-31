using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    private float deltaTime;
    private Vector3 playerPosition;
    private float playerSpeed;
    private float allocatedMemory;
    private float reservedMemory;
    private GUIStyle style;

    private void Start()
    {
        Application.targetFrameRate = 240;
        style = new GUIStyle();
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        playerPosition = PlayerMovement.Instance.transform.position;
        playerSpeed = PlayerMovement.Instance.GetRigidbody().velocity.magnitude;
        allocatedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        reservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;

        GUI.Label(new Rect(10, 10, 800, 30), "FPS: " + Mathf.Ceil(fps), style);
        GUI.Label(new Rect(10, 40, 800, 30),
            "RAM: " + reservedMemory.ToString("F0") + " MB" + "/" + allocatedMemory.ToString("F0") + " MB", style);
        GUI.Label(new Rect(10, 70, 800, 30), "VEL: " + playerSpeed.ToString("F2"), style);
    }
}