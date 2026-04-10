using System.Collections.Generic;
using UnityEngine;

public class GroundTileGenerator : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Tile Sets")]
    public GameObject[] morningTiles;
    public GameObject[] dayTiles;
    public GameObject[] nightTiles;

    [Header("Special Tiles")]
    public GameObject shopTile;

    [Header("Generation Settings")]
    public int tilesOnScreen = 6;
    public float spawnAheadDistance = 60f;
    public float removeDistance = 120f;

    private float spawnX = 0f;
    private bool isScrollingEnabled = false;


    private Queue<(GameObject instance, GameObject prefab)> activeTiles = new();
    private GameObject[] currentTiles;
    private GameObject spawnedShopTile;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("GroundTileGenerator: Player reference missing!");
            return;
        }

        TilePool.Instance.WarmUp(morningTiles);
        TilePool.Instance.WarmUp(dayTiles);
        TilePool.Instance.WarmUp(nightTiles);
    }

    void Update()
    {
        if (!isScrollingEnabled) return;
        if (currentTiles == null) return;

        if (player.position.x + spawnAheadDistance > spawnX)
        {
            SpawnTile();
        }

        RemoveOldTiles();
    }

    // ------------------------------------------------
    // MORNING START
    // ------------------------------------------------

    public void SpawnSingleMorningTile()
    {
        isScrollingEnabled = false;
        currentTiles = morningTiles;

        ClearAllTiles();

        spawnX = player.position.x;

        SpawnTile();
    }

    public void SpawnSingleNightTile()
    {
        // 1. Stop infinite scrolling so no more tiles spawn
        isScrollingEnabled = false;

        currentTiles = nightTiles;

        // 2. Spawn exactly ONE night tile right where the shop tile ends
        SpawnTile();
    }

    // ------------------------------------------------
    // CHANGE TILE SET
    // ------------------------------------------------

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

        isScrollingEnabled = true;
    }

    // ------------------------------------------------
    // INITIAL SPAWN
    // ------------------------------------------------

    void SpawnInitialTiles()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    // ------------------------------------------------
    // SPAWN TILE
    // ------------------------------------------------

    void SpawnTile()
    {
        if (currentTiles == null || currentTiles.Length == 0)
        {
            Debug.LogWarning("No tiles assigned!");
            return;
        }

        GameObject prefab = currentTiles[Random.Range(0, currentTiles.Length)];

        GameObject tile = TilePool.Instance.GetTile(prefab);

        TileChunk chunk = tile.GetComponent<TileChunk>();

        if (chunk == null)
        {
            Debug.LogError("TileChunk missing on prefab!");
            return;
        }

        Vector3 startOffset = chunk.StartPoint.localPosition;

        tile.transform.position = new Vector3(
            spawnX - startOffset.x,
            0,
            0
        );

        TileObjectSpawner.Instance.Populate(chunk);

        spawnX = chunk.GetEndX();

        activeTiles.Enqueue((tile, prefab));
    }

    // ------------------------------------------------
    // REMOVE OLD TILES
    // ------------------------------------------------

    void RemoveOldTiles()
    {
        if (activeTiles.Count == 0) return;

        var (instance, prefab) = activeTiles.Peek();

        if (player.position.x - instance.transform.position.x > removeDistance)
        {
            activeTiles.Dequeue();

            TileChunk chunk = instance.GetComponent<TileChunk>();

            if (chunk != null)
                chunk.ClearGeneratedObjects();

            TilePool.Instance.ReturnTile(instance, prefab);
        }
    }

    // ------------------------------------------------
    // SHOP TILE
    // ------------------------------------------------

    public void SpawnShopTile()
    {
        if (shopTile == null)
        {
            Debug.LogWarning("Shop tile missing!");
            return;
        }

        // Clean up any previously spawned shop tile first
        if (spawnedShopTile != null)
            Destroy(spawnedShopTile);

        spawnedShopTile = Instantiate(shopTile, new Vector3(spawnX, 0, 0), Quaternion.identity);

        TileChunk chunk = spawnedShopTile.GetComponent<TileChunk>();

        if (chunk != null)
            spawnX = chunk.GetEndX();
    }

    // ------------------------------------------------
    // CLEAR WORLD
    // ------------------------------------------------

    void ClearAllTiles()
    {
        while (activeTiles.Count > 0)
        {
            var (instance, prefab) = activeTiles.Dequeue();

            TileChunk chunk = instance.GetComponent<TileChunk>();

            if (chunk != null)
                chunk.ClearGeneratedObjects();

            // FIX: No longer need a null-prefab check here — shop tile is handled separately
            TilePool.Instance.ReturnTile(instance, prefab);
        }

        // Also destroy the shop tile if it exists
        if (spawnedShopTile != null)
        {
            Destroy(spawnedShopTile);
            spawnedShopTile = null;
        }
    }

    public void ResetWorld()
    {
        ClearAllTiles();
    }
}