using UnityEngine;

[CreateAssetMenu(fileName = "RarityColorMap", menuName = "EAP/RarityColorMap")]
public class RarityColorMap : ScriptableObject
{
    public RarityColorPair[] colorMap;
}

[System.Serializable]
public class RarityColorPair
{
    public Color rarityColor;
    public AnimalRarity rarity;
}
