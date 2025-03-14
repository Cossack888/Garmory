using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Statistics;

public class EquipmentPanelManager : MonoBehaviour
{
    private ItemEquipper itemEquipper;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform[] itemTypePanels;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite eraseIcon;
    private ItemFetcher itemFetcher;
    public Action OnButtonClicked;
    [SerializeField] TMP_Text itemName;
    private SliderControll sliders;
    [SerializeField] private CameraPoint[] cameraPoints;
    private Dictionary<Transform, CameraPoint> panelToCameraDict;
    private CameraTransition cameraTransition;
    private void OnEnable()
    {
        cameraTransition = FindFirstObjectByType<CameraTransition>();
        itemFetcher = FindFirstObjectByType<ItemFetcher>();
        sliders = FindFirstObjectByType<SliderControll>();
        itemEquipper = FindFirstObjectByType<ItemEquipper>();
        if (itemFetcher != null)
            itemFetcher.OnItemsFetched += PopulateEquipmentPanels;

    }

    private void OnDisable()
    {
        if (itemFetcher != null)
            itemFetcher.OnItemsFetched -= PopulateEquipmentPanels;
    }
    private void AddPanelHoverEvents()
    {
        foreach (var panel in itemTypePanels)
        {
            AddHoverEvent(panel.gameObject, (eventData) => OnPanelHover(panel), (eventData) => OnPanelExit());
        }
    }
    private void OnPanelHover(Transform panel)
    {

        foreach (var cameraPair in cameraTransition.GetCamPointsDict())
        {
            if (panel.name == cameraPair.Key.ToString())
            {

                cameraTransition.ChangeCamera(cameraPair.Key);
                return;
            }
        }
    }

    private void OnPanelExit()
    {
        cameraTransition.ChangeCamera(CameraPoint.Overview);
    }

    public void PopulateEquipmentPanels(List<Item> availableItems)
    {

        foreach (Transform panel in itemTypePanels)
        {

            foreach (Transform child in panel)
            {
                Destroy(child.gameObject);
            }
        }


        var groupedItems = availableItems
            .GroupBy(item => item.itemType)
            .OrderBy(group => group.Key)
            .ToList();


        foreach (var group in groupedItems)
        {

            Transform panel = itemTypePanels.FirstOrDefault(p => p.name == group.Key.ToString());

            if (panel == null)
            {

                continue;
            }

            GameObject unequipButtonObj = Instantiate(itemButtonPrefab);
            unequipButtonObj.GetComponent<RarityButton>().SetupButton(null, eraseIcon, out Button unequipButton);
            unequipButtonObj.transform.SetParent(panel);
            unequipButton.onClick.AddListener(() => UnequipItemType(group.Key));
            foreach (var item in group)
            {
                GameObject itemButton = Instantiate(itemButtonPrefab);
                Image iconImage = itemButton.GetComponentInChildren<Image>();

                itemButton.GetComponent<RarityButton>().SetupButton(item.itemRarityIcon, item.itemIcon, out Button button);
                itemButton.transform.SetParent(panel);
                button.onClick.AddListener(() => OnItemClicked(item));
                AddHoverEffect(itemButton, item);
            }
        }
        AddPanelHoverEvents();
    }
    private void AddHoverEffect(GameObject buttonObject, Item item)
    {
        AddHoverEvent(buttonObject, (eventData) => OnButtonHover(item), (eventData) => OnButtonExit());
    }
    private void AddHoverEvent(GameObject target, Action<BaseEventData> onEnter, Action<BaseEventData> onExit)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        // Create PointerEnter event
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) => onEnter(eventData));
        trigger.triggers.Add(entryEnter);

        // Create PointerExit event
        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) => onExit(eventData));
        trigger.triggers.Add(entryExit);
    }


    private void OnButtonHover(Item item)
    {
        itemName.text = item.rarity.ToString() + " " + NormalisingUtils.AddSpacesBeforeCapitals(item.itemName);
        Item equippedItem = itemEquipper.GetEquippedItemByType(item.itemType);

        if (equippedItem != null)
        {
            CompareItemStats(equippedItem, item);
        }
    }
    private void OnButtonExit()
    {
        sliders.ResetSliderColorAndValues();
        itemName.text = null;
    }


    private void OnItemClicked(Item item)
    {


        sliders.ResetSliderColorAndValues();
        itemEquipper.EquipItemByID(item.ID);
        OnButtonClicked?.Invoke();
    }
    private void UnequipItemType(ItemType itemType)
    {

        itemEquipper.UnequipItem(itemType);
        OnButtonClicked?.Invoke();
    }


    private void CompareItemStats(Item equippedItem, Item hoveredItem)
    {
        foreach (var stat in hoveredItem.stats)
        {
            StatNames statName = (StatNames)Enum.Parse(typeof(StatNames), stat.Key.ToString());
            sliders.UpdateSliderColorAndValues(statName, hoveredItem);
        }
    }


}
