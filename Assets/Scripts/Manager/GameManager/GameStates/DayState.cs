using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class DayState : IGameState
{
    GameManager gm;
    CancellationTokenSource cts;

    public DayState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Day Start");
        gm.tileGenerator.SetTileSet(TileSetType.Day);

        cts = new CancellationTokenSource();
        RunDayTimerAsync(cts.Token).Forget();
    }

    private async UniTaskVoid RunDayTimerAsync(CancellationToken token)
    {
        float duration = gm.DayDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (token.IsCancellationRequested) return;
            elapsed += Time.deltaTime;
            UIManager.Instance?.UpdateTimer(elapsed / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        gm.ExpireDayTimer(); // -> ShopState
    }

    public void Update() { }

    public void Exit()
    {
        cts?.Cancel();
        cts?.Dispose();
        UIManager.Instance?.UpdateTimer(0f);
    }
}