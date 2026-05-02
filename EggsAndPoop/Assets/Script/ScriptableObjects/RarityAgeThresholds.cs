using UnityEngine;

[System.Serializable]
public class RarityAgeThresholds
{
    public AnimalRarity rarity;
    [Tooltip("Days old when Baby stage ends")]
    public float babyUntilDays = 1f;
    [Tooltip("Days old when Young stage ends")]
    public float youngUntilDays = 4f;
    [Tooltip("Days old when Adult stage ends")]
    public float adultUntilDays = 14f;
    [Tooltip("Days old when Senior stage ends — Old begins after this")]
    public float seniorUntilDays = 28f;
}
