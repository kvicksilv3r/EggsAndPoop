using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnclosureManager : MonoBehaviour
{
    public static EnclosureManager Instance;

    public InventoryConfig inventoryConfig;

    public int LastSessionEggsEarned { get; private set; }

    private float _breedingRarityBonus;
    private readonly Dictionary<AnimalEnum, DateTime> _lastBreedTimes = new Dictionary<AnimalEnum, DateTime>();
    private readonly HashSet<AnimalEnum> _activePairCoroutines = new HashSet<AnimalEnum>();

    private void Awake()
    {
        Instance = this;
    }

    // ── Startup ──────────────────────────────────────────────────────────────

    public void Initialize()
    {
        float hoursAway = FertilizerManager.Instance.CachedHoursAway;

        float foodBeforeAfk = FertilizerManager.Instance.GetFoodAmount();
        CalculateAfkBreedingOutput(hoursAway);
        CalculateAfkFeedingOutput(hoursAway);
        CalculateAfkHappiness(hoursAway, foodBeforeAfk);
        EggSlotManager.Instance.CalculateAfkOutput(hoursAway);

        DataController.instance.CompleteSave();
        GameManager.Instance.m_AfkDataProcessed.Invoke();

        StartBreedingCoroutines();
        StartFeedingCoroutines();
        StartBreedingPairCoroutines();
        StartCoroutine(HappinessCoroutine());
    }

    // ── AFK calculations ─────────────────────────────────────────────────────

    private void CalculateAfkBreedingOutput(float hoursAway)
    {
        var penAnimals = GetAnimalsInEnclosure(EnclosureType.BreedingPen);
        var rateByFamily = new Dictionary<AnimalFamily, float>();

        foreach (var entry in penAnimals)
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null || data.family == AnimalFamily.Farm) continue;
            if (!rateByFamily.ContainsKey(data.family)) rateByFamily[data.family] = 0f;
            rateByFamily[data.family] += data.loveGeneration;
        }

        LastSessionEggsEarned = 0;
        foreach (var pair in rateByFamily)
        {
            int eggs = Mathf.FloorToInt(hoursAway * pair.Value);
            if (eggs <= 0) continue;
            var eggType = FamilyToEggType(pair.Key);
            if (eggType == null) continue;
            PlayerInventoryManager.Instance.AddEggsOfType(eggType.Value, eggs);
            LastSessionEggsEarned += eggs;
        }
    }

    private void CalculateAfkFeedingOutput(float hoursAway)
    {
        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.FeedingTrough))
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;

            int foodNeeded = Mathf.FloorToInt(hoursAway * inventoryConfig.foodConsumptionRatePerHour);
            bool hadFood = FertilizerManager.Instance.ConsumeFood(foodNeeded);

            float multiplier = AnimalAgeHelper.GetOutputMultiplier(entry, data, inventoryConfig);
            float effectivePoopRate = hadFood ? data.poopRate * 2f : data.poopRate;
            PoopManager.Instance.AddPoop(Mathf.FloorToInt(hoursAway * effectivePoopRate * multiplier));
        }

        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.Pasture))
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null || data.poopRate <= 0) continue;
            float multiplier = AnimalAgeHelper.GetOutputMultiplier(entry, data, inventoryConfig);
            PoopManager.Instance.AddPoop(Mathf.FloorToInt(hoursAway * data.poopRate * multiplier));
        }
    }

    private void CalculateAfkHappiness(float hoursAway, float foodAtSessionStart)
    {
        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.Pasture))
            entry.happinessAmount = Mathf.Min(
                entry.happinessAmount + inventoryConfig.happinessGrowthPerHour * hoursAway,
                inventoryConfig.maxHappiness);

        var activeAnimals = PlayerInventoryManager.Instance.GetActiveAnimals();
        if (activeAnimals.Count == 0) return;

        float foodConsumedPerHour = activeAnimals.Count * inventoryConfig.foodConsumptionRatePerHour;
        float hoursOfFood = foodConsumedPerHour > 0 ? foodAtSessionStart / foodConsumedPerHour : hoursAway;
        float hungryHours = Mathf.Max(0f, hoursAway - hoursOfFood);

        if (hungryHours > 0)
        {
            float decay = inventoryConfig.happinessHungerDecayPerHour * hungryHours;
            foreach (var entry in activeAnimals)
                entry.happinessAmount = Mathf.Max(entry.happinessAmount - decay, 0f);
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

            if (FertilizerManager.Instance.GetFoodAmount() <= 0)
            {
                float decay = inventoryConfig.happinessHungerDecayPerHour / 60f;
                foreach (var entry in PlayerInventoryManager.Instance.GetActiveAnimals())
                    entry.happinessAmount = Mathf.Max(entry.happinessAmount - decay, 0f);
            }
        }
    }

    // ── Real-time coroutines ──────────────────────────────────────────────────

    private void StartBreedingCoroutines()
    {
        var families = GetAnimalsInEnclosure(EnclosureType.BreedingPen)
            .Select(e => AnimalRoster.Instance.GetByIdentifier(e.animalIdentifier))
            .Where(d => d != null)
            .Select(d => d.family)
            .Distinct()
            .ToList();

        foreach (var family in families)
            StartCoroutine(BreedingCoroutine(family));
    }

    private IEnumerator BreedingCoroutine(AnimalFamily family)
    {
        float firstWait = 3600f / GetBreedingRateForFamily(family);
        yield return new WaitForSeconds(firstWait);

        var eggType = FamilyToEggType(family);
        if (eggType != null)
            PlayerInventoryManager.Instance.AddEggsOfType(eggType.Value, 1);

        while (true)
        {
            float rate = GetBreedingRateForFamily(family);
            float interval = 3600f / rate;
            yield return new WaitForSeconds(interval);

            eggType = FamilyToEggType(family);
            if (eggType != null)
                PlayerInventoryManager.Instance.AddEggsOfType(eggType.Value, 1);
        }
    }

    private void StartFeedingCoroutines()
    {
        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.FeedingTrough))
            StartCoroutine(FeedingCoroutine(entry.guid));
    }

    private IEnumerator FeedingCoroutine(string guid)
    {
        while (true)
        {
            yield return new WaitForSeconds(3600f);

            var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
            if (entry == null || entry.enclosureType != EnclosureType.FeedingTrough) yield break;

            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) yield break;

            bool hadFood = FertilizerManager.Instance.ConsumeFood(
                Mathf.CeilToInt(inventoryConfig.foodConsumptionRatePerHour));

            float effectivePoopRate = hadFood ? data.poopRate * 2f : data.poopRate;
            PoopManager.Instance.AddPoop(Mathf.FloorToInt(effectivePoopRate));
        }
    }

    // ── Breeding pairs ────────────────────────────────────────────────────────

    private void StartBreedingPairCoroutines()
    {
        foreach (var species in GetBreedingPairs())
            TryStartPairCoroutine(species);
    }

    private void TryStartPairCoroutine(AnimalEnum species)
    {
        if (_activePairCoroutines.Contains(species)) return;

        var animalData = AnimalRoster.Instance.GetByIdentifier(species);
        if (animalData == null) return;

        var eggType = FamilyToEggType(animalData.family);
        if (eggType == null) return;

        var eggData = EggRoster.Instance.GetByType(eggType.Value);
        if (eggData == null) return;

        _activePairCoroutines.Add(species);
        StartCoroutine(BreedingPairCoroutine(species, eggData));
    }

    private IEnumerator BreedingPairCoroutine(AnimalEnum species, EggData eggData)
    {
        while (HasBreedingPair(species))
        {
            yield return new WaitForSeconds(GetBreedingCooldownRemaining(species));

            if (!HasBreedingPair(species)) break;

            var result = AnimalRedeeming.Instance.OpenEgg(eggData, _breedingRarityBonus);
            if (result != null)
                PlayerInventoryManager.Instance.AddAnimal(result);

            _breedingRarityBonus = 0f;
            _lastBreedTimes[species] = DateTime.Now;
        }

        _activePairCoroutines.Remove(species);
    }

    private float GetBreedingCooldownRemaining(AnimalEnum species)
    {
        if (!_lastBreedTimes.TryGetValue(species, out DateTime lastBreed))
            return 0f;

        float elapsed = (float)(DateTime.Now - lastBreed).TotalSeconds;
        float cooldown = inventoryConfig.breedingPairCooldownHours * 3600f;
        return Mathf.Max(0f, cooldown - elapsed);
    }

    private bool HasBreedingPair(AnimalEnum species)
    {
        var penAnimals = GetAnimalsInEnclosure(EnclosureType.BreedingPen)
            .Where(a => a.animalIdentifier == species).ToList();
        return penAnimals.Any(a => a.sex == AnimalSex.Male && CanBreed(a)) &&
               penAnimals.Any(a => a.sex == AnimalSex.Female && CanBreed(a));
    }

    private List<AnimalEnum> GetBreedingPairs()
    {
        return GetAnimalsInEnclosure(EnclosureType.BreedingPen)
            .GroupBy(a => a.animalIdentifier)
            .Where(g => g.Any(a => a.sex == AnimalSex.Male && CanBreed(a))
                     && g.Any(a => a.sex == AnimalSex.Female && CanBreed(a)))
            .Select(g => g.Key)
            .ToList();
    }

    private bool CanBreed(PlayerAnimalEntry entry)
    {
        var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
        return data != null && AnimalAgeHelper.CanBreed(entry, data, inventoryConfig);
    }

    // ── Mulch ─────────────────────────────────────────────────────────────────

    public void ApplyMulchBonus(int bonus)
    {
        _breedingRarityBonus = Mathf.Min(
            _breedingRarityBonus + bonus,
            inventoryConfig.maxBreedingRarityBonus);
    }

    public float GetBreedingRarityBonus() => _breedingRarityBonus;

    // ── Assignment ────────────────────────────────────────────────────────────

    public bool AssignToEnclosure(string guid, EnclosureType targetType)
    {
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        if (entry == null) return false;
        if (entry.enclosureType == targetType) return true;

        if (targetType != EnclosureType.Storage && !CanAssignToEnclosure(targetType))
        {
            Debug.LogWarning($"[Enclosure] {entry.customAnimalName} → {targetType} REJECTED (full)");
            return false;
        }

        EnclosureType previousType = entry.enclosureType;
        bool wasActive = previousType != EnclosureType.Storage;
        bool willBeActive = targetType != EnclosureType.Storage;

        entry.enclosureType = targetType;
        Debug.Log($"[Enclosure] {entry.customAnimalName}: {previousType} → {targetType}");

        if (wasActive && !willBeActive)
            PhysicalAnimalController.Instance.RemoveAnimal(guid);
        else if (!wasActive && willBeActive)
            PhysicalAnimalController.Instance.InstantiateAnimals();

        if (targetType == EnclosureType.FeedingTrough)
            StartCoroutine(FeedingCoroutine(guid));

        if (targetType == EnclosureType.BreedingPen || previousType == EnclosureType.BreedingPen)
            RefreshBreedingPairs();

        DataController.instance.CompleteSave();
        return true;
    }

    private void RefreshBreedingPairs()
    {
        foreach (var species in GetBreedingPairs())
            TryStartPairCoroutine(species);
    }

    public bool CanAssignToEnclosure(EnclosureType type)
    {
        return type switch
        {
            EnclosureType.Pasture => PlayerInventoryManager.Instance.HasOpenActiveSlots(),
            EnclosureType.BreedingPen => GetAnimalsInEnclosure(EnclosureType.BreedingPen).Count < inventoryConfig.breedingPenCapacity,
            EnclosureType.FeedingTrough => GetAnimalsInEnclosure(EnclosureType.FeedingTrough).Count < inventoryConfig.feedingTroughCapacity,
            _ => true
        };
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    public List<PlayerAnimalEntry> GetAnimalsInEnclosure(EnclosureType type)
    {
        return PlayerInventoryManager.Instance.GetAnimals()
            .Where(a => a.enclosureType == type).ToList();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private float GetBreedingRateForFamily(AnimalFamily family)
    {
        var penAnimals = GetAnimalsInEnclosure(EnclosureType.BreedingPen);
        return penAnimals
            .Select(e => AnimalRoster.Instance.GetByIdentifier(e.animalIdentifier))
            .Where(d => d != null && d.family == family)
            .Sum(d => d.loveGeneration);
    }

    private EggType? FamilyToEggType(AnimalFamily family)
    {
        return family switch
        {
            AnimalFamily.Farm => EggType.Farm,
            _ => null
        };
    }

}
