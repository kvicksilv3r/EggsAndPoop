using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        DataController.instance.CheckForSave();
        PlayerInventoryManager.Instance.LoadInventory();
        FertilizerManager.Instance.LoadFertilizer();  // snapshots lastTimeActive before any CompleteSave
        FarmManager.Instance.LoadFarm();
        FarmManager.Instance.ApplyCurrentLevel();
        PoopManager.Instance.LoadPoop();
        CoinManager.Instance.LoadCoins();
        EnclosureManager.Instance.Initialize();        // AFK eggs + AFK poop, then fires m_AfkDataProcessed
        PhysicalAnimalController.Instance.InstantiateAnimals();
    }
}
