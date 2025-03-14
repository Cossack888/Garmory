using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityButton : MonoBehaviour
{
    [SerializeField] Image rarityImage;
    [SerializeField] Image buttonImage;
    [SerializeField] Button button;

    public void SetupButton(Sprite rarity, Sprite item, out Button button)
    {
        rarityImage.sprite = rarity;
        buttonImage.sprite = item;
        button = this.button;
    }
}
