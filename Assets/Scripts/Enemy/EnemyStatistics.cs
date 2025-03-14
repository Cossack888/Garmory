using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatistics : Statistics
{
    private Health health;
    private void Awake()
    {
        health = GetComponent<Health>();
        ChangeStatistic(StatNames.Damage, Random.Range(10, 20));
        ChangeStatistic(StatNames.HealthPoints, 100);
        ChangeStatistic(StatNames.Luck, Random.Range(10, 20));
        ChangeStatistic(StatNames.Defense, Random.Range(10, 20));
        ChangeStatistic(StatNames.AttackSpeed, Random.Range(10, 20));
        ChangeStatistic(StatNames.CriticalStrikeChance, Random.Range(10, 20));
        health.InitialiseHealthPoints(HealthPoints);
    }
}
