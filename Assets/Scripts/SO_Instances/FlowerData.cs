using UnityEngine;

[CreateAssetMenu(fileName = "NewFlower", menuName = "Inventory/Flower")]
public class FlowerData : ScriptableObject
{
    public string flowerName;
    public int value; // Different flowers = different values
    public Sprite icon;
    public Color glowColor = Color.white; // For visual variety
}