using UnityEngine;

public abstract class Statistics : MonoBehaviour
{
    public int Damage { get; private set; }
    public int HealthPoints { get; private set; }
    public int Defense { get; private set; }
    public float LifeSteal { get; private set; }
    public float CriticalStrikeChance { get; private set; }
    public float AttackSpeed { get; private set; }
    public float MovementSpeed { get; private set; }
    public float Luck { get; private set; }

    public enum StatNames
    { Damage, HealthPoints, Defense, LifeSteal, CriticalStrikeChance, AttackSpeed, MovementSpeed, Luck }
    public void SetDamage(int value) => Damage = value;
    public void SetHealthPoints(int value) => HealthPoints = value;
    public void SetDefense(int value) => Defense = value;
    public void SetLifeSteal(float value) => LifeSteal = value;
    public void SetCriticalStrikeChance(float value) => CriticalStrikeChance = value;
    public void SetAttackSpeed(float value) => AttackSpeed = value;
    public void SetMovementSpeed(float value) => MovementSpeed = value;
    public void SetLuck(float value) => Luck = value;
    public void ChangeStatistic(StatNames statName, float value)
    {
        switch (statName)
        {
            case StatNames.Damage: SetDamage(Damage + (int)value); break;
            case StatNames.HealthPoints: SetHealthPoints(HealthPoints + (int)value); break;
            case StatNames.Defense: SetDefense(Defense + (int)value); break;
            case StatNames.LifeSteal: SetLifeSteal(LifeSteal + value); break;
            case StatNames.CriticalStrikeChance: SetCriticalStrikeChance(CriticalStrikeChance + value); break;
            case StatNames.AttackSpeed: SetAttackSpeed(AttackSpeed + value); break;
            case StatNames.MovementSpeed: SetMovementSpeed(MovementSpeed + value); break;
            case StatNames.Luck: SetLuck(Luck + value); break;
        }
    }
}