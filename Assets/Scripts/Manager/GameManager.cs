using UnityEngine;
using Singleton;
using System.Collections.Generic;
using System.Collections;

// Now inheriting from the generic version we just fixed
public class GameManager : SingletonPersistent
{
    public static GameManager Instance => GetInstance<GameManager>();
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

    // We no longer need a custom 'gameManager' variable or 'Start' assignment
    // because the base SingletonPersistent<T> handles 'Instance' for us.
}