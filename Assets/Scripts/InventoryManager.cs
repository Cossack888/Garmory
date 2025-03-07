using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public Action<int> OnItemEquipped;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InputManager.Instance.OnNumericButtonPressed += EquipItem;
    }
    public void EquipItem(int number)
    {
        if (OnItemEquipped != null)
        {
            OnItemEquipped.Invoke(number);
        }
        else
        {
            Debug.Log("Can not equip item at number " + number);
        }
    }


}