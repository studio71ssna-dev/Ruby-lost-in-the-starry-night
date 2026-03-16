using UnityEngine;

public class TileObjectSpawner : MonoBehaviour
{
    public static TileObjectSpawner Instance;

    [Header("Prefabs")]
    public GameObject[] flowerPrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject wolfPrefab;

    [Header("Spawn Chances")]
    [Range(0, 1)] public float flowerChance = 0.6f;
    [Range(0, 1)] public float obstacleChance = 0.35f;
    [Range(0, 1)] public float wolfChance = 0.15f;

    void Awake()
    {
        Instance = this;
    }

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