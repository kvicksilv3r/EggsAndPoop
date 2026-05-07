using System;
using System.Collections.Generic;
using UnityEngine;

public class EggOpeningController : MonoBehaviour
{
    public static EggOpeningController Instance { get; private set; }

    private AnimalData generatedAnimal;
    private EggData eggData;

    public GameObject animalInfoBox;

    private bool hasMaxEggs = false;

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.m_EggCrackedOpen.AddListener(EggCrackedOpen);
    }

    public EggData GetEgg()
    {
        return eggData;
    }

    public void InitiateEggOpening(EggData data)
    {
        if (!PlayerInventoryManager.Instance.HasOpenAnimalSlots())
        {
            FloatingMessageController.Instance.Show("Your farm and barn are full. Make some room first!");
            return;
        }

        eggData = data;
        hasMaxEggs = PlayerInventoryManager.Instance.HasMaxEggs();
        EggHelper.Instance.SetupEgg(eggData);
        GameManager.Instance.m_SetupEgg.Invoke();
    }

    public void EggCrackedOpen()
    {
        FetchAnimal();
        DisplayOpenedAnimal();
        RegisterNewAnimal();
        RemoveEggFromInventory();
        DisplayAnimalInformation();

        GameManager.Instance.m_EggCrackedOpenPost.Invoke();
    }

    private void DisplayAnimalInformation()
    {
        animalInfoBox.GetComponent<AnimalHatchedInformation>().DisplayInfo(generatedAnimal);
    }

    public void FetchAnimal()
    {
        generatedAnimal = AnimalRedeeming.Instance.OpenEgg(eggData);
    }

    public void DisplayOpenedAnimal()
    {
        AnimalRedeemingVisuals.Instance.DisaplayAnimal(generatedAnimal);
    }

    public void RegisterNewAnimal()
    {
        bool farmWasFull = !PlayerInventoryManager.Instance.HasOpenActiveSlots();
        PlayerInventoryManager.Instance.AddAnimal(generatedAnimal);

        if (farmWasFull)
            FloatingMessageController.Instance.Show("Your farm is full — they're settling into the barn for now.");
    }

    public void RemoveEggFromInventory()
    {
        PlayerInventoryManager.Instance.RemoveEgg(eggData.eggType);
    }

    public void OpenAllEggs()
    {
        if (!PlayerInventoryManager.Instance.HasEggs()) return;

        if (!PlayerInventoryManager.Instance.HasOpenAnimalSlots())
        {
            FloatingMessageController.Instance.Show("Your farm and barn are full. Make some room first!");
            return;
        }

        // Snapshot egg counts before modifying the list during iteration
        var snapshot = new List<(EggType type, int count)>();
        foreach (var e in PlayerInventoryManager.Instance.playerEggs)
            snapshot.Add((e.eggType, e.eggAmount));

        int countBefore = PlayerInventoryManager.Instance.playerAnimals.Count;

        foreach (var (type, count) in snapshot)
        {
            var data = EggRoster.Instance.GetByType(type);
            if (data == null) continue;

            for (int i = 0; i < count; i++)
            {
                if (!PlayerInventoryManager.Instance.HasOpenAnimalSlots()) break;
                var animal = AnimalRedeeming.Instance.OpenEgg(data);
                PlayerInventoryManager.Instance.AddAnimal(animal);
                PlayerInventoryManager.Instance.RemoveEgg(type);
            }
        }

        // Collect the entries that were just added so the panel shows names + icons
        var newEntries = new List<PlayerAnimalEntry>();
        var allAnimals = PlayerInventoryManager.Instance.playerAnimals;
        for (int i = countBefore; i < allAnimals.Count; i++)
            newEntries.Add(allAnimals[i]);

        GameManager.Instance.m_EggCrackedOpenPost.Invoke();
        OpenAllResultsPanel.Instance?.Show(newEntries);
    }
}
