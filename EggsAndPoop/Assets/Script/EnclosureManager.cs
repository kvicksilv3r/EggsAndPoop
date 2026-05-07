using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnclosureManager : MonoBehaviour
{
    public static EnclosureManager Instance;

    public InventoryConfig inventoryConfig;

    private void Awake()
    {
        Instance = this;
    }

    // ── Startup ──────────────────────────────────────────────────────────────

    public void Initialize()
    {
        float hoursAway = FoodManager.Instance.CachedHoursAway;
        float foodBeforeAfk = FoodManager.Instance.GetFoodAmount();

        CalculateAfkFoodOutput(hoursAway);
        CalculateAfkHappiness(hoursAway, foodBeforeAfk);
        EggSlotManager.Instance.CalculateAfkOutput(hoursAway);

        DataController.instance.CompleteSave();
        GameManager.Instance.m_AfkDataProcessed.Invoke();

        StartCoroutine(FoodCoroutine());
        StartCoroutine(HappinessCoroutine());
        StartCoroutine(OldAgeCheckCoroutine());
    }

    // ── AFK calculations ─────────────────────────────────────────────────────

    private void CalculateAfkFoodOutput(float hoursAway)
    {
        int farmingAnimals = GetAnimalsInEnclosure(EnclosureType.FarmingPlot).Count;
        FoodManager.Instance.AddFood(
            Mathf.FloorToInt(farmingAnimals * inventoryConfig.foodGenerationRatePerAnimalPerHour * hoursAway));

        int activeCount = PlayerInventoryManager.Instance.GetActiveAnimals().Count;
        FoodManager.Instance.ConsumeFood(
            Mathf.FloorToInt(activeCount * inventoryConfig.foodConsumptionRatePerHour * hoursAway));
    }

    private void CalculateAfkHappiness(float hoursAway, float foodAtSessionStart)
    {
        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.Pasture))
            entry.happinessAmount = Mathf.Min(
                entry.happinessAmount + inventoryConfig.happinessGrowthPerHour * hoursAway,
                inventoryConfig.maxHappiness);

        var activeAnimals = PlayerInventoryManager.Instance.GetActiveAnimals();
        if (activeAnimals.Count == 0) return;

        int farmingAnimals = GetAnimalsInEnclosure(EnclosureType.FarmingPlot).Count;
        float netFoodPerHour = farmingAnimals * inventoryConfig.foodGenerationRatePerAnimalPerHour
                             - activeAnimals.Count * inventoryConfig.foodConsumptionRatePerHour;

        float hungryHours = netFoodPerHour >= 0f ? 0f
            : Mathf.Max(0f, hoursAway - foodAtSessionStart / -netFoodPerHour);

        if (hungryHours > 0f)
        {
            float decay = inventoryConfig.happinessHungerDecayPerHour * hungryHours;
            foreach (var entry in activeAnimals)
                entry.happinessAmount = Mathf.Max(entry.happinessAmount - decay, 0f);
        }
    }

    // ── Real-time coroutines ──────────────────────────────────────────────────

    private IEnumerator FoodCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3600f);

            int farmingAnimals = GetAnimalsInEnclosure(EnclosureType.FarmingPlot).Count;
            FoodManager.Instance.AddFood(
                Mathf.FloorToInt(farmingAnimals * inventoryConfig.foodGenerationRatePerAnimalPerHour));

            int activeCount = PlayerInventoryManager.Instance.GetActiveAnimals().Count;
            FoodManager.Instance.ConsumeFood(
                Mathf.CeilToInt(activeCount * inventoryConfig.foodConsumptionRatePerHour));
        }
    }

    private IEnumerator HappinessCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);

            float gain = inventoryConfig.happinessGrowthPerHour / 60f;
            foreach (var entry in GetAnimalsInEnclosure(EnclosureType.Pasture))
                entry.happinessAmount = Mathf.Min(entry.happinessAmount + gain, inventoryConfig.maxHappiness);

            if (!FoodManager.Instance.HasFood())
            {
                float decay = inventoryConfig.happinessHungerDecayPerHour / 60f;
                foreach (var entry in PlayerInventoryManager.Instance.GetActiveAnimals())
                    entry.happinessAmount = Mathf.Max(entry.happinessAmount - decay, 0f);
            }
        }
    }

    private IEnumerator OldAgeCheckCoroutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            CheckOldAgeNotifications();
            yield return new WaitForSeconds(600f);
        }
    }

    private void CheckOldAgeNotifications()
    {
        bool saved = false;

        foreach (var entry in PlayerInventoryManager.Instance.GetAnimals())
        {
            if (entry.oldAgeNotified) continue;
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;
            if (AnimalAgeHelper.GetAgeStage(entry, data, inventoryConfig) != AnimalAge.Old) continue;

            entry.oldAgeNotified = true;
            FloatingMessageController.Instance.Show(
                $"{entry.customAnimalName} is getting on in years. Still loved, just moving a little slower.");
            saved = true;
        }

        if (saved) DataController.instance.CompleteSave();
    }

    // ── Assignment ────────────────────────────────────────────────────────────

    public bool AssignToEnclosure(string guid, EnclosureType targetType)
    {
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        if (entry == null) return false;
        if (entry.enclosureType == targetType) return true;

        EnclosureType previousType = entry.enclosureType;
        bool wasActive = previousType != EnclosureType.Storage;
        bool willBeActive = targetType != EnclosureType.Storage;

        // Skip capacity check when moving between active enclosures — the active count doesn't change.
        if (willBeActive && !wasActive && !CanAssignToEnclosure(targetType))
        {
            Debug.LogWarning($"[Enclosure] {entry.customAnimalName} → {targetType} REJECTED (full)");
            return false;
        }

        entry.enclosureType = targetType;

        if (wasActive && !willBeActive)
            PhysicalAnimalController.Instance.RemoveAnimal(guid);
        else if (!wasActive && willBeActive)
            PhysicalAnimalController.Instance.InstantiateAnimals();

        DataController.instance.CompleteSave();
        return true;
    }

    public bool CanAssignToEnclosure(EnclosureType type)
    {
        return type switch
        {
            EnclosureType.Pasture     => PlayerInventoryManager.Instance.HasOpenActiveSlots(),
            EnclosureType.BreedingPen => GetAnimalsInEnclosure(EnclosureType.BreedingPen).Count < inventoryConfig.breedingPenCapacity,
            EnclosureType.FarmingPlot => GetAnimalsInEnclosure(EnclosureType.FarmingPlot).Count < inventoryConfig.farmingPlotCapacity,
            _                         => true
        };
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    public List<PlayerAnimalEntry> GetAnimalsInEnclosure(EnclosureType type)
    {
        return PlayerInventoryManager.Instance.GetAnimals()
            .Where(a => a.enclosureType == type).ToList();
    }
}
