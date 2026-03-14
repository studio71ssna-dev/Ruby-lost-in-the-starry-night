using UnityEngine;
using TMPro;

public class DayCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;

    private void Start()
    {
        UpdateDay(GameManager.Instance.DayCount);

        GameManager.Instance.OnDayChanged += UpdateDay;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged -= UpdateDay;
    }

    private void UpdateDay(int day)
    {
        dayText.text = $"DAY {day}";
    }
}