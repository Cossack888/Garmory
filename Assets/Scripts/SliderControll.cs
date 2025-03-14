using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Statistics;
using static UnityEditor.Progress;

public class SliderControll : MonoBehaviour
{
    private PlayerStatistics playerStats;
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private Transform sliderParent;
    private Dictionary<StatNames, Slider> statSliders = new Dictionary<StatNames, Slider>();

    private void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStatistics>();
        CreateSliders();
        playerStats.OnStatsChanged += UpdateSliders;
        UpdateSliders();
    }

    private void OnDestroy()
    {
        playerStats.OnStatsChanged -= UpdateSliders;
    }
    private void CreateSliders()
    {
        foreach (StatNames stat in System.Enum.GetValues(typeof(StatNames)))
        {
            GameObject sliderObject = Instantiate(sliderPrefab, sliderParent);
            sliderObject.name = stat.ToString();

            Slider slider = sliderObject.GetComponent<Slider>();
            TMP_Text sliderLabel = sliderObject.GetComponentInChildren<TMP_Text>();
            if (sliderLabel != null)
            {
                sliderLabel.text = stat.ToString();
                slider.maxValue = 150;
            }

            statSliders[stat] = slider;
        }
    }
    private void UpdateSliders()
    {
        foreach (var stat in statSliders.Keys)
        {
            float statValue = GetStatValue(stat);
            statSliders[stat].value = statValue;

            bool isPercentage = stat.ToString().Contains("Luck") || stat.ToString().Contains("LifeSteal")
                || stat.ToString().Contains("Critical") || stat.ToString().Contains("Speed");
            string displayValue = isPercentage
                ? $"{statValue:F0}%"
                : statValue.ToString("F0");

            statSliders[stat].GetComponentInChildren<TMP_Text>().text =
                NormalisingUtils.AddSpacesBeforeCapitals(stat.ToString()) + "   " + displayValue;
        }
    }
    public void UpdateSliderColorAndValues(StatNames statName, Item hoveredItem)
    {
        if (statSliders.ContainsKey(statName))
        {
            Slider slider = statSliders[statName];
            StatisticMod statMod = (StatisticMod)Enum.Parse(typeof(StatisticMod), statName.ToString());
            playerStats.SimulateNewStatistics(hoveredItem);
            Color newColor = playerStats.CompareStatistics(playerStats.GetCurrentStats(), playerStats.GetTemporaryStats());
            float predictedStatValue = playerStats.GetTemporaryStats().ContainsKey(statName) ? playerStats.GetTemporaryStats()[statName] : 0f;
            slider.value = predictedStatValue;
            slider.fillRect.GetComponent<Image>().color = newColor;
            playerStats.ReturnToRegularStatisctics();
        }
    }


    public void ResetSliderColorAndValues()
    {
        foreach (Slider slider in statSliders.Values)
        {

            slider.fillRect.GetComponent<Image>().color = Color.white;
        }
        playerStats.ChangeItem();
    }


    private float GetStatValue(StatNames stat)
    {
        switch (stat)
        {
            case StatNames.Damage: return playerStats.Damage;
            case StatNames.HealthPoints: return playerStats.HealthPoints;
            case StatNames.Defense: return playerStats.Defense;
            case StatNames.LifeSteal: return playerStats.LifeSteal;
            case StatNames.CriticalStrikeChance: return playerStats.CriticalStrikeChance;
            case StatNames.AttackSpeed: return playerStats.AttackSpeed;
            case StatNames.MovementSpeed: return playerStats.MovementSpeed;
            case StatNames.Luck: return playerStats.Luck;
            default:

                return 0;
        }
    }
}
