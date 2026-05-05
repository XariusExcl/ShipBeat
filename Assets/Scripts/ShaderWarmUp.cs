using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Unity.Jobs;
using System.Diagnostics;
using UnityEngine.Events;

public class ShaderWarmUp : MonoBehaviour
{
    public static UnityEvent WarmupDone = new();
    public GraphicsStateCollection graphicsStateCollection;
    Stopwatch stopwatch;
    JobHandle warmupJob;
    bool completedCalled = false;
    bool started = false;
    void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
        Invoke("WarmUp", 0.02f);
    }

    void WarmUp()
    {
        UnityEngine.Debug.Log("Start Shader Warmup");
        warmupJob = graphicsStateCollection.WarmUp();
        started = true;
    }

    void Update()
    {
        if (!started) return;
        if(!warmupJob.IsCompleted || !graphicsStateCollection.isWarmedUp)
        {
            UnityEngine.Debug.Log($"{graphicsStateCollection.completedWarmupCount} / {graphicsStateCollection.totalGraphicsStateCount}");
        } else if (!completedCalled)
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Shader warmup done, took {stopwatch.Elapsed.TotalMilliseconds:F3}ms.");
            WarmupDone.Invoke();
            completedCalled = true;
            Destroy(gameObject);
        }
    }
}