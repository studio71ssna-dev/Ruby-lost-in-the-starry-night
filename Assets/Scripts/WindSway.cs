using UnityEngine;
using DG.Tweening;

public class WindSway : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("How far the object bends (in degrees)")]
    public float swayAngle = 8f;

    [Tooltip("How long a single sway takes")]
    public float swayDuration = 2f;

    void Start()
    {
        // 1. Shift the starting angle slightly to the left
        transform.localRotation = Quaternion.Euler(0, 0, -swayAngle / 2f);

        // 2. Tween it to the right, creating a balanced back-and-forth swing
        transform.DOLocalRotate(new Vector3(0, 0, swayAngle / 2f), swayDuration)
            .SetEase(Ease.InOutSine)         // InOutSine gives that perfectly smooth, natural pendulum momentum
            .SetLoops(-1, LoopType.Yoyo)     // -1 means infinite loops. Yoyo makes it go back and forth
            .SetDelay(Random.Range(0f, 2f)); // Random delay so your field of flowers don't all move in perfect synchronization
    }
}