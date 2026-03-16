using System.Collections.Generic;
using UnityEngine;

public class GroundTileGenerator : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Tile Sets")]
    public GameObject[] morningTiles;
    public GameObject[] dayTiles;
    public GameObject[] nightTiles;

    [Header("Special Tiles")]
    public GameObject shopTile;

    [Header("Tile Settings")]
    public float tileLength = 20f;
    public int tilesOnScreen = 6;

    private float spawnX = 0f;

    private Queue<GameObject> activeTiles = new();

    private GameObject[] currentTiles;

    // -----------------------------
    // INITIALIZE
    // -----------------------------

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("GroundTileGenerator: Player reference missing!");
            return;
        }
    }

    // -----------------------------
    // UPDATE LOOP
    // -----------------------------

    void Update()
    {
        if (currentTiles == null) return;

        if (player.position.x > spawnX - (tilesOnScreen * tileLength))
        {
            SpawnTile();
            RemoveOldestTile();
        }
    }

    // -----------------------------
    // TILESET SWITCHING
    // -----------------------------

    public void SetTileSet(TileSetType type)
    {
        switch (type)
        {
            case TileSetType.Morning:
                currentTiles = morningTiles;
                break;

            case TileSetType.Day:
                currentTiles = dayTiles;
                break;

            case TileSetType.Night:
                currentTiles = nightTiles;
                break;
        }

        SpawnInitialTiles();
    }

    // -----------------------------
    // INITIAL WORLD BUILD
    // -----------------------------

    void SpawnInitialTiles()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    // -----------------------------
    // TILE SPAWNING
    // -----------------------------

    void SpawnTile()
    {
        if (currentTiles == null || currentTiles.Length == 0)
        {
            Debug.LogWarning("GroundTileGenerator: No tiles assigned!");
            return;
        }

        GameObject prefab = currentTiles[Random.Range(0, currentTiles.Length)];

        GameObject tile = TilePool.Instance.GetTile();

        tile.transform.position = new Vector3(spawnX, 0, 0);

        TileChunk chunk = tile.GetComponent<TileChunk>();

        if (chunk != null)
        {
            TileObjectSpawner.Instance.Populate(chunk);
        }

        activeTiles.Enqueue(tile);

        spawnX += tileLength;
    }

    // -----------------------------
    // REMOVE OLD TILE
    // -----------------------------

    void RemoveOldestTile()
    {
        if (activeTiles.Count == 0) return;

        GameObject oldTile = activeTiles.Dequeue();

        TilePool.Instance.ReturnTile(oldTile);
    }

    // -----------------------------
    // SHOP TILE
    // -----------------------------

    public void SpawnShopTile()
    {
        if (shopTile == null)
        {
            Debug.LogWarning("Shop tile not assigned!");
            return;
        }

        GameObject tile = Instantiate(
            shopTile,
            new Vector3(spawnX, 0, 0),
            Quaternion.identity
        );

        activeTiles.Enqueue(tile);

        spawnX += tileLength;
    }

    // -----------------------------
    // WORLD RESET
    // -----------------------------

    public void ResetWorld()
    {
        while (activeTiles.Count > 0)
        {
            GameObject tile = activeTiles.Dequeue();
            TilePool.Instance.ReturnTile(tile);
        }

        spawnX = 0f;
    }
}