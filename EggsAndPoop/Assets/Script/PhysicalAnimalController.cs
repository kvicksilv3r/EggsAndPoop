using System.Collections.Generic;
using UnityEngine;

public class PhysicalAnimalController : MonoBehaviour
{
    public static PhysicalAnimalController Instance;

    public GameObject spawnHolster;

    public Dictionary<PhysicalAnimal, PlayerAnimalEntry> animalLinks = new Dictionary<PhysicalAnimal, PlayerAnimalEntry>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.m_SaveDataLoaded.AddListener(InstantiateSavedAnimals);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            InstantiateSavedAnimals();
        }
    }

    public void InstantiateSavedAnimals()
    {
        foreach (var animal in PlayerInventoryManager.Instance.GetAnimals())
        {
            var spawnPos = spawnHolster.transform.GetChild(Random.Range(0, spawnHolster.transform.childCount)).transform.position;
            var forwardRot = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            var g = Instantiate(animal.animalData.prefab, spawnPos, Quaternion.LookRotation(forwardRot, Vector3.up));
            g.AddComponent<PhysicalAnimal>();

            animalLinks.Add(g.GetComponent<PhysicalAnimal>(), animal);
        }
    }
}
