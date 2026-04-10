using UnityEngine;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GroundTileGenerator tileGenerator;
    [Header("Day Settings")]
    [Header("Static Environments")]
    public GameObject staticNightEnvironment; // Drag your permanent Night parent object here
    public Transform nightSpawnPoint;         // Where Ruby teleports when night starts
    public Transform morningSpawnPoint;       // Where Ruby teleports when a new day starts
    public float DayDuration = 60f;
    GameStateMachine stateMachine;

    public MorningState MorningState { get; private set; }
    public DayState DayState { get; private set; }
    public ShopState ShopState { get; private set; }
    public NightState NightState { get; private set; }

    public int DayCount { get; private set; } = 1;

    public event Action<int> OnDayChanged;
    public event Action OnDayTimerExpired;   // fired by DayState when timer ends

    public UnityEvent DayStartEvent;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        stateMachine = new GameStateMachine();
        MorningState = new MorningState(this);
        DayState = new DayState(this);
        ShopState = new ShopState(this);
        NightState = new NightState(this);
    }

    void Start()
    {
        ChangeState(MorningState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(IGameState newState) => stateMachine.ChangeState(newState);

    public void StartMorning() => ChangeState(MorningState);

    // Called by the day timer expiring
    public void ExpireDayTimer()
    {
        OnDayTimerExpired?.Invoke();
        ChangeState(ShopState);
    }

    // Called by Proceed button in shop
    public void ProceedFromShop() => ChangeState(NightState);

    public void SleepAndNextDay()
    {
        DayCount++;
        OnDayChanged?.Invoke(DayCount);
        ChangeState(MorningState);
    }

    public void OpenShop() => ChangeState(ShopState);
}