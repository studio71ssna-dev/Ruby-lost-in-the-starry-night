using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public GameObject[] chunkPrefabs; // Your hand-designed Van Gogh segments
    public Transform playerTransform;
    public float spawnDistance = 20f; // How far ahead to spawn
    public float chunkLength = 30f;   // Length of each prefab

    private List<GameObject> activeChunks = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        // Spawn initial 3 chunks
        for (int i = 0; i < 3; i++) { SpawnChunk(); }
    }

    void Update()
    {
        // Check if player is close to the end of the current chunks
        if (playerTransform.position.x > spawnZ - (spawnDistance * 2))
        {
            SpawnChunk();
            DeleteOldChunk();
        }
    }

    void SpawnChunk()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        GameObject go = Instantiate(chunkPrefabs[randomIndex], transform.right * spawnZ, Quaternion.identity);
        activeChunks.Add(go);
        spawnZ += chunkLength;
    }

    void DeleteOldChunk()
    {
        if (activeChunks.Count > 4) // Keep a few behind the player just in case
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
        }
    }
}