using UnityEngine;

[CreateAssetMenu(fileName = "InventoryConfig", menuName = "EAP/New InventoryConfig")]
public class InventoryConfig : ScriptableObject
{
    public int baseMaxOwnedEggs;
    public int baseMaxOwnedAnimals;
    public int baseMaxActiveAnimals;
    public int hoursForNewEgg;
    public int baseFertilizerCapacity;
    public int daysUntilQuirkUnlocks = 3;
    public int breedingPenCapacity;
    public int feedingTroughCapacity;
    public int baseFoodCapacity;
    [Tooltip("Food consumed per animal per hour in the Feeding Trough")]
    public float foodConsumptionRatePerHour = 1f;
    [Tooltip("How many fertilizer units make 1 food unit")]
    public float fertilizerToFoodRatio = 2f;
}
