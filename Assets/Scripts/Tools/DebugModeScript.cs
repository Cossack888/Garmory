using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class DebugModeScript : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;
    private bool debugMode = false;
    private void Start()
    {
        if (debugText != null)
        {
            debugText.gameObject.SetActive(debugMode);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            debugMode = !debugMode;
            if (debugText != null)
            {
                debugText.gameObject.SetActive(debugMode);
            }
        }
        if (debugMode && debugText != null)
        {
            DisplayProfilerData();
        }
    }
    private void DisplayProfilerData()
    {
        if (debugText == null) return;
        float totalMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
        float usedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;
        float unusedMemory = totalMemory - usedMemory;
        float textureMemory = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
        float systemMemoryUsage = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
        debugText.text = string.Format(
            "Memory Usage:\n" +
            "Total Memory: {0} MB\n" +
            "Used Memory: {1} MB\n" +
            "Unused Memory: {2} MB\n" +
            "Texture Memory: {3} MB\n" +
            "System Memory: {4} MB",
            totalMemory, usedMemory, unusedMemory, textureMemory, systemMemoryUsage
        );
    }
}
