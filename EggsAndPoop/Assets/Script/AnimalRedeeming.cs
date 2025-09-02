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

    public AnimalData OpenEgg(EggData newEgg)
    {
        SetEgg(newEgg);
        return OpenEgg();
    }

    public AnimalData OpenEgg()
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

        var chosenAnimal = RanomizeAnimal();

        return chosenAnimal;
    }

    private AnimalData RanomizeAnimal()
    {
        var minRarity = activeEgg.eggMinRarity;
        var maxRarity = activeEgg.eggMaxRarity;

        var minRoll = dropRates.dropRates.Where(d => d.rarity == minRarity).FirstOrDefault().dropRate;
        var maxRoll = dropRates.dropRates.Where(d => d.rarity == maxRarity).FirstOrDefault().maxRollIfHighestRarity;

        var selectedDropNumber = Random.Range(minRoll, maxRoll);
        var chosenRarity = GenerateRarity(selectedDropNumber);

        var availableAnimals = AnimalRoster.Instance.GetByFamily(activeEgg.eggTypes)
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
