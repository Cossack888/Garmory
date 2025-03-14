using UnityEngine;

public class SpritesLoader : MonoBehaviour
{
    public string spriteFolderPath = "Icons";

    public Sprite[] ArmorSprites { get; private set; }
    public Sprite[] HelmetSprites { get; private set; }
    public Sprite[] WeaponSprites { get; private set; }
    public Sprite[] BootSprites { get; private set; }
    public Sprite[] RingSprites { get; private set; }
    public Sprite[] NecklaceSprites { get; private set; }
    public Sprite[] RaritySprites { get; private set; }

    private void Start()
    {

        ArmorSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Armor");
        HelmetSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Helmets");
        WeaponSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Weapons");
        BootSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Boots");
        RingSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Rings");
        NecklaceSprites = LoadSpritesFromFolder($"{spriteFolderPath}/Necklaces");
        RaritySprites = LoadSpritesFromFolder($"{spriteFolderPath}/Rarity");
    }

    private Sprite[] LoadSpritesFromFolder(string folderPath)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
        if (sprites.Length == 0)
        {
            Debug.Log("No sprites found in the folder" + folderPath);
        }
        return sprites;
    }

    public Sprite GetSpriteByName(Sprite[] spriteArray, string nameSubstring)
    {
        if (spriteArray == null || spriteArray.Length == 0)
        {
            return null;
        }
        foreach (Sprite sprite in spriteArray)
        {
            if (sprite.name.ToLower().Contains(nameSubstring.ToLower()))
            {
                return sprite;
            }
        }
        return null;
    }

}
