using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public int ID;
    public Sprite itemIcon;
    public Sprite itemRarityIcon;
    public GameObject itemPrefab;
    public ItemType itemType;
    [Header("Transform Adjustments")]
    public Vector3 scale = Vector3.one;
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Dictionary<StatisticMod, float> stats = new Dictionary<StatisticMod, float>();
    public Rarity rarity;
    public string GetGreatestStatName()
    {
        if (stats.Count == 0)
            return "No stats available";
        StatisticMod greatestStat = StatisticMod.Damage;
        float greatestValue = float.MinValue;

        foreach (var stat in stats)
        {
            if (stat.Value > greatestValue)
            {
                greatestStat = stat.Key;
                greatestValue = stat.Value;
            }
        }
        return greatestStat.ToString();
    }
}

public enum StatisticMod
{
    Damage, HealthPoints, Defense, LifeSteal, CriticalStrikeChance, AttackSpeed, MovementSpeed, Luck
}
public enum Rarity
{
    Common, Uncommon, Rare, Epic, Legendary
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

