using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    public static TilePool Instance;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab);
            tile.SetActive(false);
            pool.Enqueue(tile);
        }
    }

    public GameObject GetTile()
    {
        if (pool.Count > 0)
        {
            GameObject tile = pool.Dequeue();
            tile.SetActive(true);
            return tile;
        }

        return Instantiate(tilePrefab);
    }

    public void ReturnTile(GameObject tile)
    {
        TileChunk chunk = tile.GetComponent<TileChunk>();

        if (chunk != null)
            chunk.ClearGeneratedObjects();

        tile.SetActive(false);
        pool.Enqueue(tile);
    }
}