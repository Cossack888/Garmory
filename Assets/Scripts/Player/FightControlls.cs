using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightControlls : MonoBehaviour
{
    private InputManager inputManager;
    public Action<AnimationType> OnAttack;
    public Action<bool> OnDefend;
    public Action<bool> OnToggleWeapon;
    private bool armed;
    private Health health;
    private Coroutine automaticDefense;
    private GameObject weapon;
    [SerializeField] private GameObject shield;
    private ItemEquipper itemEquipper;
    public enum AttackMode
    {
        Single,
        Combo,
        Reckless
    }
    public AttackMode attackMode = AttackMode.Single;
    private void OnEnable()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            inputManager.OnAttack += Attack;
            inputManager.OnDefend += Defend;
            inputManager.OnNumericButtonPressed += SwitchMode;
            inputManager.OnToggleWeapon += ToggleWeapon;
        }
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += DisableWhenDead;
        }
    }
    private void OnDisable()
    {
        if (inputManager != null)
            inputManager.OnAttack -= Attack;
        inputManager.OnDefend -= Defend;
        inputManager.OnNumericButtonPressed -= SwitchMode;
        inputManager.OnToggleWeapon -= ToggleWeapon;
    }
    public void SetWeapon()
    {
        itemEquipper = GetComponent<ItemEquipper>();
        if (itemEquipper != null)
        {
            foreach (GameObject item in itemEquipper.CurrentEquippedItems)
            {
                if (item.GetComponent<ItemComponent>().item.itemType == ItemType.Weapon)
                {
                    weapon = item;
                }
            }
        }
        if (weapon != null)
        {
            ToggleWeapon();
        }
    }
    private void DisableWhenDead()
    {
        this.enabled = false;
    }
    public void SwitchMode(int number)
    {
        switch (number)
        {
            case 0:
                attackMode = AttackMode.Single;
                break;
            case 1:
                attackMode = AttackMode.Combo;
                break;
            case 2:
                attackMode = AttackMode.Reckless;
                break;
            default:
                break;
        }
    }
    public void ToggleWeapon()
    {
        if (weapon == null)
        {
            return;
        }
        armed = !armed;
        shield.SetActive(armed);
        weapon.SetActive(armed);
        OnToggleWeapon?.Invoke(!armed);
    }
    public void Defend(bool mode)
    {
        if (armed)
        {
            OnDefend?.Invoke(mode);
        }
    }
    public void AutomaticDefense(Transform attacker)
    {
        if (armed)
        {
            transform.LookAt(new Vector3(attacker.position.x, transform.position.y, attacker.position.z));
            OnDefend?.Invoke(true);
            if (automaticDefense == null)
            {
                automaticDefense = StartCoroutine(StopAutomaticDefense());
            }
        }
    }
    public IEnumerator StopAutomaticDefense()
    {
        yield return new WaitForSeconds(1);
        OnDefend?.Invoke(false);
        automaticDefense = null;
    }
    public void Attack()
    {
        ChooseAttackMode(attackMode);

    }
    public void ChooseAttackMode(AttackMode mode)
    {
        if (armed)
        {
            switch (mode)
            {
                case AttackMode.Single:
                    OnAttack?.Invoke(AnimationType.SimpleAttack);
                    break;
                case AttackMode.Combo:
                    OnAttack?.Invoke(AnimationType.ComboAttack);
                    break;
                case AttackMode.Reckless:
                    OnAttack?.Invoke(AnimationType.JumpAttack);
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case AttackMode.Single:
                    OnAttack?.Invoke(AnimationType.UnarmedSingle);
                    break;
                case AttackMode.Combo:
                    OnAttack?.Invoke(AnimationType.UnarmedCombo);
                    break;
            }
        }
    }
}
