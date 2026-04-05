using UnityEngine;
using Singleton;

// FIX: Was using a raw manual singleton (Instance = this in Awake) inconsistent with
// every other manager in the project. Converted to SingletonPersistent so it gets
// DontDestroyOnLoad, duplicate-detection, and the shared GetInstance<T>() accessor
// automatically, just like AudioManager, ParticleManager, etc.
public class TileObjectSpawner : SingletonPersistent
{
    public static TileObjectSpawner Instance => GetInstance<TileObjectSpawner>();

    [Header("Prefabs")]
    public GameObject[] flowerPrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject wolfPrefab;

    [Header("Spawn Chances")]
    [Range(0, 1)] public float flowerChance = 0.6f;
    [Range(0, 1)] public float obstacleChance = 0.35f;
    [Range(0, 1)] public float wolfChance = 0.15f;

    public void Populate(TileChunk chunk)
    {
        SpawnPoint[] points = chunk.GetSpawnPoints();

        foreach (SpawnPoint point in points)
        {
            switch (point.spawnType)
            {
                case SpawnType.Flower:
                    TrySpawn(point, flowerPrefabs, flowerChance, chunk.transform);
                    break;

                case SpawnType.Obstacle:
                    TrySpawn(point, obstaclePrefabs, obstacleChance, chunk.transform);
                    break;

                case SpawnType.Wolf:
                    if (Random.value < wolfChance)
                        Spawn(wolfPrefab, point.transform, chunk.transform);
                    break;
            }
        }
    }

    void TrySpawn(SpawnPoint point, GameObject[] prefabs, float chance, Transform parent)
    {
        if (prefabs.Length == 0) return;

        if (Random.value < chance)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Spawn(prefab, point.transform, parent);
        }
    }

    void Spawn(GameObject prefab, Transform spawnPoint, Transform parent)
    {
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity, parent);
        obj.tag = "Generated";
    }
}