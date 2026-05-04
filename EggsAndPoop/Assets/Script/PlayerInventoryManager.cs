using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance;
    public InventoryConfig inventoryConfig;
    public QuirkData quirkData;

    public int extendedEggCapacity = 0;

    public List<EggType> unlockedEggTypes = new List<EggType>();

    public List<PlayerAnimalEntry> playerAnimals = new List<PlayerAnimalEntry>();
    public List<PlayerEggEntry> playerEggs = new List<PlayerEggEntry>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddEggs(int amount)
    {
        for (int i = 0; i < amount; i++)
            AddEgg();
    }

    public void AddEggsOfType(EggType eggType, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!CanAddEgg()) return;

            var existing = playerEggs.FirstOrDefault(e => e.eggType == eggType);
            if (existing != null)
            {
                existing.eggAmount++;
            }
            else
            {
                playerEggs.Add(new PlayerEggEntry { eggType = eggType, eggAmount = 1 });
            }
        }
    }

    public void AddEgg()
    {
        if (CanAddEgg() == false)
        {
            return;
        }

        EggType eggType = unlockedEggTypes[UnityEngine.Random.Range(0, unlockedEggTypes.Count)];

        var existingEntry = playerEggs.FirstOrDefault(e => e.eggType == eggType);

        if (existingEntry != null)
        {
            existingEntry.eggAmount++;
        }
        else
        {
            var newEggEntry = new PlayerEggEntry();
            newEggEntry.eggType = eggType;
            newEggEntry.eggAmount = 1;
            playerEggs.Add(newEggEntry);
        }
    }

    public void RemoveEgg(EggType eggType)
    {
        var eggIndex = playerEggs.FindIndex(e => e.eggType == eggType);
        playerEggs[eggIndex].eggAmount--;

        if (playerEggs[eggIndex].eggAmount <= 0)
        {
            playerEggs.RemoveAt(eggIndex);
        }
    }

    public bool CanAddEgg()
    {
        var totalOwnedEggs = 0;

        foreach (var egg in playerEggs)
        {
            totalOwnedEggs += egg.eggAmount;
        }

        if (totalOwnedEggs < GetMaxEggCapacity())
        {
            return true;
        }

        return false;
    }

    public void LoadInventory()
    {
        var saveData = DataController.instance.GetData();

        playerEggs = (saveData.playerEggs ?? new PlayerEggEntry[0]).ToList();
        playerAnimals = (saveData.playerAnimals ?? new PlayerAnimalEntry[0]).ToList();
        unlockedEggTypes = (saveData.unlockedEggTypes ?? new EggType[0]).ToList();

        if (!unlockedEggTypes.Contains(EggType.Farm))
            unlockedEggTypes.Add(EggType.Farm);

        extendedEggCapacity = saveData.extraEggCapacity;

        print("Inventory loaded");
    }

    public int GetMaxEggCapacity()
    {
        return inventoryConfig.baseMaxOwnedEggs + extendedEggCapacity + FarmManager.Instance.GetBonusEggCapacity();
    }

    public void AddUnlockedEggType(EggType eggType)
    {
        unlockedEggTypes.Add(eggType);
    }

    public void AddAnimal(AnimalData addedAnimal)
    {
        PlayerAnimalEntry entry = new PlayerAnimalEntry();
        entry.animalIdentifier = addedAnimal.animalIdentifier;
        entry.timeOfBirth = DateTime.Now.ToLong();
        entry.guid = Guid.NewGuid().ToString();
        entry.physicalAnimalData = new PhysicalAnimalData();
        entry.enclosureType = HasOpenActiveSlots() ? EnclosureType.Pasture : EnclosureType.Storage;
        entry.quirkId = quirkData != null ? UnityEngine.Random.Range(0, quirkData.quirks.Length) : 0;
        entry.sex = addedAnimal.sexOverride switch
        {
            AnimalSexOverride.Male => AnimalSex.Male,
            AnimalSexOverride.Female => AnimalSex.Female,
            _ => (AnimalSex)UnityEngine.Random.Range(0, 2)
        };
        entry.customAnimalName = AnimalNameGenerator.GetRandomName(entry.sex);

        playerAnimals.Add(entry);
    }

    public void RemoveAnimal(string animalGuid)
    {
        var removedAnimal = playerAnimals.FirstOrDefault(a => a.guid == animalGuid);
        playerAnimals.Remove(removedAnimal);
        PhysicalAnimalController.Instance.RemoveAnimal(animalGuid);

        DataController.instance.CompleteSave();
    }

    public void MoveAnimalToStorage(string animalGuid)
    {
        EnclosureManager.Instance.AssignToEnclosure(animalGuid, EnclosureType.Storage);
    }

    public void MoveAnimalToFarm(string animalGuid)
    {
        EnclosureManager.Instance.AssignToEnclosure(animalGuid, EnclosureType.Pasture);
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        UpdatePhysicalAnimals();
        saveData.extraEggCapacity = extendedEggCapacity;
        saveData.playerEggs = playerEggs.ToArray();
        saveData.unlockedEggTypes = unlockedEggTypes.ToArray();
        saveData.playerAnimals = playerAnimals.ToArray();
    }

    private void UpdatePhysicalAnimals()
    {
        foreach (var playerAnimal in GetActiveAnimals())
        {
            if (!PhysicalAnimalController.Instance.AnimalExists(playerAnimal.guid))
            {
                continue;
            }
            playerAnimal.physicalAnimalData = PhysicalAnimalController.Instance.GetAnimalPhysicalData(playerAnimal.guid);
        }
    }

    public bool HasEggs()
    {
        return playerEggs.Count > 0;
    }

    public bool HasMaxEggs()
    {
        return GetCurrentEggCount() == GetMaxEggCapacity();
    }

    public int GetMaxAnimalCount()
    {
        return inventoryConfig.baseMaxOwnedAnimals + FarmManager.Instance.GetBonusAnimalSlots();
    }

    public int GetMaxActiveAnimalCount()
    {
        return inventoryConfig.baseMaxActiveAnimals + FarmManager.Instance.GetBonusAnimalSlots();
    }

    public int GetCurrentEggCount()
    {
        var totalOwnedEggs = 0;

        foreach (var egg in playerEggs)
        {
            totalOwnedEggs += egg.eggAmount;
        }

        return totalOwnedEggs;
    }

    public bool HasOpenAnimalSlots()
    {
        return playerAnimals.Count < GetMaxAnimalCount();
    }

    public bool HasOpenActiveSlots()
    {
        return GetActiveAnimals().Count < GetMaxActiveAnimalCount();
    }

    public List<PlayerAnimalEntry> GetAnimals()
    {
        return playerAnimals;
    }

    public List<PlayerAnimalEntry> GetActiveAnimals()
    {
        return playerAnimals.Where(a => a.enclosureType != EnclosureType.Storage).ToList();
    }

    public List<PlayerAnimalEntry> GetStorageAnimals()
    {
        return playerAnimals.Where(a => a.enclosureType == EnclosureType.Storage).ToList();
    }
}
