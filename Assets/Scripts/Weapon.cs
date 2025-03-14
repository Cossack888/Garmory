using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Collider col;
    private int damage;
    private float lifeSteal;
    private Health ownerHealth;
    private GameObject owner;
    private List<GameObject> ownerBody = new List<GameObject>();
    private Weapon[] allWeapons;
    private bool ownerSet;
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
        Statistics ownerStats = owner.GetComponent<Statistics>();
        if (ownerStats != null)
        {
            if (gameObject.CompareTag("Fist"))
            {
                damage = 5;
            }
            else
            {
                damage = ownerStats.Damage;
                lifeSteal = ownerStats.LifeSteal;
            }
        }

        foreach (Collider col in owner.GetComponentsInChildren<Collider>())
        {
            ownerBody.Add(col.gameObject);
        }
        ownerHealth = newOwner.GetComponent<Health>();
        allWeapons = owner.GetComponentsInChildren<Weapon>();
        ownerSet = true;
    }

    private void Start()
    {
        col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.Log("No collider attached");
        }
    }
    public void ToggleCollider(bool state)
    {
        if (col != null)
        {
            col.enabled = state;
        }
    }
    public bool CanBlock(Transform attackerTransform, Transform defenderTransform)
    {
        Vector3 directionToAttacker = (attackerTransform.position - defenderTransform.position).normalized;
        float angle = Vector3.Angle(defenderTransform.forward, directionToAttacker);
        return angle >= -45 && angle <= 45;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!ownerSet || ownerBody.Contains(other.gameObject)) return;
        Health targetHealth = other.GetComponentInParent<Health>();
        if (targetHealth == null) return;
        Statistics targetStats = other.GetComponentInParent<Statistics>();
        FightControlls playerControls = other.GetComponentInParent<FightControlls>();
        bool canBlock = CanBlock(this.owner.transform, other.transform);
        bool hasDefended = false;

        if (canBlock && targetStats != null)
        {
            float normalizedDefenseChance = NormalisingUtils.NormalizeStat(targetStats.Defense);
            hasDefended = (normalizedDefenseChance > Random.value);

            if (hasDefended && playerControls)
            {
                playerControls.AutomaticDefense(this.owner.transform);
                targetHealth.SetIsBlocking(true);
            }
        }
        int finalDamage = hasDefended ? Mathf.FloorToInt(damage / 3) : damage;
        targetHealth.TakeDamage(finalDamage, hasDefended);
        if (ownerHealth != null && !ownerHealth.IsDead())
        {
            float lifeStealAmount = (lifeSteal / 100f) * finalDamage;
            ownerHealth.GainLife(Mathf.FloorToInt(lifeStealAmount));
        }
        DisableAllWeaponColliders();
    }
    private void DisableAllWeaponColliders()
    {
        if (allWeapons != null)
        {
            foreach (Weapon weapon in allWeapons)
            {
                weapon.ToggleCollider(false);
            }
        }
    }
}
