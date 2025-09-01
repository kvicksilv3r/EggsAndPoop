using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;

public class AnimalRoster : MonoBehaviour
{
    public AnimalData[] animals;

    public static AnimalRoster Instance;

    private void Awake()
    {
        Instance = this;

        LoadAnimals();
        LoadEggScene();
    }

    private void LoadEggScene()
    {

    }

    private void LoadAnimals()
    {
        animals = Resources.LoadAll<AnimalData>("AnimalData");
    }

    public AnimalData[] GetData()
    {
        return animals;
    }

    public AnimalData[] GetByFamily(AnimalFamily family)
    {
        return animals.Where(a => a.family == family).ToArray();
    }

    public AnimalData[] GetByFamily(AnimalFamily[] families)
    {
        return animals.Where(a => families.Contains(a.family)).ToArray();
    }
}
