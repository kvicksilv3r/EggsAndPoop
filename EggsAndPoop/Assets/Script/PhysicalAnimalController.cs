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
    }

    private void Start()
    {
        GameManager.Instance.m_EggCrackedOpenPost.AddListener(InstantiateAnimals);
    }

    public void InstantiateAnimals()
    {
        foreach (var animal in PlayerInventoryManager.Instance.GetAnimals())
        {
            if (animalLinks.ContainsKey(animal.guid))
            {
                continue;
            }

            var spawnPos = spawnHolster.transform.GetChild(UnityEngine.Random.Range(0, spawnHolster.transform.childCount)).transform.position;
            var forwardRot = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            var specificAnimalPrefab = AnimalRoster.Instance.GetByIdentifier(animal.animalIdentifier).prefab;

            var g = Instantiate(physicalAnimalPrefab, spawnPos, Quaternion.identity);
            Instantiate(specificAnimalPrefab, g.transform);
            var p = g.GetComponent<PhysicalAnimal>();
            p.physicalAnimalData = animal.physicalAnimalData;
            p.animalData = AnimalRoster.Instance.GetByIdentifier(animal.animalIdentifier);

            g.name = animal.customAnimalName;

            animalLinks.Add(animal.guid, g.GetComponent<PhysicalAnimal>());
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
