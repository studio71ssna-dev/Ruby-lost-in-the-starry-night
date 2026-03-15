using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Lightweight runtime monitor to detect long frames and capture recent logs/state changes.
// Add this to a persistent GameObject in the scene while debugging.
public class FrameMonitor : MonoBehaviour
{
    [Tooltip("Frame time threshold in seconds to consider a frame stall (unscaled).")]
    public float stallThreshold = 0.1f; //100ms

    [Tooltip("How many recent log lines to keep for diagnostics.")]
    public int recentLogCapacity = 200;

    private Queue<string> recentLogs = new Queue<string>();

    // reflection caches to avoid repeated lookups
    private Type _gameManagerType;
    private object _gameManagerInstance;
    private UnityEngine.Events.UnityEvent _dayStartEvent;
    private UnityEngine.Events.UnityEvent _afterDayEndsEvent;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

        // Find GameManager by name using reflection so this script doesn't fail to compile
        // if types are in different assemblies or ordering changes.
        _gameManagerType = FindTypeByName("GameManager");
        if (_gameManagerType != null)
        {
            // Get static Instance field/property
            FieldInfo instField = _gameManagerType.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instField != null)
                _gameManagerInstance = instField.GetValue(null);
            else
            {
                PropertyInfo instProp = _gameManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instProp != null)
                    _gameManagerInstance = instProp.GetValue(null);
            }

            if (_gameManagerInstance != null)
            {
                // Attempt to get the UnityEvent fields and subscribe
                _dayStartEvent = GetUnityEventField(_gameManagerType, _gameManagerInstance, "DayStartEvent");
                if (_dayStartEvent != null) _dayStartEvent.AddListener(OnDayStarted);

                _afterDayEndsEvent = GetUnityEventField(_gameManagerType, _gameManagerInstance, "AfterDayEndsEvent");
                if (_afterDayEndsEvent != null) _afterDayEndsEvent.AddListener(OnAfterDayEnds);
            }
        }
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;

        if (_dayStartEvent != null) _dayStartEvent.RemoveListener(OnDayStarted);
        if (_afterDayEndsEvent != null) _afterDayEndsEvent.RemoveListener(OnAfterDayEnds);

        _dayStartEvent = null;
        _afterDayEndsEvent = null;
        _gameManagerInstance = null;
        _gameManagerType = null;
    }

    private UnityEngine.Events.UnityEvent GetUnityEventField(Type type, object instance, string fieldName)
    {
        try
        {
            FieldInfo fi = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (fi != null)
            {
                return fi.GetValue(instance) as UnityEngine.Events.UnityEvent;
            }

            PropertyInfo pi = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (pi != null)
            {
                return pi.GetValue(instance) as UnityEngine.Events.UnityEvent;
            }
        }
        catch (Exception) { }
        return null;
    }

    private Type FindTypeByName(string name)
    {
        // Try Type.GetType first (works if in same assembly with namespace)
        Type t = Type.GetType(name);
        if (t != null) return t;

        // Search loaded assemblies
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                t = asm.GetType(name);
                if (t != null) return t;

                // fallback: search by simple name
                foreach (var tt in asm.GetTypes())
                {
                    if (tt.Name == name) return tt;
                }
            }
            catch (ReflectionTypeLoadException) { }
        }
        return null;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        string time = Time.realtimeSinceStartup.ToString("F2");
        string line = $"[{time}] {type}: {condition}";
        recentLogs.Enqueue(line);
        if (recentLogs.Count > recentLogCapacity) recentLogs.Dequeue();
    }

    private void OnDayStarted()
    {
        recentLogs.Enqueue($"[{Time.realtimeSinceStartup:F2}] EVENT: DayStartEvent invoked");
        if (recentLogs.Count > recentLogCapacity) recentLogs.Dequeue();
    }

    private void OnAfterDayEnds()
    {
        recentLogs.Enqueue($"[{Time.realtimeSinceStartup:F2}] EVENT: AfterDayEndsEvent invoked");
        if (recentLogs.Count > recentLogCapacity) recentLogs.Dequeue();
    }

    private float lastRealtime;

    private void Start()
    {
        lastRealtime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        float now = Time.realtimeSinceStartup;
        float delta = now - lastRealtime;
        lastRealtime = now;

        if (delta >= stallThreshold)
        {
            Debug.LogWarning($"[FrameMonitor] Long frame detected: {delta:F3}s (threshold {stallThreshold:F3}s). Dumping recent logs...");
            DumpRecentLogs();
        }
    }

    private void DumpRecentLogs()
    {
        Debug.Log("[FrameMonitor] ---- Recent logs (most recent last) ----");
        foreach (var l in recentLogs)
        {
            Debug.Log(l);
        }
        Debug.Log("[FrameMonitor] ---- end recent logs ----");
    }
}
