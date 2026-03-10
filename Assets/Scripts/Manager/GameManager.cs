using Cysharp.Threading.Tasks;
using Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;


// Now inheriting from the generic version we just fixed
public class GameManager : SingletonPersistent
{
    public enum GameState
    {
        DayTime,
        Shop,
        NightTime,
        Quiz,
        Garden,
        GameOver
    }

    public GameState gamestate;
    public static GameManager Instance => GetInstance<GameManager>();

    [SerializeField] private GameObject shopPanel; // Drag your inactive Shop UI here

    public async UniTask EnterShopPhase()
    {
        gamestate = GameState.Shop;

        // 1. Show the UI
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);

            // 2. Use the ShopManager to handle the shopping logic
            // We 'await' this so the game doesn't move to Night until shopping is done
            await shopPanel.GetComponent<ShopManager>().StartShopping();
        }

        // 3. Once StartShopping() finishes, move to the next state
        TransitionToNight();
    }

    public async UniTask TransitionToNight()
    {
        gamestate = GameState.NightTime;
        SceneManager.LoadScene("Test_Music_Level");
    }

}