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

    /*public void StartMorning()
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
    }*/

    // ======================
    // SHOP
    // ======================

    public async void OpenShop()
    {
        Debug.Log($"[GameManager] OpenShop invoked at {Time.realtimeSinceStartup:F2}");
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

    /*public void StartNight()
    {
        State = GameState.Night;
        background.SetNight();
        tileGenerator.SwitchToNightChunks();
        tileGenerator.SpawnNightChunk();

    }*/

    // ======================
    // QUIZ
    // ======================

    public void StartQuiz()
    {
        Debug.Log($"[GameManager] StartQuiz invoked at {Time.realtimeSinceStartup:F2}");
        State = GameState.Quiz;
        choiceManager.ShowRandomQuestion();
    }

    // ======================
    // NEXT DAY
    // ======================

    public void SleepAndNextDay()
    {
        Debug.Log($"[GameManager] SleepAndNextDay invoked at {Time.realtimeSinceStartup:F2}");
        dayCount++;
        StartMorning();
    }

    public void StartMorning()
    {
        Debug.Log($"[GameManager] StartMorning invoked at {Time.realtimeSinceStartup:F2} - restoring timeScale and starting day {dayCount}");

        // Ensure the time scale is normal when a new day starts (protect against lingering pauses)
        if (Time.timeScale == 0f)
        {
            Debug.LogWarning("[GameManager] Time.timeScale was 0 on StartMorning — forcing to 1");
            Time.timeScale = 1f;
        }

        State = GameState.Morning;

        background.SetMorning();
        tileGenerator.SwitchToDayChunks();

        // Resets world and spawns the Day Start Chunk at X=0
        tileGenerator.SpawnMorningStart();

        dayTimeManager.gameObject.SetActive(true);
        dayTimeManager.StartNewDay();
        OnDayChanged?.Invoke(dayCount);
        DayStartEvent.Invoke();

        Debug.Log($"[GameManager] Morning started. State={State} Day={dayCount}");
    }

    public void OnDayEnded()
    {
        Debug.Log($"[GameManager] OnDayEnded invoked at {Time.realtimeSinceStartup:F2}");
        State = GameState.ShopArrival;
        // Resets world and spawns the Shop at X=0
        tileGenerator.SpawnShopChunk();
    }

    public void StartNight()
    {
        Debug.Log($"[GameManager] StartNight invoked at {Time.realtimeSinceStartup:F2}");
        State = GameState.Night;
        background.SetNight();
        tileGenerator.SwitchToNightChunks();

        // Resets world and spawns the House/Night chunk at X=0
        tileGenerator.SpawnNightChunk();
    }

}