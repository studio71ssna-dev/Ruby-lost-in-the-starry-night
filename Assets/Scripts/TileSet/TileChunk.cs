using UnityEngine;

public class TileChunk : MonoBehaviour
{
    private SpawnPoint[] spawnPoints;

    void Awake()
    {
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public SpawnPoint[] GetSpawnPoints()
    {
        return spawnPoints;
    }

    public void ClearGeneratedObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (child.CompareTag("Generated"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}