using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ItemFetcher : MonoBehaviour
{
    public static ItemFetcher Instance { get; private set; }
    private GameServerMock gameServerMock;
    [SerializeField] private GameObject armorPrefab;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject bootPrefab;
    [SerializeField] private GameObject helmetPrefab;
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private GameObject necklacePrefab;
    [SerializeField] private SpritesLoader loader;
    [SerializeField] private Slider loadingSlider;
    public List<Item> generatedItems { get; private set; } = new List<Item>();
    public event System.Action<List<Item>> OnItemsFetched;

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
        var fetchTask = gameServerMock.GetItemsAsync();
        StartCoroutine(UpdateLoadingBar(fetchTask));
        string jsonData = await fetchTask;
        generatedItems = ProcessItems(jsonData);

    }

    private IEnumerator UpdateLoadingBar(Task fetchTask)
    {
        float progress = 0f;

        while (!fetchTask.IsCompleted)
        {
            progress += Time.deltaTime * 0.1f;
            loadingSlider.value = Mathf.Clamp01(progress);
            yield return null;
        }


        while (loadingSlider.value < 1f)
        {
            progress += Time.deltaTime * 3f;
            loadingSlider.value = Mathf.Clamp01(progress);
            yield return null;
        }
        GameManager.Instance.InitialiseStartButton();
        loadingSlider.value = 1f;

    }
    public void PopulateDropdowns()
    {
        StartCoroutine(PopulateDropdownsCoroutine());
    }
    IEnumerator PopulateDropdownsCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        OnItemsFetched?.Invoke(generatedItems);
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
    private static List<int> usedIDs = new List<int>();

    private int GenerateUniqueID()
    {
        int newID;
        do
        {
            newID = UnityEngine.Random.Range(1, 1000);
        }
        while (usedIDs.Contains(newID));

        usedIDs.Add(newID);
        return newID;
    }
    private Item CreateItem(JObject itemData)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.itemName = itemData["Name"].ToString();
        newItem.itemType = GetItemType(itemData["Category"].ToString());
        Transform temporaryObject = null;
        Sprite[] spriteArray = null;
        switch (newItem.itemType)
        {
            case ItemType.Armor:
                temporaryObject = armorPrefab.transform;
                spriteArray = loader.ArmorSprites;
                break;
            case ItemType.Boots:
                temporaryObject = bootPrefab.transform;
                spriteArray = loader.BootSprites;
                break;
            case ItemType.Weapon:
                temporaryObject = weaponPrefab.transform;
                spriteArray = loader.WeaponSprites;
                break;
            case ItemType.Helmet:
                temporaryObject = helmetPrefab.transform;
                spriteArray = loader.HelmetSprites;
                break;
            case ItemType.Ring:
                temporaryObject = ringPrefab.transform;
                spriteArray = loader.RingSprites;
                break;
            case ItemType.Necklace:
                temporaryObject = necklacePrefab.transform;
                spriteArray = loader.NecklaceSprites;
                break;
        }
        int rarity = itemData["Rarity"].Value<int>();
        Sprite[] raritySprites = loader.RaritySprites;
        switch (rarity)
        {
            case 1:
                newItem.rarity = Rarity.Common; break;

            case 2:
                newItem.rarity = Rarity.Uncommon; break;
            case 3:
                newItem.rarity = Rarity.Rare; break;
            case 4:
                newItem.rarity = Rarity.Epic; break;
            case 5:
                newItem.rarity = Rarity.Legendary; break;
        }
        newItem.itemRarityIcon = loader.GetSpriteByName(raritySprites, newItem.rarity.ToString());
        newItem.scale = temporaryObject.localScale;
        newItem.position = temporaryObject.position;
        newItem.rotation = temporaryObject.rotation.eulerAngles;
        newItem.itemIcon = loader.GetSpriteByName(spriteArray, newItem.itemName);
        newItem.itemPrefab = temporaryObject.gameObject;
        newItem.stats[StatisticMod.Damage] = itemData["Damage"].Value<int>();
        newItem.stats[StatisticMod.HealthPoints] = itemData["HealthPoints"].Value<int>();
        newItem.stats[StatisticMod.Defense] = itemData["Defense"].Value<int>();
        newItem.stats[StatisticMod.LifeSteal] = itemData["LifeSteal"].Value<float>();
        newItem.stats[StatisticMod.CriticalStrikeChance] = itemData["CriticalStrikeChance"].Value<float>();
        newItem.stats[StatisticMod.AttackSpeed] = itemData["AttackSpeed"].Value<float>();
        newItem.stats[StatisticMod.MovementSpeed] = itemData["MovementSpeed"].Value<float>();
        newItem.stats[StatisticMod.Luck] = itemData["Luck"].Value<float>();
        newItem.ID = GenerateUniqueID();
        newItem.itemName = (newItem.itemName + " Of " + newItem.GetGreatestStatName()).ToString();
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
