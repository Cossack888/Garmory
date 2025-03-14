using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int health;
    [SerializeField] private int maxHealth;
    public Action OnDeath;
    public Action<SingleAnimations> OnBlocked;
    public Action<SingleAnimations> OnHit;
    private bool isDead = false;
    private List<GameObject> ownWeapons = new List<GameObject>();
    private bool isBlocking;

    public void InitialiseWeapons()
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.SetOwner(gameObject);
            ownWeapons.Add(weapon.gameObject);
        }
    }
    public void InitialiseHealthPoints(int healthPoints)
    {
        maxHealth = healthPoints;
        health = maxHealth;
    }
    public bool IsBlocking() { return isBlocking; }
    public void SetIsBlocking(bool state)
    {
        isBlocking = state;
    }
    public void GainLife(int stolenLife)
    {
        if (isDead) return;
        health += stolenLife;
    }
    public void TakeDamage(int damage, bool blocked)
    {
        if (blocked)
        {
            OnBlocked?.Invoke(SingleAnimations.blocked);
        }
        if (!isDead && !blocked)
        {
            OnHit?.Invoke(SingleAnimations.hit);
        }
        health -= damage;


        if (health <= 0)
        {
            Die();
        }
    }
    public bool IsDead()
    {
        return isDead;
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();
    }
}
