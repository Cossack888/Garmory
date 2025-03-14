using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemEquipper : MonoBehaviour
{
    private List<GameObject> currentEquippedItems = new List<GameObject>();
    private List<GameObject> allItemsInHierarchy = new List<GameObject>();
    private List<Item> availableItems = new List<Item>();
    private Bones bones;
    private List<ItemType> itemLocationsOccupied = new List<ItemType>();
    [SerializeField] private GameObject originalCharacterPrefab;
    [SerializeField]
    private Transform leftHandTransform, rightHandTransform, neckTransform,
        leftFootTransform, rightFootTransform, bodyTransform, headTransform, fingerTransform;
    private ItemFetcher itemFetcher;
    private void OnEnable()
    {
        bones = GetComponentInChildren<Bones>();
        itemFetcher = FindFirstObjectByType<ItemFetcher>();
        if (itemFetcher != null)
            itemFetcher.OnItemsFetched += LoadItemsAtRuntime;
    }
    public List<GameObject> CurrentEquippedItems => currentEquippedItems;
    public List<GameObject> ItemsInHierarchy => allItemsInHierarchy;
    public List<Item> AvailableItems => availableItems;
    private void OnDisable()
    {
        if (itemFetcher != null)
            itemFetcher.OnItemsFetched -= LoadItemsAtRuntime;
    }
    public void LoadItemsAtRuntime(List<Item> newItems)
    {
        availableItems = newItems.ToList();
    }
    public void EquipItemByID(int itemID)
    {
        Item item = availableItems.FirstOrDefault(i => i.ID == itemID);
        if (item == null) return;

        GameObject hierarchyItem = allItemsInHierarchy.FirstOrDefault(obj =>
            obj.GetComponent<ItemComponent>()?.item.ID == itemID);
        UnequipItem(item.itemType);

        if (hierarchyItem != null)
        {
            hierarchyItem.SetActive(true);
            currentEquippedItems.Add(hierarchyItem);
        }
        else
        {
            EquipNewItem(item);
        }
    }

    private void EquipNewItem(Item item)
    {
        GameObject itemPrefab = CheckIfMeshRenderer(item);
        if (itemPrefab == null) return;

        GameObject newItem = Instantiate(itemPrefab);
        newItem.name = item.itemName;
        AttachItem(newItem, GetEquipTransform(item), item);

        currentEquippedItems.Add(newItem);
        allItemsInHierarchy.Add(newItem);
        itemLocationsOccupied.Add(item.itemType);
    }

    private GameObject CheckIfMeshRenderer(Item itemToCheck)
    {
        if (itemToCheck.itemPrefab.TryGetComponent(out SkinnedMeshRenderer mesh))
        {
            var bodyPartPair = new BodyPartPair { original = originalCharacterPrefab, prefab = mesh };
            if (!itemLocationsOccupied.Contains(itemToCheck.itemType))
            {
                bones.SetupBodyPartPair(bodyPartPair);
                bones.AttachBodyParts(itemToCheck);
                itemLocationsOccupied.Add(itemToCheck.itemType);
            }
            return null;
        }
        return itemToCheck.itemPrefab;
    }

    public Item GetEquippedItemByType(ItemType itemType)
    {
        foreach (GameObject itemObject in currentEquippedItems)
        {
            Item item = itemObject.GetComponent<ItemComponent>().item;

            if (item.itemType == itemType)
            {
                return item;
            }
        }
        return null;
    }

    private void AttachItem(GameObject itemObject, Transform parent, Item item)
    {
        itemObject.transform.SetParent(parent);
        itemObject.transform.localPosition = item.position;
        itemObject.transform.localRotation = Quaternion.Euler(item.rotation);
        itemObject.transform.localScale = item.scale;

        var itemComponent = itemObject.GetComponent<ItemComponent>() ?? itemObject.AddComponent<ItemComponent>();
        itemComponent.item = item;

    }

    private Transform GetEquipTransform(Item item)
    {
        return item.itemType switch
        {
            ItemType.Armor => bodyTransform,
            ItemType.Helmet => headTransform,
            ItemType.Necklace => neckTransform,
            ItemType.Ring => fingerTransform,
            ItemType.Weapon => rightHandTransform,
            _ => bodyTransform
        };
    }
    public void UnequipByID(int ID)
    {
        for (int i = 0; i < availableItems.Count; i++)
        {
            if (availableItems[i].ID == ID)
            {

                UnequipItem(availableItems[i].itemType);
            }
        }
    }

    public void UnequipItem(ItemType itemType)
    {
        foreach (GameObject equippedItem in currentEquippedItems.ToList())
        {
            ItemComponent itemComponent = equippedItem.GetComponent<ItemComponent>();
            if (itemComponent != null && itemComponent.item.itemType == itemType)
            {

                equippedItem.SetActive(false);

                currentEquippedItems.Remove(equippedItem);
                itemLocationsOccupied.Remove(itemType);
            }
        }
    }
    public void DestroyUnequippedItems()
    {
        foreach (GameObject itemObject in allItemsInHierarchy.ToList())
        {
            if (!currentEquippedItems.Contains(itemObject))
            {
                Destroy(itemObject);
            }
        }
    }
}
