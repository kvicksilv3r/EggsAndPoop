using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalAnimalController : MonoBehaviour
{
    public static PhysicalAnimalController Instance;

    public GameObject spawnHolster;
    public GameObject physicalAnimalPrefab;

    public Dictionary<string, PhysicalAnimal> animalLinks = new Dictionary<string, PhysicalAnimal>();

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(InstantiateAnimals);
    }

    public void InstantiateAnimals()
    {
        foreach (var animal in PlayerInventoryManager.Instance.GetActiveAnimals())
        {
            if (animalLinks.ContainsKey(animal.guid))
            {
                continue;
            }

            var animalData = AnimalRoster.Instance.GetByIdentifier(animal.animalIdentifier);
            if (animalData == null)
            {
                Debug.LogError($"No AnimalData found for identifier {animal.animalIdentifier}. Skipping animal '{animal.customAnimalName}'.");
                continue;
            }

            var spawnPos = spawnHolster.transform.GetChild(UnityEngine.Random.Range(0, spawnHolster.transform.childCount)).transform.position;

            var g = Instantiate(physicalAnimalPrefab, spawnPos, Quaternion.identity);
            Instantiate(animalData.prefab, g.transform);

            g.name = animal.customAnimalName;

            var p = g.GetComponent<PhysicalAnimal>();
            p.Initialize(animalData, animal.physicalAnimalData);
            p.animalGuid = animal.guid;

            animalLinks.Add(animal.guid, p);
        }
    }

    public void RemoveAnimal(string animalGuid)
    {
        if (animalLinks.ContainsKey(animalGuid) == false)
        {
            print("Animal links did not contain specified animal");
            return;
        }

        DestroyImmediate(animalLinks[animalGuid].gameObject);
        animalLinks.Remove(animalGuid);
    }

    public bool AnimalExists(string animalGuid)
    {
        return (animalLinks.ContainsKey(animalGuid));
    }

    public PhysicalAnimalData GetAnimalPhysicalData(string guid)
    {
        return animalLinks[guid].physicalAnimalData;
    }
}
