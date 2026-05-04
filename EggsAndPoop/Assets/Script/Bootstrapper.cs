using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        DataController.instance.CheckForSave();
        PlayerInventoryManager.Instance.LoadInventory();
        FoodManager.Instance.LoadFood();  // snapshots lastTimeActive before any CompleteSave
        FarmManager.Instance.LoadFarm();
        FarmManager.Instance.ApplyCurrentLevel();
        CoinManager.Instance.LoadCoins();
        EggSlotManager.Instance.LoadSlots();
        EggNestManager.Instance.LoadUnclaimedEggs();
        EnclosureManager.Instance.Initialize();        // AFK eggs + AFK poop, then fires m_AfkDataProcessed
        EggSlotManager.Instance.StartRealTimeGeneration();
        PhysicalAnimalController.Instance.InstantiateAnimals();
        EggNestManager.Instance.SpawnUnclaimedEggs();
    }
}
