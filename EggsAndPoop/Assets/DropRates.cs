using UnityEngine;

[CreateAssetMenu(fileName = "DropRates", menuName = "EAP/DropRates")]
public class DropRates : ScriptableObject
{
    public DropRatePair[] dropRates;
}

[System.Serializable]
public class DropRatePair
{
    public AnimalRarity rarity;
    [Tooltip("Percentage chance of this item dropping"), Range(0, 100)]
    public int dropRate;
    [Range(0, 100)]
    public int maxRollIfHighestRarity;
}
