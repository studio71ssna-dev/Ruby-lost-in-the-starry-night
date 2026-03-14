using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public int cost;
    public Sprite icon;
    public Sprite gardenSprite;

    // You can add logic specific to the item here
    public float statModifier;
}