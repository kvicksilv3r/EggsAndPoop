using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance;
    public InventoryConfig inventoryConfig;

    public int extendedEggCapacity = 0;

    public List<EggType> unlockedEggTypes = new List<EggType>();

    public List<PlayerAnimalEntry> playerAnimals = new List<PlayerAnimalEntry>();
    public List<PlayerEggEntry> playerEggs = new List<PlayerEggEntry>();

    private void Awake()
    {
        Instance = this;
        unlockedEggTypes.Add(EggType.Farm);
    }

    private void Start()
    {
        GameManager.Instance.m_SaveDataLoaded.AddListener(LoadInventory);
    }

    public void AddEgg()
    {
        if (CanAddEgg() == false)
        {
            return;
        }

        EggType eggType = unlockedEggTypes[Random.Range(0, unlockedEggTypes.Count)];

        var hasEggType = playerEggs.Select(e => e.eggType).Count() > 0;

        if (hasEggType)
        {
            playerEggs.Where(e => e.eggType == eggType).FirstOrDefault().eggAmount++;
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

        if (totalOwnedEggs < MaxEggCapacity())
        {
            return true;
        }

        return false;
    }

    public void LoadInventory()
    {
        var saveData = DataController.instance.GetData();
        playerEggs.Clear();
        playerEggs = saveData.playerEggs.ToList();

        playerAnimals.Clear();
        playerAnimals = saveData.playerAnimals.ToList();
    }

    public int MaxEggCapacity()
    {
        return inventoryConfig.baseMaxOwnedEggs + extendedEggCapacity;
    }

    public void AddUnlockedEggType(EggType eggType)
    {
        unlockedEggTypes.Add(eggType);
    }

    public void AddAnimal(AnimalData addedAnimal)
    {
        PlayerAnimalEntry entry = new PlayerAnimalEntry();
        entry.animalData = addedAnimal;
        entry.customAnimalName = addedAnimal.animalName;

        playerAnimals.Add(entry);
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.extraEggCapacity = extendedEggCapacity;
        saveData.playerEggs = playerEggs.ToArray();
        saveData.unlockedEggTypes = unlockedEggTypes.ToArray();
        saveData.playerAnimals = playerAnimals.ToArray();
    }
}
