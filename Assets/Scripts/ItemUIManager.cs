using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUIManager : MonoBehaviour
{
    public TMP_Dropdown armorDropdown;
    public TMP_Dropdown bootsDropdown;
    public TMP_Dropdown weaponDropdown;
    public TMP_Dropdown helmetDropdown;
    public TMP_Dropdown necklaceDropdown;
    public TMP_Dropdown ringDropdown;
    public ItemFetcher itemFetcher;

    private void Start()
    {
        itemFetcher.OnItemsFetched += PopulateDropdowns;
    }

    private void OnDestroy()
    {
        itemFetcher.OnItemsFetched -= PopulateDropdowns;
    }
    private void PopulateDropdowns()
    {
        List<string> armorItems = new List<string>();
        List<string> bootsItems = new List<string>();
        List<string> weaponItems = new List<string>();
        List<string> helmetItems = new List<string>();
        List<string> necklaceItems = new List<string>();
        List<string> ringItems = new List<string>();
        foreach (Item item in itemFetcher.generatedItems)
        {
            switch (item.itemType)
            {
                case ItemType.Armor:
                    armorItems.Add(item.itemName);
                    break;
                case ItemType.Boots:
                    bootsItems.Add(item.itemName);
                    break;
                case ItemType.Weapon:
                    weaponItems.Add(item.itemName);
                    break;
                case ItemType.Helmet:
                    helmetItems.Add(item.itemName);
                    break;
                case ItemType.Necklace:
                    necklaceItems.Add(item.itemName);
                    break;
                case ItemType.Ring:
                    ringItems.Add(item.itemName);
                    break;
            }
        }

        
        PopulateDropdown(armorDropdown, armorItems);
        PopulateDropdown(bootsDropdown, bootsItems);
        PopulateDropdown(weaponDropdown, weaponItems);
        PopulateDropdown(helmetDropdown, helmetItems);
        PopulateDropdown(necklaceDropdown, necklaceItems);
        PopulateDropdown(ringDropdown, ringItems);
    }

    
    private void PopulateDropdown(TMP_Dropdown dropdown, List<string> items)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(items);
    }
}
