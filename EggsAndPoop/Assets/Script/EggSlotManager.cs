using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EggSlotManager : MonoBehaviour
{
    public static EggSlotManager Instance;

    public InventoryConfig inventoryConfig;

    private float[] _slotProgress = new float[4];
    private int[] _loveReductionLevels = new int[3];
    private int[] _luckLevels = new int[3];
    private int _cartonTier;

    public int LastSessionEggsEarned { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadSlots()
    {
        var data = DataController.instance.GetData();
        _slotProgress = data.slotProgresses != null && data.slotProgresses.Length == 4
            ? data.slotProgresses : new float[4];
        _loveReductionLevels = data.slotLoveReductionLevel != null && data.slotLoveReductionLevel.Length == 3
            ? data.slotLoveReductionLevel : new int[3];
        _luckLevels = data.slotLuckLevel != null && data.slotLuckLevel.Length == 3
            ? data.slotLuckLevel : new int[3];
        _cartonTier = data.cartonCurrentTier;
    }

    public void CalculateAfkOutput(float hoursAway)
    {
        LastSessionEggsEarned = 0;
        ApplyLove(GetLovePerHour() * hoursAway);
    }

    public void StartRealTimeGeneration()
    {
        StartCoroutine(SlotCoroutine());
    }

    private IEnumerator SlotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ApplyLove(GetLovePerHour() / 3600f);
        }
    }

    private void ApplyLove(float love)
    {
        for (int i = 0; i < 4; i++)
        {
            if (EggNestManager.Instance != null && EggNestManager.Instance.IsNestOccupied(i))
                continue;

            _slotProgress[i] += love;
            float cost = GetSlotCostForIndex(i);

            if (_slotProgress[i] >= cost)
            {
                _slotProgress[i] = 0f;
                FireSlot(i);
            }
        }
    }

    private void FireSlot(int i)
    {
        EggType eggType;
        if (i < 3)
        {
            var types = PlayerInventoryManager.Instance.unlockedEggTypes;
            eggType = types[UnityEngine.Random.Range(0, types.Count)];
        }
        else
        {
            eggType = EggType.Farm;
            if (inventoryConfig.cartonTiers != null && inventoryConfig.cartonTiers.Length > 0)
                _cartonTier = Mathf.Min(_cartonTier + 1, inventoryConfig.cartonTiers.Length - 1);
        }

        EggNestManager.Instance?.SpawnEggAtNest(i, eggType);
        LastSessionEggsEarned++;
    }

    public float GetLovePerHour()
    {
        float total = inventoryConfig.baseLovePerHour;

        foreach (var entry in EnclosureManager.Instance.GetAnimalsInEnclosure(EnclosureType.Pasture))
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;
            float multiplier = AnimalAgeHelper.GetOutputMultiplier(entry, data, inventoryConfig);
            total += data.loveGeneration * inventoryConfig.lovePerPastureAnimalPerHour * multiplier;
        }

        var penAnimals = EnclosureManager.Instance.GetAnimalsInEnclosure(EnclosureType.BreedingPen);
        var pairedSpecies = GetPairedSpecies(penAnimals);

        foreach (var entry in penAnimals)
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;
            float multiplier = AnimalAgeHelper.GetOutputMultiplier(entry, data, inventoryConfig);
            float pairBonus = pairedSpecies.Contains(entry.animalIdentifier) ? inventoryConfig.breedingPairLoveMultiplier : 1f;
            total += data.loveGeneration * inventoryConfig.lovePerBreedingAnimalPerHour * multiplier * pairBonus;
        }

        return total;
    }

    private HashSet<AnimalEnum> GetPairedSpecies(List<PlayerAnimalEntry> penAnimals)
    {
        var paired = new HashSet<AnimalEnum>();
        foreach (var species in penAnimals.Select(a => a.animalIdentifier).Distinct())
        {
            var group = penAnimals.Where(a => a.animalIdentifier == species).ToList();
            if (group.Any(a => a.sex == AnimalSex.Male) && group.Any(a => a.sex == AnimalSex.Female))
                paired.Add(species);
        }
        return paired;
    }

    public string TimeUntilNextEgg()
    {
        float lovePerSecond = GetLovePerHour() / 3600f;
        if (lovePerSecond <= 0) return "no love...";

        float minRemaining = float.MaxValue;
        for (int i = 0; i < 4; i++)
        {
            if (EggNestManager.Instance != null && EggNestManager.Instance.IsNestOccupied(i))
                continue;
            float remaining = GetSlotCostForIndex(i) - _slotProgress[i];
            if (remaining < minRemaining) minRemaining = remaining;
        }

        if (minRemaining == float.MaxValue) return "all nests full";
        if (minRemaining <= 0) return "any moment now...";

        return FormatTime(minRemaining / lovePerSecond);
    }

    public string TimeUntilNextEgg(int slotIndex)
    {
        float lovePerSecond = GetLovePerHour() / 3600f;
        if (lovePerSecond <= 0) return "no love...";

        float remaining = GetSlotCostForIndex(slotIndex) - _slotProgress[slotIndex];
        if (remaining <= 0) return "any moment now...";

        return FormatTime(remaining / lovePerSecond);
    }

    private static string FormatTime(float seconds)
    {
        var span = TimeSpan.FromSeconds(seconds);
        string hours = span.Hours > 0 ? span.Hours + "h " : "";
        string minutes = span.Minutes > 0 ? span.Minutes + "m " : "";
        return hours + minutes + span.Seconds + "s";
    }

    public float GetSlotProgress(int slot) => slot >= 0 && slot < 4 ? _slotProgress[slot] : 0f;
    public int GetCartonTier() => _cartonTier;

    public float GetSlotCostForIndex(int slot)
    {
        if (slot < 3)
        {
            if (inventoryConfig.eggSlots == null || inventoryConfig.eggSlots.Length <= slot)
                return 200f;
            var config = inventoryConfig.eggSlots[slot];
            float reduction = _loveReductionLevels[slot] * config.loveReductionPerUpgradeLevel;
            return Mathf.Max(config.baseLoveCost - reduction, 10f);
        }
        if (inventoryConfig.cartonTiers == null || inventoryConfig.cartonTiers.Length == 0)
            return 2000f;
        return inventoryConfig.cartonTiers[Mathf.Clamp(_cartonTier, 0, inventoryConfig.cartonTiers.Length - 1)].loveCost;
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.slotProgresses = _slotProgress;
        saveData.slotLoveReductionLevel = _loveReductionLevels;
        saveData.slotLuckLevel = _luckLevels;
        saveData.cartonCurrentTier = _cartonTier;
    }
}
