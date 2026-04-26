using System;
using System.Linq;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance;

    public InventoryConfig inventoryConfig;

    private int _farmLevel;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadFarm()
    {
        _farmLevel = DataController.instance.GetData().farmLevel;
    }

    public void ApplyCurrentLevel()
    {
        // Bonuses are computed on-demand via Get* methods — nothing to apply at startup.
        // Called here to mirror other manager patterns; extend if side-effects are needed.
    }

    public int GetLevel() => _farmLevel;

    public int GetUpgradeCost()
    {
        return Mathf.RoundToInt(inventoryConfig.farmUpgradeBaseCost * Mathf.Pow(_farmLevel + 1, inventoryConfig.farmUpgradeCostExponent));
    }

    public bool TryUpgrade()
    {
        int cost = GetUpgradeCost();
        if (!CoinManager.Instance.SpendCoins(cost)) return false;
        _farmLevel++;
        DataController.instance.CompleteSave();
        return true;
    }

    public int GetBonusAnimalSlots() => SumTiers(t => t.additionalAnimalSlots);
    public int GetBonusEggCapacity() => SumTiers(t => t.additionalEggCapacity);
    public int GetBonusFertilizerCapacity() => SumTiers(t => t.additionalFertilizerCapacity);
    public int GetBonusPoopCapacity() => SumTiers(t => t.additionalPoopCapacity);

    public string GetBonusSummary()
    {
        return $"+{GetBonusAnimalSlots()} animal slots\n" +
               $"+{GetBonusEggCapacity()} egg capacity\n" +
               $"+{GetBonusFertilizerCapacity()} fertilizer capacity\n" +
               $"+{GetBonusPoopCapacity()} poop capacity";
    }

    private int SumTiers(Func<FarmUpgradeTier, int> selector)
    {
        return inventoryConfig.farmUpgradeTiers
            .Take(_farmLevel)
            .Sum(selector);
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.farmLevel = _farmLevel;
    }
}
