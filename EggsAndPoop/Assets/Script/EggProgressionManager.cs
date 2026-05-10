using System.Linq;
using UnityEngine;

public class EggProgressionManager : MonoBehaviour
{
    public static EggProgressionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public EggData[] GetOrderedEggs()
    {
        return EggRoster.Instance.GetData()
            .Where(e => e.eggType != EggType.Unknown)
            .OrderBy(e => e.tier)
            .ToArray();
    }

    public EggData GetNextLocked()
    {
        return GetOrderedEggs().FirstOrDefault(e => !IsUnlocked(e.eggType));
    }

    public bool IsUnlocked(EggType type)
    {
        return PlayerInventoryManager.Instance.unlockedEggTypes.Contains(type);
    }

    public bool AreConditionsMet(EggData egg)
    {
        if (egg.unlockConditions == null) return true;

        foreach (var condition in egg.unlockConditions)
        {
            if (!IsConditionMet(condition))
                return false;
        }
        return true;
    }

    private bool IsConditionMet(EggUnlockCondition condition)
    {
        switch (condition.type)
        {
            case EggUnlockConditionType.TotalAnimals:
                return PlayerInventoryManager.Instance.playerAnimals.Count >= condition.requiredAmount;

            case EggUnlockConditionType.FarmLevel:
                return FarmManager.Instance.GetLevel() >= condition.requiredAmount;

            case EggUnlockConditionType.AnimalsOfFamily:
                int count = PlayerInventoryManager.Instance.playerAnimals.Count(a =>
                {
                    var data = AnimalRoster.Instance.GetByIdentifier(a.animalIdentifier);
                    return data != null && data.family == condition.requiredFamily;
                });
                return count >= condition.requiredAmount;

            case EggUnlockConditionType.EggTypeUnlocked:
                return IsUnlocked(condition.requiredEggType);

            default:
                return true;
        }
    }

    public bool CanPurchase(EggData egg)
    {
        return !IsUnlocked(egg.eggType)
            && AreConditionsMet(egg)
            && CoinManager.Instance.GetCoins() >= egg.unlockCost;
    }

    public bool TryPurchase(EggData egg)
    {
        if (!CanPurchase(egg)) return false;
        if (!CoinManager.Instance.SpendCoins(egg.unlockCost)) return false;

        PlayerInventoryManager.Instance.AddUnlockedEggType(egg.eggType);
        DataController.instance.CompleteSave();
        return true;
    }

    // Human-readable description of a single condition for the UI.
    public string DescribeCondition(EggUnlockCondition condition, out bool met)
    {
        switch (condition.type)
        {
            case EggUnlockConditionType.TotalAnimals:
            {
                int have = PlayerInventoryManager.Instance.playerAnimals.Count;
                met = have >= condition.requiredAmount;
                return $"Own {condition.requiredAmount} animals (you have {have})";
            }
            case EggUnlockConditionType.FarmLevel:
            {
                int level = FarmManager.Instance.GetLevel();
                met = level >= condition.requiredAmount;
                return $"Farm level {condition.requiredAmount} (you are level {level})";
            }
            case EggUnlockConditionType.AnimalsOfFamily:
            {
                int have = PlayerInventoryManager.Instance.playerAnimals.Count(a =>
                {
                    var data = AnimalRoster.Instance.GetByIdentifier(a.animalIdentifier);
                    return data != null && data.family == condition.requiredFamily;
                });
                met = have >= condition.requiredAmount;
                return $"Own {condition.requiredAmount} {condition.requiredFamily} animals (you have {have})";
            }
            case EggUnlockConditionType.EggTypeUnlocked:
            {
                var eggData = EggRoster.Instance.GetByType(condition.requiredEggType);
                string name = eggData != null ? eggData.eggName : condition.requiredEggType.ToString();
                met = IsUnlocked(condition.requiredEggType);
                return $"{name} unlocked";
            }
            default:
                met = true;
                return "";
        }
    }
}
