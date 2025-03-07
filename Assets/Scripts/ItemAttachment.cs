using UnityEditor;
using UnityEngine;

public class ItemAttachment : MonoBehaviour
{

    [Header("Item Settings")]
    public Item[] items;
    private Transform leftAttachPoint;
    private Transform rightAttachPoint;
    private Transform centralAttachPoint;
    private Transform finalAttachPoint;
    private GameObject currentItem;
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
    void Start()
    {
        if (items.Length > 0)
            EquipItem(0);
    }
    public enum AttachPoint
    {
        LeftAttachPoint,
        RightAttachPoint,
        CentralAttachPoint,
    }
    public void EquipItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= items.Length)
        {
            Debug.LogWarning("Invalid item index");
            return;
        }

        if (currentItem != null)
            Destroy(currentItem);

        Item item = items[itemIndex];
        switch (item.itemType)
        {
            case ItemType.Armor:
                centralAttachPoint = bodyTransform;
                break;
            case ItemType.Weapon:
                rightAttachPoint = rightHandTransform;
                break;
            case ItemType.Boots:
                leftAttachPoint = leftFootTransform;
                rightAttachPoint = rightFootTransform;
                break;
            case ItemType.Helmet:
                centralAttachPoint = headTransform;
                break;
            case ItemType.Necklace:
                centralAttachPoint = neckTransform;
                break;
            case ItemType.Ring:
                rightAttachPoint = fingerTransform;
                break;
            default:
                Debug.LogWarning("Invalid item type " + item.itemType);
                break;
        }
        switch (item.attachPoint)
        {
            case AttachPoint.LeftAttachPoint:
                finalAttachPoint = leftAttachPoint;
                break;
            case AttachPoint.RightAttachPoint:
                finalAttachPoint = rightAttachPoint;
                break;
            case AttachPoint.CentralAttachPoint:
                finalAttachPoint = centralAttachPoint;
                break;
            default:
                Debug.LogWarning("Invalid attach point " + item.attachPoint);
                break;
        }
        if (finalAttachPoint == null)
        {
            Debug.LogError("Final attach point is null. Check item type and attach point.");
            return;
        }
        currentItem = Instantiate(item.itemPrefab, finalAttachPoint.position, Quaternion.identity, finalAttachPoint);
        currentItem.transform.localScale = item.scale;
        currentItem.transform.localPosition = item.position;
        currentItem.transform.localRotation = Quaternion.Euler(item.rotation);
    }

}
