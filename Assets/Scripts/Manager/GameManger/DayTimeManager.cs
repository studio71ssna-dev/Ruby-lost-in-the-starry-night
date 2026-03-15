using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class DayTimeManager : MonoBehaviour
{
    [Header("Day Settings")]
    public float dayDuration = 60f;

    // Change: Track currency value instead of just count
    private int sessionStardust = 0;
    private CancellationTokenSource _phaseCts;

    // Track whether the game was paused so we can log pause/resume transitions
    private bool _isPausedFlag = false;

    private async UniTask RunDayCycle()
    {
        Debug.Log($"[DayTimeManager] RunDayCycle started at {Time.realtimeSinceStartup:F2}");
        Debug.Log("The sun rises. Day phase started.");
        float remainingTime = dayDuration;

        while (remainingTime > 0)
        {
            // Use scaled delta so the timer pauses when Time.timeScale == 0 (Pause)
            float delta = Time.deltaTime;

            // Detect pause/resume transitions for debugging
            if (Time.timeScale == 0f)
            {
                if (!_isPausedFlag)
                {
                    _isPausedFlag = true;
                    Debug.Log($"[DayTimeManager] Detected pause at realtime {Time.realtimeSinceStartup:F2}");
                }
            }
            else
            {
                if (_isPausedFlag)
                {
                    _isPausedFlag = false;
                    Debug.Log($"[DayTimeManager] Resumed at realtime {Time.realtimeSinceStartup:F2}");
                }
            }

            remainingTime -= delta;

            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTimer(Mathf.Clamp01(remainingTime / dayDuration));

            await UniTask.Yield(PlayerLoopTiming.Update, _phaseCts.Token);
        }

        await EndDayTransition();
    }

    // Change: Accept FlowerData so we can add its specific value
    public void AddFlowerToSession(FlowerData flower)
    {
        sessionStardust += flower.value;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateFlowerCount(sessionStardust);
    }

    private async Cysharp.Threading.Tasks.UniTask EndDayTransition()
    {
        Debug.Log($"[DayTimeManager] EndDayTransition invoked at {Time.realtimeSinceStartup:F2}");

        InventoryManager.Instance.AddStardust(sessionStardust);

        GameManager.Instance.OnDayEnded();

        gameObject.SetActive(false);
    }

    public void StartNewDay()
    {
        Debug.Log($"[DayTimeManager] StartNewDay invoked at {Time.realtimeSinceStartup:F2}");

        // Reset the session stardust for the new day
        sessionStardust = 0;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateFlowerCount(sessionStardust);

        // Cancel and dispose any previous token to avoid leaked tokens
        _phaseCts?.Cancel();
        _phaseCts?.Dispose();
        _phaseCts = new CancellationTokenSource();

        // Reset pause flag when starting a new day
        _isPausedFlag = (Time.timeScale == 0f);

        // Fire and forget the task
        RunDayCycle().Forget();
    }

    private void OnDestroy()
    {
        _phaseCts?.Cancel();
        _phaseCts?.Dispose();
    }
}