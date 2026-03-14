using SingletonManagers;
using UnityEngine;

public class FlowerItem : MonoBehaviour
{
    [Header("Data Asset")]
    public FlowerData data;

    private void OnDestroy()
    {
        ParticleManager.Instance.PlayParticle("PickUp", transform.position,data.glowColor);
        AudioManager.Instance.Play("Pickup",transform.position);

    }
}