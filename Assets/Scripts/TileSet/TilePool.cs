using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    public static TilePool Instance;

    [SerializeField] private int initialPoolSizePerPrefab = 4;

    // One queue per prefab
    private Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        Instance = this;
    }

    // Pre-warm the pool for a set of prefabs
    public void WarmUp(GameObject[] prefabs)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab == null) continue;
            if (!pools.ContainsKey(prefab))
                pools[prefab] = new Queue<GameObject>();

            while (pools[prefab].Count < initialPoolSizePerPrefab)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pools[prefab].Enqueue(obj);
            }
        }
    }

    public GameObject GetTile(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        if (pools[prefab].Count > 0)
        {
            GameObject tile = pools[prefab].Dequeue();
            tile.SetActive(true);
            return tile;
        }

        // Pool empty — instantiate a new one
        return Instantiate(prefab);
    }

    public void ReturnTile(GameObject tile, GameObject sourcePrefab)
    {
        TileChunk chunk = tile.GetComponent<TileChunk>();
        if (chunk != null) chunk.ClearGeneratedObjects();

        tile.SetActive(false);

        if (sourcePrefab == null)
        {
            Debug.LogError("ReturnTile called with NULL prefab!");
            return;
        }

        pools[sourcePrefab].Enqueue(tile);
    }
}