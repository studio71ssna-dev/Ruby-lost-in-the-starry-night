using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Singleton;
using UnityEngine.Events;

public class GardenManager : SingletonPersistent
{
    // This connects to the GetInstance<T> logic in your uploaded file
    public static GardenManager Instance => GetInstance<GardenManager>();

    [Header("Grid Settings")]
    public int rows = 5;
    public int cols = 5;
    public GameObject cellPrefab;
    public Sprite grassSprite;

    [Header("Flower Data")]
    public List<ItemData> possibleFlowers;

    // This dictionary saves your garden layout across scene loads
    private Dictionary<int, ItemData> plantedGardenMap = new Dictionary<int, ItemData>();

    protected override void OnAwake()
    {
        base.OnAwake(); // Essential to keep the Singleton logic working
        Debug.Log("Garden Manager Initialized");
    }

    public void GenerateNewGardenData()
    {
        plantedGardenMap.Clear();

        List<ItemData> owned = new List<ItemData>();
        foreach (var f in possibleFlowers)
        {
            if (InventoryManager.Instance.HasItem(f))
                owned.Add(f);
        }

        for (int i = 0; i < rows * cols; i++)
        {
            // 30% chance to plant a flower if you own it
            if (owned.Count > 0 && Random.value < 0.3f)
            {
                plantedGardenMap[i] = owned[Random.Range(0, owned.Count)];
            }
        }
    }

    public void ConstructUI(Transform gridContainer)
    {
        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        for (int i = 0; i < rows * cols; i++)
        {
            GameObject cell = Instantiate(cellPrefab, gridContainer);
            Image img = cell.GetComponent<Image>();
            cell.transform.localScale = Vector3.one;

            if (plantedGardenMap.ContainsKey(i))
                img.sprite = plantedGardenMap[i].gardenSprite;
            else
                img.sprite = grassSprite;
        }
    }
}