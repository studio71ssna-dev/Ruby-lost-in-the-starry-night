using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance;

    [SerializeField] private Image[] hearts;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < current;
        }
    }
}