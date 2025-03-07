using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemAttachment : MonoBehaviour
{
    [Header("Item Settings")]
    public List<Item> items = new List<Item>();
    private List<GameObject> currentEquippedItems = new List<GameObject>();

    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform neckTransform;
    [SerializeField] private Transform leftFootTransform;
    [SerializeField] private Transform rightFootTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform fingerTransform;

    private void Awake()
    {
        InventoryManager.Instance.OnItemEquipped += EquipItem;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemEquipped -= EquipItem;
    }

    public void LoadItemsAtRuntime(List<Item> newItems)
    {
        items.Clear();
        items.AddRange(newItems);
    }

    public void EquipItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= items.Count)
        {

            return;
        }

        Item item = items[itemIndex];

        bool alreadyEquipped = currentEquippedItems.Exists(equippedItem =>
        equippedItem.GetComponent<ItemComponent>().item.itemType == item.itemType);

        if (alreadyEquipped)
        {
            UnequipItem(item.itemType);
        }
        else
        {
            EquipNewItem(itemIndex);
        }
    }

    private void EquipNewItem(int itemIndex)
    {
        Item item = items[itemIndex];
        GameObject equippedItemPrefab = item.itemPrefab;
        if (item.itemType == ItemType.Boots)
        {
            GameObject leftBoot = Instantiate(equippedItemPrefab, Vector3.zero, Quaternion.identity);
            GameObject rightBoot = Instantiate(ItemFetcher.Instance.RightBoot, Vector3.zero, Quaternion.identity);
            MirrorBoots(leftBoot, item, leftFootTransform, true);
            MirrorBoots(rightBoot, item, rightFootTransform, false);
            currentEquippedItems.Add(leftBoot);
            currentEquippedItems.Add(rightBoot);
            return;
        }
        GameObject newItem = Instantiate(equippedItemPrefab, Vector3.zero, Quaternion.identity);
        ItemComponent itemComponent = newItem.AddComponent<ItemComponent>();
        itemComponent.item = item;
        Transform attachTransform = GetEquipTransform(item);
        newItem.transform.SetParent(attachTransform);
        newItem.transform.localPosition = item.position;
        newItem.transform.localRotation = Quaternion.Euler(item.rotation);
        newItem.transform.localScale = item.scale;
        currentEquippedItems.Add(newItem);

    }


    private void MirrorBoots(GameObject boots, Item item, Transform transform, bool left)
    {
        if (boots != null)
        {
            boots.name = item.itemName;
            boots.transform.SetParent(transform);
            if (left)
            {
                boots.transform.localPosition = item.position;
                boots.transform.localRotation = Quaternion.Euler(item.rotation);
                boots.transform.localScale = item.scale;
            }
            else
            {
                Transform RightBoot = ItemFetcher.Instance.RightBoot.transform;
                boots.transform.localPosition = RightBoot.position;
                boots.transform.localRotation = RightBoot.rotation;
                boots.transform.localScale = RightBoot.localScale;

            }
        }
    }

    private Transform GetEquipTransform(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Armor:
                return bodyTransform;
            case ItemType.Boots:
                return leftFootTransform;
            case ItemType.Helmet:
                return headTransform;
            case ItemType.Necklace:
                return neckTransform;
            case ItemType.Ring:
                return fingerTransform;
            case ItemType.Weapon:
                return rightHandTransform;
            default:
                return bodyTransform;
        }
    }
    public void UnequipItem(ItemType itemType)
    {

        for (int i = currentEquippedItems.Count - 1; i >= 0; i--)
        {
            GameObject equippedItem = currentEquippedItems[i];
            Item item = equippedItem.GetComponent<ItemComponent>().item;

            if (item.itemType == itemType)
            {
                Destroy(equippedItem);
                currentEquippedItems.RemoveAt(i);

                return;
            }


        }
    }
}
