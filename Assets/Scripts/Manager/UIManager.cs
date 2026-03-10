using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Singleton;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIManager : SingletonPersistent
{
    public static UIManager Instance => GetInstance<UIManager>();

    [Header("Flower UI")]
    [SerializeField] private TextMeshProUGUI flowerText;
    [SerializeField] private Image flowerIcon;

    [Header("Timer UI")]
    [SerializeField] private Image timerFill;
    [SerializeField] private Gradient timerColorGradient;

    // Call this whenever Ruby picks up a flower
    public void UpdateFlowerCount(int count)
    {
        flowerText.text = count.ToString();

        // Quick "Punch" effect for juice
        flowerIcon.transform.localScale = Vector3.one * 1.2f;
        flowerIcon.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    // Call this from DayTimeManager to update the bar
    public void UpdateTimer(float normalizedTime)
    {
        timerFill.fillAmount = normalizedTime;
        timerFill.color = timerColorGradient.Evaluate(normalizedTime);
    }
}