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

    [Header("Tuning")]
    [SerializeField] private int initialSpawnCount = 5;   // spawn more up-front to avoid gaps
    [SerializeField] private int maxActiveChunks = 8;     // keep more chunks alive before deleting old ones

    private GameObject[] activeSet;
    private List<GameObject> activeChunks = new();

    private float spawnX = 0f;

    void Start()
    {
        SwitchToDayChunks();

        Debug.Log($"[GroundTileGenerator] Start invoked at {Time.realtimeSinceStartup:F2} - initialSpawnCount={initialSpawnCount}");

        // Pre-spawn enough chunks to guarantee a buffer ahead of the player
        for (int i = 0; i < Mathf.Max(3, initialSpawnCount); i++)
            SpawnRandomChunk();

        Debug.Log($"[GroundTileGenerator] After initial spawn: spawnX={spawnX} activeChunks={activeChunks.Count}");
    }

    void Update()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[GroundTileGenerator] GameManager.Instance is null");
            return;
        }

        // Only spawn random chunks if we are in the Morning state
        if (GameManager.Instance.State != GameManager.GameState.Morning) 
        {
            // Helpful trace for diagnosing "delayed execution" between days
            // (log at reduced frequency)
            if (Time.frameCount %300 ==0)
                Debug.Log($"[GroundTileGenerator] Skipping spawn because state={GameManager.Instance.State} at realtime {Time.realtimeSinceStartup:F2}");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("[GroundTileGenerator] playerTransform is null");
            return;
        }

        // Use a while loop to catch up if the player advances multiple chunk lengths in a single frame.
        // This prevents the player running past the last spawned chunk when frame spikes or large movement occurs.
        int spawnCalls =0;
        while (playerTransform.position.x > spawnX - spawnDistance)
        {
            SpawnRandomChunk();
            DeleteOldChunk();
            spawnCalls++;
            if (spawnCalls >20)
            {
                Debug.LogError($"[GroundTileGenerator] Too many spawn calls in one frame, breaking out to avoid freeze (spawnX={spawnX}, playerX={playerTransform.position.x})");
                break;
            }
        }

        if (spawnCalls >0)
            Debug.Log($"[GroundTileGenerator] Spawn loop ran {spawnCalls} times at realtime {Time.realtimeSinceStartup:F2} - spawnX={spawnX} playerX={playerTransform.position.x} activeChunks={activeChunks.Count}");
    }

    // ---------- MODE SWITCH ----------

    public void SwitchToDayChunks()
    {
        activeSet = dayChunks;
        Debug.Log($"[GroundTileGenerator] Switched to DAY chunks at {Time.realtimeSinceStartup:F2}");
    }

    public void SwitchToNightChunks()
    {
        activeSet = nightChunks;
        Debug.Log($"[GroundTileGenerator] Switched to NIGHT chunks at {Time.realtimeSinceStartup:F2}");
    }

    // ---------- SPAWNING ----------

    public void ForceSpawnNext(GameObject targetChunk)
    {
        Debug.Log($"[GroundTileGenerator] ForceSpawnNext called at {Time.realtimeSinceStartup:F2} - clearing down to2 chunks");

        // Keep the chunk the player is on (and maybe the one immediately next), destroy the rest
        while (activeChunks.Count >2)
        {
            int lastIndex = activeChunks.Count -1;
            Destroy(activeChunks[lastIndex]);
            activeChunks.RemoveAt(lastIndex);
        }

        // Recalculate spawnX based on the last remaining chunk
        if (activeChunks.Count >0)
        {
            spawnX = activeChunks[activeChunks.Count -1].transform.position.x + chunkLength;
        }

        // Spawn the requested chunk and track it
        GameObject go = Instantiate(targetChunk, Vector3.right * spawnX, Quaternion.identity);
        activeChunks.Add(go);
        spawnX += chunkLength;

        Debug.Log($"[GroundTileGenerator] ForceSpawnNext spawned {targetChunk.name} at x={spawnX - chunkLength}");
    }

    void SpawnRandomChunk()
    {
        if (activeSet == null || activeSet.Length ==0)
        {
            Debug.LogWarning($"[GroundTileGenerator] activeSet is null or empty at {Time.realtimeSinceStartup:F2}");
            return;
        }

        int index = Random.Range(0, activeSet.Length);

        GameObject go =
            Instantiate(activeSet[index],
            Vector3.right * spawnX,
            Quaternion.identity);

        activeChunks.Add(go);
        spawnX += chunkLength;

        Debug.Log($"[GroundTileGenerator] Spawned chunk {activeSet[index].name} at x={spawnX - chunkLength} - new spawnX={spawnX} activeChunks={activeChunks.Count}");
    }

    void DeleteOldChunk()
    {
        // Remove oldest chunks until we're under the maxActiveChunks count.
        while (activeChunks.Count > maxActiveChunks)
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
            Debug.Log($"[GroundTileGenerator] Deleted old chunk. activeChunks now={activeChunks.Count}");
        }
    }


    public void ResetWorldToChunk(GameObject targetChunk)
    {
        Debug.Log($"[GroundTileGenerator] ResetWorldToChunk called at {Time.realtimeSinceStartup:F2} - target={targetChunk?.name}");

        //1. Clear all current chunks
        foreach (GameObject chunk in activeChunks)
        {
            Destroy(chunk);
        }
        activeChunks.Clear();

        //2. Reset the spawning logic to0
        spawnX =0f;

        //3. Teleport the player back to the start (0, currentY,0)
        if (playerTransform != null)
            playerTransform.position = new Vector3(0, playerTransform.position.y,0);

        //4. Spawn the single specific chunk
        GameObject go = Instantiate(targetChunk, Vector3.zero, Quaternion.identity);
        activeChunks.Add(go);

        // Set spawnX for the NEXT chunk if movement occurs
        spawnX = chunkLength;

        Debug.Log($"[GroundTileGenerator] World reset. spawnX={spawnX} activeChunks={activeChunks.Count}");
    }

    // Simplified Helper Methods
    public void SpawnMorningStart() { Debug.Log($"[GroundTileGenerator] SpawnMorningStart called at {Time.realtimeSinceStartup:F2}"); ResetWorldToChunk(dayStartChunk); }
    public void SpawnShopChunk() { Debug.Log($"[GroundTileGenerator] SpawnShopChunk called at {Time.realtimeSinceStartup:F2}"); ResetWorldToChunk(shopChunk); }
    public void SpawnNightChunk() { Debug.Log($"[GroundTileGenerator] SpawnNightChunk called at {Time.realtimeSinceStartup:F02}"); ResetWorldToChunk(nightStartChunk); }
}