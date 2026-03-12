using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Slider pressureBar;
    [Header("Bar Behaviour")]
    [SerializeField] private float fillSpeed = 0.1f;

    [Header("Hit Reductions")]
    [SerializeField] private float perfectReduction = 0.2f;
    [SerializeField] private float goodReduction = 0.1f;

    private bool barActive = false;

    private void Update()
    {
        if (!barActive || pressureBar == null) return;

        pressureBar.value = Mathf.Clamp01(
            pressureBar.value + fillSpeed * Time.deltaTime
        );
    }

    public void Perfect()
    {
        ReduceBar(perfectReduction);
    }

    public void Good()
    {
        ReduceBar(goodReduction);
    }

    public void Miss()
    {
        // intentionally empty
    }

    public void ActivateBarUI()
    {
        if (pressureBar == null) return;
        barActive=true;
        pressureBar.gameObject.SetActive(true);
    }
    private void ReduceBar(float amount)
    {
        if (!barActive || pressureBar == null) return;

        pressureBar.value = Mathf.Clamp01(pressureBar.value - amount);
    }
}