using System.Linq;
using UnityEngine;

public class AnimalRedeeming : MonoBehaviour
{
    public EggData activeEgg;

    public EggData debugEgg;

    public DropRates dropRates;

    public static AnimalRedeeming Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetEgg(EggData newEgg)
    {
        activeEgg = newEgg;
    }

    public AnimalData OpenEgg(EggData newEgg, float rarityBonus = 0f)
    {
        SetEgg(newEgg);
        return OpenEgg(rarityBonus);
    }

    public AnimalData OpenEgg(float rarityBonus = 0f)
    {
        if (!activeEgg)
        {
            print("No egg found");

            if (!debugEgg)
            {
                print("Not even a debug egg :C");
                return null;
            }

            activeEgg = debugEgg;
        }

        return RandomizeAnimal(rarityBonus);
    }

    private AnimalData RandomizeAnimal(float rarityBonus = 0f)
    {
        var minRarity = activeEgg.eggMinRarity;
        var maxRarity = activeEgg.eggMaxRarity;

        var minRoll = dropRates.dropRates.Where(d => d.rarity == minRarity).FirstOrDefault().dropRate;
        var maxRoll = dropRates.dropRates.Where(d => d.rarity == maxRarity).FirstOrDefault().maxRollIfHighestRarity;

        var baseRoll = Random.Range(minRoll, maxRoll);
        var selectedDropNumber = Mathf.Clamp(Mathf.RoundToInt(baseRoll + rarityBonus), minRoll, maxRoll - 1);
        var chosenRarity = GenerateRarity(selectedDropNumber);

        var availableAnimals = AnimalRoster.Instance.GetByFamily(activeEgg.containedAnimalTypes)
            .Where(a => a.rarity == chosenRarity).ToArray();

        return availableAnimals[Random.Range(0, availableAnimals.Length)];
    }

    private AnimalRarity GenerateRarity(int selectedDropNumber)
    {
        AnimalRarity selectedRarity = AnimalRarity.Common;

        for (int i = 0; i < dropRates.dropRates.Length; i++)
        {
            if (selectedDropNumber >= dropRates.dropRates[i].dropRate)
            {
                selectedRarity = dropRates.dropRates[i].rarity;
            }

            else
            {
                break;
            }
        }

        return selectedRarity;
    }
}
