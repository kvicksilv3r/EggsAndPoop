using System;
using System.Collections;
using UnityEngine;

public class EggSlotManager : MonoBehaviour
{
    public static EggSlotManager Instance;

    public InventoryConfig inventoryConfig;

    private float _slotProgress;
    private int _activeSlot;
    private int[] _loveReductionLevels = new int[3];
    private int[] _luckLevels = new int[3];
    private int _cartonTier;
    private float _cartonProgress;

    public int LastSessionEggsEarned { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadSlots()
    {
        var data = DataController.instance.GetData();
        _slotProgress = data.currentSlotProgress;
        _activeSlot = Mathf.Clamp(data.currentSlotIndex, 0, 2);
        _loveReductionLevels = data.slotLoveReductionLevel != null && data.slotLoveReductionLevel.Length == 3
            ? data.slotLoveReductionLevel : new int[3];
        _luckLevels = data.slotLuckLevel != null && data.slotLuckLevel.Length == 3
            ? data.slotLuckLevel : new int[3];
        _cartonTier = data.cartonCurrentTier;
        _cartonProgress = data.cartonLoveProgress;
    }

    public void CalculateAfkOutput(float hoursAway)
    {
        LastSessionEggsEarned = 0;
        ApplyLove(GetLovePerHour() * hoursAway);
        ApplyCartonLove(inventoryConfig.cartonLovePerHour * hoursAway);
    }

    public void StartRealTimeGeneration()
    {
        StartCoroutine(SlotCoroutine());
        StartCoroutine(CartonCoroutine());
    }

    private IEnumerator SlotCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            ApplyLove(GetLovePerHour() / 60f);
        }
    }

    private IEnumerator CartonCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            ApplyCartonLove(inventoryConfig.cartonLovePerHour / 60f);
        }
    }

    private void ApplyLove(float love)
    {
        while (love > 0)
        {
            float cost = CalculateCurrentSlotCost();
            float needed = cost - _slotProgress;

            if (love >= needed && PlayerInventoryManager.Instance.CanAddEgg())
            {
                love -= needed;
                _slotProgress = 0f;
                PlayerInventoryManager.Instance.AddEgg();
                LastSessionEggsEarned++;
                _activeSlot = (_activeSlot + 1) % 3;
            }
            else
            {
                _slotProgress = Mathf.Min(_slotProgress + love, cost);
                break;
            }
        }
    }

    private void ApplyCartonLove(float love)
    {
        _cartonProgress += love;

        while (_cartonProgress >= GetCartonTierCost(_cartonTier))
        {
            _cartonProgress -= GetCartonTierCost(_cartonTier);
            PlayerInventoryManager.Instance.AddEggsOfType(EggType.Farm, 1);
            _cartonTier = Mathf.Min(_cartonTier + 1, inventoryConfig.cartonTiers.Length - 1);
        }
    }

    private float CalculateCurrentSlotCost()
    {
        if (inventoryConfig.eggSlots == null || inventoryConfig.eggSlots.Length <= _activeSlot)
            return 200f;
        var config = inventoryConfig.eggSlots[_activeSlot];
        float reduction = _loveReductionLevels[_activeSlot] * config.loveReductionPerUpgradeLevel;
        return Mathf.Max(config.baseLoveCost - reduction, 10f);
    }

    private float GetCartonTierCost(int tier)
    {
        if (inventoryConfig.cartonTiers == null || inventoryConfig.cartonTiers.Length == 0)
            return 2000f;
        return inventoryConfig.cartonTiers[Mathf.Clamp(tier, 0, inventoryConfig.cartonTiers.Length - 1)].loveCost;
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
        return total;
    }

    public string TimeUntilNextEgg()
    {
        float lovePerSecond = GetLovePerHour() / 3600f;
        if (lovePerSecond <= 0) return "no love...";

        float remaining = CalculateCurrentSlotCost() - _slotProgress;
        if (remaining <= 0) return "any moment now...";

        var span = TimeSpan.FromSeconds(remaining / lovePerSecond);
        string hours = span.Hours > 0 ? span.Hours + "h " : "";
        string minutes = span.Minutes > 0 ? span.Minutes + "m " : "";
        return hours + minutes + span.Seconds + "s";
    }

    public float GetSlotProgress() => _slotProgress;
    public float GetCurrentSlotCost() => CalculateCurrentSlotCost();
    public int GetActiveSlot() => _activeSlot;
    public float GetCartonProgress() => _cartonProgress;
    public float GetCartonCost() => GetCartonTierCost(_cartonTier);
    public int GetCartonTier() => _cartonTier;

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.currentSlotProgress = _slotProgress;
        saveData.currentSlotIndex = _activeSlot;
        saveData.slotLoveReductionLevel = _loveReductionLevels;
        saveData.slotLuckLevel = _luckLevels;
        saveData.cartonCurrentTier = _cartonTier;
        saveData.cartonLoveProgress = _cartonProgress;
    }
}
