using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnclosureManager : MonoBehaviour
{
    public static EnclosureManager Instance;

    public InventoryConfig inventoryConfig;

    private const float BreedingPenFloorEggsPerHour = 1f / 24f;

    public int LastSessionEggsEarned { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // ── Startup ──────────────────────────────────────────────────────────────

    public void Initialize()
    {
        float hoursAway = FertilizerManager.Instance.CachedHoursAway;

        CalculateAfkBreedingOutput(hoursAway);
        CalculateAfkFeedingOutput(hoursAway);

        DataController.instance.CompleteSave();
        GameManager.Instance.m_AfkDataProcessed.Invoke();

        StartBreedingCoroutines();
        StartFeedingCoroutines();
    }

    // ── AFK calculations ─────────────────────────────────────────────────────

    private void CalculateAfkBreedingOutput(float hoursAway)
    {
        var penAnimals = GetAnimalsInEnclosure(EnclosureType.BreedingPen);
        var rateByFamily = new Dictionary<AnimalFamily, float>();

        rateByFamily[AnimalFamily.Farm] = BreedingPenFloorEggsPerHour;

        foreach (var entry in penAnimals)
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;
            if (!rateByFamily.ContainsKey(data.family)) rateByFamily[data.family] = 0f;
            rateByFamily[data.family] += data.eggContribution;
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
        var troughAnimals = GetAnimalsInEnclosure(EnclosureType.FeedingTrough);

        foreach (var entry in troughAnimals)
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null) continue;

            int foodNeeded = Mathf.FloorToInt(hoursAway * inventoryConfig.foodConsumptionRatePerHour);
            bool hadFood = FertilizerManager.Instance.ConsumeFood(foodNeeded);

            float effectivePoopRate = hadFood ? data.poopRate * 2f : data.poopRate;
            FertilizerManager.Instance.AddFertilizer(Mathf.FloorToInt(hoursAway * effectivePoopRate));
        }

        // Pasture animals still produce normal AFK poop
        foreach (var entry in GetAnimalsInEnclosure(EnclosureType.Pasture))
        {
            var data = AnimalRoster.Instance.GetByIdentifier(entry.animalIdentifier);
            if (data == null || data.poopRate <= 0) continue;
            FertilizerManager.Instance.AddFertilizer(Mathf.FloorToInt(hoursAway * data.poopRate));
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

        // Always run the floor for Farm
        if (!families.Contains(AnimalFamily.Farm))
            families.Add(AnimalFamily.Farm);

        foreach (var family in families)
            StartCoroutine(BreedingCoroutine(family));
    }

    private IEnumerator BreedingCoroutine(AnimalFamily family)
    {
        while (true)
        {
            float rate = GetBreedingRateForFamily(family);
            float interval = 3600f / rate;
            yield return new WaitForSeconds(interval);

            var eggType = FamilyToEggType(family);
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
            FertilizerManager.Instance.AddFertilizer(Mathf.FloorToInt(effectivePoopRate));
        }
    }

    // ── Assignment ────────────────────────────────────────────────────────────

    public void AssignToEnclosure(string guid, EnclosureType targetType)
    {
        var entry = PlayerInventoryManager.Instance.GetAnimals().Find(a => a.guid == guid);
        if (entry == null) return;
        if (entry.enclosureType == targetType) return;

        if (targetType != EnclosureType.Storage && !CanAssignToEnclosure(targetType))
        {
            Debug.LogWarning($"Enclosure {targetType} is full.");
            return;
        }

        bool wasActive = entry.enclosureType != EnclosureType.Storage;
        bool willBeActive = targetType != EnclosureType.Storage;

        entry.enclosureType = targetType;

        if (wasActive && !willBeActive)
            PhysicalAnimalController.Instance.RemoveAnimal(guid);
        else if (!wasActive && willBeActive)
            PhysicalAnimalController.Instance.InstantiateAnimals();

        if (targetType == EnclosureType.FeedingTrough)
            StartCoroutine(FeedingCoroutine(guid));

        DataController.instance.CompleteSave();
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
        float rate = penAnimals
            .Select(e => AnimalRoster.Instance.GetByIdentifier(e.animalIdentifier))
            .Where(d => d != null && d.family == family)
            .Sum(d => d.eggContribution);

        if (family == AnimalFamily.Farm)
            rate = Mathf.Max(rate, BreedingPenFloorEggsPerHour);

        return rate;
    }

    private EggType? FamilyToEggType(AnimalFamily family)
    {
        // Extend this as new egg types are added.
        return family switch
        {
            AnimalFamily.Farm => EggType.Farm,
            _ => null
        };
    }

    public string TimeUntilNextEgg()
    {
        float totalRate = GetBreedingRateForFamily(AnimalFamily.Farm);
        float intervalSeconds = 3600f / totalRate;
        var span = System.TimeSpan.FromSeconds(intervalSeconds);

        string hours = span.Hours > 0 ? span.Hours + "h " : "";
        string minutes = span.Minutes > 0 ? span.Minutes + "m " : "";
        string seconds = span.Seconds + "s";
        return hours + minutes + seconds;
    }
}
