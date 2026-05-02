using System.Collections.Generic;
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

    [Header("Poop Economy")]
    public int basePoopCapacity = 100;
    [Tooltip("Coins earned per poop unit sold")]
    public float poopToCoinsRate = 1f;
    [Tooltip("How many poop units make 1 fertilizer unit")]
    public float poopToFertilizerRatio = 1f;
    [Tooltip("Rarity roll bonus added per poop unit when mulching the breeding pen")]
    public int poopMulchBonusPerUnit = 5;
    [Tooltip("Maximum accumulated breeding rarity bonus from mulching")]
    public int maxBreedingRarityBonus = 200;

    [Header("Breeding Pen")]
    [Tooltip("Hours between bred-animal productions per same-species pair")]
    public float breedingPairCooldownHours = 72f;

    [Header("Farm Upgrades")]
    public int farmUpgradeBaseCost = 100;
    public float farmUpgradeCostExponent = 1.5f;
    public List<FarmUpgradeTier> farmUpgradeTiers = new List<FarmUpgradeTier>();

    public RarityAgeThresholds GetAgeThresholds(AnimalRarity rarity)
    {
        return ageThresholds.Find(t => t.rarity == rarity);
    }

    [Header("Animal Age")]
    public List<RarityAgeThresholds> ageThresholds = new List<RarityAgeThresholds>();
    public float babyOutputMultiplier   = 0.5f;
    public float youngOutputMultiplier  = 0.8f;
    public float adultOutputMultiplier  = 1.0f;
    public float seniorOutputMultiplier = 0.7f;
    public float oldOutputMultiplier    = 0.1f;
    public AnimalAge minimumBreedingAge = AnimalAge.Adult;

    [Header("Happiness")]
    public float maxHappiness                  = 100f;
    public float happinessGrowthPerHour        = 5f;
    public float happinessHungerDecayPerHour   = 10f;
    public float happinessMinMultiplier        = 1.0f;
    public float happinessMaxMultiplier        = 2.0f;

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
