using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Morning,
        ShopArrival,
        Shopping,
        Night,
        Quiz,
        GameOver
    }

    public GameState State { get; private set; }
    public UnityEvent AfterDayEndsEvent;
    public UnityEvent DayStartEvent;
    public int DayCount => dayCount;
    public event Action<int> OnDayChanged;
    [Header("References")]
    public GroundTileGenerator tileGenerator;
    public GameObject shopUI;
    public ChoiceManager choiceManager;
    public ParallaxBackground background;
    public DayTimeManager dayTimeManager;
    [SerializeField] private ShopManager shopManager;


    private int dayCount = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartMorning();
    }


    // ======================
    // MORNING
    // ======================

    public void StartMorning()
    {
        State = GameState.Morning;
        DayStartEvent.Invoke(); 
        background.SetMorning();
        tileGenerator.SwitchToDayChunks();

        // Wake up the timer and start it
        dayTimeManager.gameObject.SetActive(true);
        dayTimeManager.StartNewDay();

        OnDayChanged?.Invoke(dayCount);
        Debug.Log($"DAY {dayCount} STARTED");
    }

    // Called by DayTimeManager
    public void OnDayEnded()
    {
        State = GameState.ShopArrival;
        tileGenerator.SpawnShopChunk();
    }

    // ======================
    // SHOP
    // ======================

    public async void OpenShop()
    {
        State = GameState.Shopping;
        AfterDayEndsEvent.Invoke();
        // The code pauses here until the player clicks the Leave button in the shop
        await shopManager.StartShopping();

        // ONCE THE SHOP IS CLOSED, TRIGGER THE NIGHT
        StartNight();
    }

    // ======================
    // NIGHT
    // ======================

    public void StartNight()
    {
        State = GameState.Night;
        background.SetNight();
        tileGenerator.SwitchToNightChunks();
        tileGenerator.SpawnNightChunk();


    }

    // ======================
    // QUIZ
    // ======================

    public void StartQuiz()
    {
        State = GameState.Quiz;
        choiceManager.ShowRandomQuestion();
    }

    // ======================
    // NEXT DAY
    // ======================

    public void SleepAndNextDay()
    {
        
        dayCount++;
        StartMorning();
        
    }

}