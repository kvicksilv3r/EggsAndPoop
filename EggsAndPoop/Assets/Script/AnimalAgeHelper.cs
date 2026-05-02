using System;
using UnityEngine;

public static class AnimalAgeHelper
{
    public static AnimalAge GetAgeStage(PlayerAnimalEntry entry, AnimalData data, InventoryConfig config)
    {
        float ageDays = (float)(DateTime.Now - new DateTime(entry.timeOfBirth)).TotalDays;
        var t = config.GetAgeThresholds(data.rarity);
        if (t == null) return AnimalAge.Adult;

        if (ageDays < t.babyUntilDays)   return AnimalAge.Baby;
        if (ageDays < t.youngUntilDays)  return AnimalAge.Young;
        if (ageDays < t.adultUntilDays)  return AnimalAge.Adult;
        if (ageDays < t.seniorUntilDays) return AnimalAge.Senior;
        return AnimalAge.Old;
    }

    public static float GetAgeMultiplier(AnimalAge stage, InventoryConfig config)
    {
        return stage switch
        {
            AnimalAge.Baby   => config.babyOutputMultiplier,
            AnimalAge.Young  => config.youngOutputMultiplier,
            AnimalAge.Adult  => config.adultOutputMultiplier,
            AnimalAge.Senior => config.seniorOutputMultiplier,
            AnimalAge.Old    => config.oldOutputMultiplier,
            _                => 1f
        };
    }

    public static float GetHappinessMultiplier(PlayerAnimalEntry entry, InventoryConfig config)
    {
        return Mathf.Lerp(config.happinessMinMultiplier, config.happinessMaxMultiplier,
                          entry.happinessAmount / config.maxHappiness);
    }

    public static float GetOutputMultiplier(PlayerAnimalEntry entry, AnimalData data, InventoryConfig config)
    {
        var stage = GetAgeStage(entry, data, config);
        return GetAgeMultiplier(stage, config) * GetHappinessMultiplier(entry, config);
    }

    public static bool CanBreed(PlayerAnimalEntry entry, AnimalData data, InventoryConfig config)
    {
        return GetAgeStage(entry, data, config) >= config.minimumBreedingAge;
    }
}
