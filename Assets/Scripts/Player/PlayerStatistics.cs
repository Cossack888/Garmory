using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStatistics : Statistics
{
    private List<Item> equippedItems = new List<Item>();
    private List<Item> simulatedItems = new List<Item>();
    private Item tempItem;
    private Dictionary<StatNames, float> statValues = new Dictionary<StatNames, float>();
    private Dictionary<StatNames, float> tempStatValues = new Dictionary<StatNames, float>();
    private ItemEquipper equipper;
    private EquipmentPanelManager panelManager;
    public Action OnStatsChanged;
    private void Start()
    {
        CalculateAndSetStatistics(equippedItems, statValues);
        if (equippedItems.Count == 0)
        {
            RandomiseStatistics();
        }
        equipper = GetComponent<ItemEquipper>();
    }
    private void OnEnable()
    {
        panelManager = FindFirstObjectByType<EquipmentPanelManager>();
        if (panelManager != null)
            panelManager.OnButtonClicked += ChangeItem;
    }
    private void OnDisable()
    {
        if (panelManager != null)
            panelManager.OnButtonClicked -= ChangeItem;
    }
    public void CalculateAndSetStatistics(List<Item> Items, Dictionary<StatNames, float> statValues)
    {
        ResetStats();
        statValues.Clear();
        int totalDamage = 0;
        int totalHealthPoints = 0;
        int totalDefense = 0;
        float totalLifeSteal = 0;
        float totalCriticalStrikeChance = 0;
        float totalAttackSpeed = 0;
        float totalMovementSpeed = 0;
        float totalLuck = 0;

        foreach (var item in Items)
        {
            foreach (var stat in item.stats)
            {
                StatNames statName = GetStatNameFromMod(stat.Key);
                float value = stat.Value * GetRarityMultiplier(item.rarity);


                if (statValues.ContainsKey(statName))
                {
                    statValues[statName] += value;
                }
                else
                {
                    statValues.Add(statName, value);
                }


                switch (statName)
                {
                    case StatNames.Damage:
                        totalDamage += Mathf.FloorToInt(value);
                        break;
                    case StatNames.HealthPoints:
                        totalHealthPoints += Mathf.FloorToInt(value);
                        break;
                    case StatNames.Defense:
                        totalDefense += Mathf.FloorToInt(value);
                        break;
                    case StatNames.LifeSteal:
                        totalLifeSteal += value;
                        break;
                    case StatNames.CriticalStrikeChance:
                        totalCriticalStrikeChance += value;
                        break;
                    case StatNames.AttackSpeed:
                        totalAttackSpeed += value;
                        break;
                    case StatNames.MovementSpeed:
                        totalMovementSpeed += value;
                        break;
                    case StatNames.Luck:
                        totalLuck += value;
                        break;
                }
            }
        }
        SetDamage(totalDamage);
        SetHealthPoints(totalHealthPoints);
        SetDefense(totalDefense);
        SetLifeSteal(totalLifeSteal);
        SetCriticalStrikeChance(totalCriticalStrikeChance);
        SetAttackSpeed(totalAttackSpeed);
        SetMovementSpeed(totalMovementSpeed);
        SetLuck(totalLuck);
        LogStatistics();
    }


    public void SimulateNewStatistics(Item item)
    {
        List<Item> newSimulatedItems = new List<Item>(equippedItems);
        for (int i = 0; i < newSimulatedItems.Count; i++)
        {
            Item currentItem = newSimulatedItems[i];
            if (currentItem != null && currentItem.itemType == item.itemType)
            {
                newSimulatedItems[i] = item;
                break;
            }
        }
        simulatedItems = newSimulatedItems;
        CalculateAndSetStatistics(simulatedItems, tempStatValues);
    }

    public void ReturnToRegularStatisctics()
    {
        simulatedItems.Clear();
        tempStatValues.Clear();
    }

    public Color CompareStatistics(Dictionary<StatNames, float> equippedItemsStats, Dictionary<StatNames, float> simulatedItemsStats)
    {
        foreach (StatNames statNames in equippedItemsStats.Keys)
        {
            float currentStateOfStat = equippedItemsStats[statNames];
            foreach (StatNames temStatNames in simulatedItemsStats.Keys)
            {
                float tempStateOfStat = simulatedItemsStats[temStatNames];
                if (tempStateOfStat > currentStateOfStat)
                {
                    return Color.green;
                }
                else if (tempStateOfStat < currentStateOfStat)
                {
                    return Color.red;
                }
                else
                {
                    return Color.white;
                }
            }
        }
        return Color.white;
    }

    public float GetRarityMultiplier(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 0.75f;
            case Rarity.Uncommon:
                return 1.0f;
            case Rarity.Rare:
                return 1.25f;
            case Rarity.Epic:
                return 1.5f;
            case Rarity.Legendary:
                return 1.75f;
            default:
                throw new ArgumentOutOfRangeException(nameof(rarity), $"Unknown rarity: {rarity}");
        }
    }

    public void ChangeItem()
    {
        equippedItems.Clear();
        foreach (GameObject itemObj in equipper.CurrentEquippedItems)
        {
            equippedItems.Add(itemObj.GetComponent<ItemComponent>().item);
        }
        CalculateAndSetStatistics(equippedItems, statValues);
    }

    public Dictionary<StatNames, float> GetCurrentStats()
    {
        return statValues;
    }
    public Dictionary<StatNames, float> GetTemporaryStats()
    {
        return tempStatValues;
    }
    public void ResetStats()
    {
        SetDamage(0);
        SetHealthPoints(0);
        SetDefense(0);
        SetLifeSteal(0);
        SetCriticalStrikeChance(0);
        SetAttackSpeed(0);
        SetMovementSpeed(0);
        SetLuck(0);
    }

    private StatNames GetStatNameFromMod(StatisticMod mod)
    {
        switch (mod)
        {
            case StatisticMod.Damage: return StatNames.Damage;
            case StatisticMod.HealthPoints: return StatNames.HealthPoints;
            case StatisticMod.Defense: return StatNames.Defense;
            case StatisticMod.LifeSteal: return StatNames.LifeSteal;
            case StatisticMod.CriticalStrikeChance: return StatNames.CriticalStrikeChance;
            case StatisticMod.AttackSpeed: return StatNames.AttackSpeed;
            case StatisticMod.MovementSpeed: return StatNames.MovementSpeed;
            case StatisticMod.Luck: return StatNames.Luck;
            default: throw new System.ArgumentException("Unknown stat: " + mod);
        }
    }
    public void LogStatistics()
    {
        OnStatsChanged?.Invoke();
        string statsLog = $"Damage: {Damage}, HealthPoints: {HealthPoints}, Defense: {Defense}, " +
                          $"LifeSteal: {LifeSteal}, CriticalStrikeChance: {CriticalStrikeChance}, " +
                          $"AttackSpeed: {AttackSpeed}, MovementSpeed: {MovementSpeed}, Luck: {Luck}";
        Debug.Log(statsLog);
    }
    public void RandomiseStatistics()
    {
        ChangeStatistic(StatNames.Damage, Random.Range(10, 20));
        ChangeStatistic(StatNames.HealthPoints, Random.Range(20, 50));
        ChangeStatistic(StatNames.LifeSteal, Random.Range(10, 20));
        ChangeStatistic(StatNames.Luck, Random.Range(10, 20));
        ChangeStatistic(StatNames.Defense, Random.Range(10, 20));
        ChangeStatistic(StatNames.AttackSpeed, Random.Range(10, 20));
        ChangeStatistic(StatNames.CriticalStrikeChance, Random.Range(10, 20));
    }
}
