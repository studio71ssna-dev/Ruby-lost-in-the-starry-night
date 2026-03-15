using System.Collections.Generic;
using UnityEngine;

public class GroundTileGenerator : MonoBehaviour
{
    [Header("Chunks")]
    public GameObject[] dayChunks;
    public GameObject[] nightChunks;
    public GameObject shopChunk;
    public GameObject nightStartChunk;

    [Header("Specific Chunks")]
    public GameObject dayStartChunk; // Assign your "Morning" start prefab

    public Transform playerTransform;

    public float spawnDistance = 20f;
    public float chunkLength = 30f;

    private GameObject[] activeSet;
    private List<GameObject> activeChunks = new();

    private float spawnX = 0f;

    void Start()
    {
        SwitchToDayChunks();

        for (int i = 0; i < 3; i++)
            SpawnRandomChunk();
    }

    void Update()
    {
        // Only spawn random chunks if we are in the Morning state
        if (GameManager.Instance.State != GameManager.GameState.Morning) return;

        if (playerTransform.position.x > spawnX - spawnDistance * 2)
        {
            SpawnRandomChunk();
            DeleteOldChunk();
        }
    }

    // ---------- MODE SWITCH ----------

    public void SwitchToDayChunks()
    {
        activeSet = dayChunks;
    }

    public void SwitchToNightChunks()
    {
        activeSet = nightChunks;
    }

    // ---------- SPAWNING ----------

    public void ForceSpawnNext(GameObject targetChunk)
    {
        // Keep the chunk the player is on (and maybe the one immediately next), destroy the rest
        while (activeChunks.Count > 2)
        {
            int lastIndex = activeChunks.Count - 1;
            Destroy(activeChunks[lastIndex]);
            activeChunks.RemoveAt(lastIndex);
        }

        // Recalculate spawnX based on the last remaining chunk
        if (activeChunks.Count > 0)
        {
            spawnX = activeChunks[activeChunks.Count - 1].transform.position.x + chunkLength;
        }

        // Spawn the requested chunk and track it! (This fixes the memory leak we discussed earlier)
        GameObject go = Instantiate(targetChunk, Vector3.right * spawnX, Quaternion.identity);
        activeChunks.Add(go);
        spawnX += chunkLength;
    }

    /*public void SpawnShopChunk()
    {
        ForceSpawnNext(shopChunk);
    }

    public void SpawnNightChunk()
    {
        ForceSpawnNext(nightStartChunk);
    }*/
    void SpawnRandomChunk()
    {
        if (activeSet == null || activeSet.Length == 0) return;

        int index = Random.Range(0, activeSet.Length);

        GameObject go =
            Instantiate(activeSet[index],
            Vector3.right * spawnX,
            Quaternion.identity);

        activeChunks.Add(go);
        spawnX += chunkLength;
    }

    void DeleteOldChunk()
    {
        if (activeChunks.Count > 5)
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
        }
    }


    public void ResetWorldToChunk(GameObject targetChunk)
    {
        // 1. Clear all current chunks
        foreach (GameObject chunk in activeChunks)
        {
            Destroy(chunk);
        }
        activeChunks.Clear();

        // 2. Reset the spawning logic to 0
        spawnX = 0f;

        // 3. Teleport the player back to the start (0, currentY, 0)
        playerTransform.position = new Vector3(0, playerTransform.position.y, 0);

        // 4. Spawn the single specific chunk
        GameObject go = Instantiate(targetChunk, Vector3.zero, Quaternion.identity);
        activeChunks.Add(go);

        // Set spawnX for the NEXT chunk if movement occurs
        spawnX = chunkLength;
    }

    // Simplified Helper Methods
    public void SpawnMorningStart() => ResetWorldToChunk(dayStartChunk);
    public void SpawnShopChunk() => ResetWorldToChunk(shopChunk);
    public void SpawnNightChunk() => ResetWorldToChunk(nightStartChunk);
}