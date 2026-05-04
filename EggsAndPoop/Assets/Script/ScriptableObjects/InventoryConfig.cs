using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InventoryConfig", menuName = "EAP/New InventoryConfig")]
public class InventoryConfig : ScriptableObject
{
    public int baseMaxOwnedEggs;
    public int baseMaxOwnedAnimals;
    public int baseMaxActiveAnimals;
    public int breedingPenCapacity;
    [FormerlySerializedAs("feedingTroughCapacity")]
    public int farmingPlotCapacity;
    public int baseFoodCapacity;

    [Header("Food")]
    [Tooltip("Food generated per hour by each animal in the Farming Plot")]
    public float foodGenerationRatePerAnimalPerHour = 5f;
    [Tooltip("Food consumed per active animal per hour")]
    public float foodConsumptionRatePerHour = 1f;

    [Header("Breeding Pen")]
    public int daysUntilQuirkUnlocks = 3;

    [Header("Farm Upgrades")]
    public int farmUpgradeBaseCost = 100;
    public float farmUpgradeCostExponent = 1.5f;
    public List<FarmUpgradeTier> farmUpgradeTiers = new List<FarmUpgradeTier>();

    [Header("Animal Age")]
    public List<RarityAgeThresholds> ageThresholds = new List<RarityAgeThresholds>();
    public float babyOutputMultiplier   = 0.5f;
    public float youngOutputMultiplier  = 0.8f;
    public float adultOutputMultiplier  = 1.0f;
    public float seniorOutputMultiplier = 0.7f;
    public float oldOutputMultiplier    = 0.1f;
    public AnimalAge minimumBreedingAge = AnimalAge.Adult;

    public RarityAgeThresholds GetAgeThresholds(AnimalRarity rarity)
    {
        return ageThresholds.Find(t => t.rarity == rarity);
    }

    [Header("Happiness")]
    public float maxHappiness                 = 100f;
    public float happinessGrowthPerHour       = 5f;
    public float happinessHungerDecayPerHour  = 10f;
    public float happinessMinMultiplier       = 0.5f;
    public float happinessMaxMultiplier       = 2.0f;

    [Header("Love & Egg Slots")]
    [Tooltip("Love generated per hour with no animals")]
    public float baseLovePerHour = 20f;
    [Tooltip("Additional love per hour for each animal in Pasture")]
    public float lovePerPastureAnimalPerHour = 10f;
    [Tooltip("Additional love per hour for each animal in the Breeding Pen")]
    public float lovePerBreedingAnimalPerHour = 15f;
    [Tooltip("Multiplier applied to each animal in a matched breeding pair (same species, male + female)")]
    public float breedingPairLoveMultiplier = 2f;
    public EggSlotConfig[] eggSlots = new EggSlotConfig[3];
    public CartonTierConfig[] cartonTiers = new CartonTierConfig[5];
}
