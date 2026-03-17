using UnityEngine;

public enum ItemType { Upgrade, Flower } // <-- Add this enum

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    [TextArea] public string description;
    public int cost;
    public Sprite icon;
    public Sprite gardenSprite;
    public float statModifier;
}