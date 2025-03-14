using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BodyPartPair
{
    public GameObject original;
    public SkinnedMeshRenderer prefab;
}

public class Bones : MonoBehaviour
{
    [SerializeField] private List<BodyPartPair> bodyPartPairs;
    [SerializeField] private Transform rootBone;
    private ItemEquipper itemEquipper;
    private List<Item> itemsInHierarchy = new List<Item>();
    private void Start()
    {
        itemEquipper = GetComponentInParent<ItemEquipper>();
    }
    public List<Item> ItemsInHierarchy()
    {
        return itemsInHierarchy;
    }
    public void SetupBodyPartPair(BodyPartPair pair)
    {
        if (pair != null && rootBone != null)
        {
            bool pairExists = bodyPartPairs.Any(existingPair => existingPair.original == pair.original && existingPair.prefab == pair.prefab);
            if (!pairExists)
            {
                bodyPartPairs.Add(pair);
            }
        }
    }
    public void AttachBodyParts(Item item = null)
    {

        BodyPartPair pair = bodyPartPairs.FirstOrDefault(p => p.prefab.name == item.itemPrefab.name);
        if (pair == null)
        {

            return;
        }
        Item previousItem = itemsInHierarchy.FirstOrDefault(i => i.itemType == item.itemType);
        if (previousItem != null)
        {
            itemEquipper.UnequipByID(previousItem.ID);
            itemsInHierarchy.Remove(previousItem);
        }
        GameObject existingItem = itemEquipper.ItemsInHierarchy
            .FirstOrDefault(obj => obj.name == item.itemName);

        if (existingItem != null)
        {
            existingItem.SetActive(true);
            itemEquipper.CurrentEquippedItems.Add(existingItem);
            itemsInHierarchy.Add(item);

            return;
        }
        SkinnedMeshRenderer thisRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (thisRenderer == null)
        {

            return;
        }
        Transform[] targetBones = thisRenderer.bones;
        SkinnedMeshRenderer newPart = Instantiate(pair.prefab, transform);
        newPart.transform.localPosition = Vector3.zero;
        newPart.transform.localRotation = Quaternion.identity;
        newPart.transform.localScale = Vector3.one;
        newPart.gameObject.name = item.itemName;
        var itemComponent = newPart.gameObject.GetComponent<ItemComponent>() ?? newPart.gameObject.AddComponent<ItemComponent>();
        itemComponent.item = item;
        itemEquipper.CurrentEquippedItems.Add(newPart.gameObject);
        itemEquipper.ItemsInHierarchy.Add(newPart.gameObject);
        itemsInHierarchy.Add(item);
        SkinnedMeshRenderer[] originalRenderers = pair.original.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var originalRenderer in originalRenderers)
        {
            if (originalRenderer.sharedMesh == pair.prefab.sharedMesh)
            {
                Transform[] mappedBones = MapBones(originalRenderer.bones, targetBones);
                newPart.bones = mappedBones;
                newPart.rootBone = rootBone;
                break;
            }
        }
    }
    private Transform[] MapBones(Transform[] sourceBones, Transform[] targetBones)
    {
        Dictionary<string, Transform> boneDictionary = new Dictionary<string, Transform>();

        foreach (Transform bone in targetBones)
        {
            if (bone != null)
                boneDictionary[bone.name] = bone;
        }

        Transform[] mappedBones = new Transform[sourceBones.Length];

        for (int i = 0; i < sourceBones.Length; i++)
        {
            if (sourceBones[i] != null && boneDictionary.TryGetValue(sourceBones[i].name, out Transform matchingBone))
            {
                mappedBones[i] = matchingBone;
            }
            else
            {

                mappedBones[i] = null;
            }
        }

        return mappedBones;
    }
}
