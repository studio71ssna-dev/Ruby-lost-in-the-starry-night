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

    private async void Start()
    {
        _phaseCts = new CancellationTokenSource();
        await RunDayCycle();
    }

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

    private async UniTask EndDayTransition()
    {
        // Fix: Use the new method name 'AddStardust' from our refactored InventoryManager
        InventoryManager.Instance.AddStardust(sessionStardust);

        GameManager.Instance.EnterShopPhase().Forget();
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _phaseCts?.Cancel();
        _phaseCts?.Dispose();
    }
}