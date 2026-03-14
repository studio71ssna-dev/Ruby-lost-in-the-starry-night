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


    private async UniTask RunDayCycle()
    {
        Debug.Log("The sun rises. Day phase started.");
        float remainingTime = dayDuration;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTimer(remainingTime / dayDuration);

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
        InventoryManager.Instance.AddStardust(sessionStardust);

        GameManager.Instance.OnDayEnded();

        gameObject.SetActive(false);
    }

    public void StartNewDay()
    {
        // Reset the session stardust for the new day
        sessionStardust = 0;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateFlowerCount(sessionStardust);

        _phaseCts?.Cancel(); // Clean up old token if it exists
        _phaseCts = new CancellationTokenSource();

        // Fire and forget the task
        RunDayCycle().Forget();
    }

    private void OnDestroy()
    {
        _phaseCts?.Cancel();
        _phaseCts?.Dispose();
    }
}