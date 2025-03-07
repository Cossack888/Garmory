using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemFetcher : MonoBehaviour
{
    public static ItemFetcher Instance { get; private set; }
    private GameServerMock gameServerMock;
    public ItemAttachment itemAttachment;

    [SerializeField] GameObject armorPrefab;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject leftBootPrefab;
    [SerializeField] GameObject rightBootPrefab;
    [SerializeField] GameObject helmetPrefab;
    [SerializeField] GameObject ringPrefab;
    [SerializeField] GameObject necklacePrefab;

    public List<Item> generatedItems { get; private set; } = new List<Item>();


    public event System.Action OnItemsFetched;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private async void Start()
    {
        gameServerMock = new GameServerMock();

        await FetchItems();
    }


    public async Task FetchItems()
    {
        string jsonData = await gameServerMock.GetItemsAsync();
        generatedItems = ProcessItems(jsonData);
        itemAttachment.LoadItemsAtRuntime(generatedItems);
        OnItemsFetched?.Invoke();
    }

    private List<Item> ProcessItems(string jsonData)
    {
        List<Item> itemList = new List<Item>();
        JObject jsonObject = JObject.Parse(jsonData);
        JArray itemsArray = (JArray)jsonObject["Items"];

        foreach (JObject itemData in itemsArray)
        {
            Item newItem = CreateItem(itemData);
            itemList.Add(newItem);
        }

        return itemList;
    }
    public GameObject RightBoot => rightBootPrefab;

    private Item CreateItem(JObject itemData)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.itemName = itemData["Name"].ToString();
        newItem.itemType = GetItemType(itemData["Category"].ToString());
        Transform temporaryObject = null;

        switch (newItem.itemType)
        {
            case ItemType.Armor:
                temporaryObject = armorPrefab.transform;
                break;
            case ItemType.Boots:
                temporaryObject = leftBootPrefab.transform;
                break;
            case ItemType.Weapon:
                temporaryObject = weaponPrefab.transform;
                break;
            case ItemType.Helmet:
                temporaryObject = helmetPrefab.transform;
                break;
            case ItemType.Ring:
                temporaryObject = ringPrefab.transform;
                break;
            case ItemType.Necklace:
                temporaryObject = necklacePrefab.transform;
                break;
        }

        newItem.scale = temporaryObject.localScale;
        newItem.position = temporaryObject.position;
        newItem.rotation = temporaryObject.rotation.eulerAngles;
        newItem.itemIcon = null;
        newItem.itemPrefab = temporaryObject.gameObject;

        return newItem;
    }

    private ItemType GetItemType(string category)
    {
        return category switch
        {
            "Armor" => ItemType.Armor,
            "Boots" => ItemType.Boots,
            "Helmet" => ItemType.Helmet,
            "Necklace" => ItemType.Necklace,
            "Ring" => ItemType.Ring,
            "Weapon" => ItemType.Weapon,
            _ => throw new System.Exception("Unknown item type: " + category)
        };
    }
}
