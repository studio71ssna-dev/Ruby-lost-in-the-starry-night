using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class DayTimeManager : MonoBehaviour
{
    [Header("Day Settings")]
    public float dayDuration = 60f;
    private int sessionFlowers = 0;
    private CancellationTokenSource _phaseCts;

    private async void Start()
    {
        _phaseCts = new CancellationTokenSource();

        // Start the day cycle
        await RunDayCycle();
    }

    private async UniTask RunDayCycle()
    {
        Debug.Log("The sun rises. Day phase started.");
        float remainingTime = dayDuration;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            // Send normalized value (0 to 1) to UI for the progress bar
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTimer(remainingTime / dayDuration);

            // Wait for the next frame
            await UniTask.Yield(PlayerLoopTiming.Update, _phaseCts.Token);
        }

        await EndDayTransition();
    }

    public void AddFlowerToSession()
    {
        sessionFlowers++;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateFlowerCount(sessionFlowers);
    }

    private async UniTask EndDayTransition()
    {
        // Save flowers to the permanent inventory
        InventoryManager.Instance.AddFlowers(sessionFlowers);

        // Tell the GameManager to open the shop
        // We don't need to 'await' here unless you have a fade-out animation first
        GameManager.Instance.EnterShopPhase().Forget();

        // Destroy the day manager or disable the player movement
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _phaseCts?.Cancel();
        _phaseCts?.Dispose();
    }
}