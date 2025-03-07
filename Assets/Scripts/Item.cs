using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public ItemType itemType;
    [Header("Transform Adjustments")]
    public Vector3 scale = Vector3.one;
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
}

public enum ItemType
{
    Armor,
    Boots,
    Helmet,
    Necklace,
    Ring,
    Weapon
}

