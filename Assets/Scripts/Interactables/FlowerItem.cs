using UnityEngine;

public class FlowerItem : MonoBehaviour
{
    [Header("Data Asset")]
    public FlowerData data;
    public GameObject collectEffectPrefab; // Optional: Particle effect prefab for collection

    private void OnDestroy()
    {
        //play particle effect or sound effect here if needed
        //collectEffectPrefab.SetActive(false);

    }
}