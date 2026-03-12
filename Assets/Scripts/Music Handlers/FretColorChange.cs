using UnityEngine;
using UnityEngine.UI;

public class FretColorChange : MonoBehaviour
{
    [SerializeField] private Image[] lane_HitCirclesImages;
    [SerializeField] private Color[] colors;

    private bool[] fretPressed = new bool[3];
    private int activeFret = -1;

    // Store delegate references so we can unsubscribe safely
    private System.Action<bool> fret1Delegate;
    private System.Action<bool> fret2Delegate;
    private System.Action<bool> fret3Delegate;

    private void OnEnable()
    {
        if (InputHandler.Instance == null) return;

        // Assign delegates
        fret1Delegate = (p) => HandleFret(0, p);
        fret2Delegate = (p) => HandleFret(1, p);
        fret3Delegate = (p) => HandleFret(2, p);

        // Subscribe
        InputHandler.Instance.OnFret1 += fret1Delegate;
        InputHandler.Instance.OnFret2 += fret2Delegate;
        InputHandler.Instance.OnFret3 += fret3Delegate;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance == null) return;

        // Unsubscribe safely
        InputHandler.Instance.OnFret1 -= fret1Delegate;
        InputHandler.Instance.OnFret2 -= fret2Delegate;
        InputHandler.Instance.OnFret3 -= fret3Delegate;
    }

    private void HandleFret(int fretIndex, bool isPressed)
    {
        fretPressed[fretIndex] = isPressed;

        if (isPressed)
        {
            if (activeFret == -1)
            {
                activeFret = fretIndex;
                ChangeColor(fretIndex);
            }
        }
        else
        {
            if (activeFret == fretIndex)
            {
                activeFret = GetNextPressedFret();

                if (activeFret != -1)
                    ChangeColor(activeFret);
                else
                    ResetColor();
            }
        }
    }

    private int GetNextPressedFret()
    {
        for (int i = 0; i < fretPressed.Length; i++)
        {
            if (fretPressed[i])
                return i;
        }

        return -1;
    }

    private void ChangeColor(int fretIndex)
    {
        Color c = colors[fretIndex+1];

        foreach (var img in lane_HitCirclesImages)
            img.color = c;
    }

    private void ResetColor()
    {
        foreach (var img in lane_HitCirclesImages)
            img.color = colors[0]; // default open fret color
    }
}