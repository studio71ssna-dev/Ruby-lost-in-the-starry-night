using UnityEngine;
using UnityEngine.Events;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GroundTileGenerator tileGenerator;
    public MusicTimer musicTimer;

    GameStateMachine stateMachine;

    public MorningState MorningState { get; private set; }
    public DayState DayState { get; private set; }
    public ShopState ShopState { get; private set; }
    public NightState NightState { get; private set; }

    public int DayCount { get; private set; } = 1;

    public event Action<int> OnDayChanged;

    public UnityEvent DayStartEvent;
    void Awake()
    {
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

    public void ChangeState(IGameState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void StartMorning()
    {
        ChangeState(MorningState);
    }

    public void SleepAndNextDay()
    {
        DayCount++;
        OnDayChanged?.Invoke(DayCount);

        ChangeState(MorningState);
    }

    public void OpenShop()
    {
        ChangeState(ShopState);
    }
}