using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        DataController.instance.CheckForSave();
        PlayerInventoryManager.Instance.LoadInventory();
        FertilizerManager.Instance.LoadFertilizer();  // snapshots lastTimeActive before any CompleteSave
        EnclosureManager.Instance.Initialize();        // AFK eggs + AFK poop, then fires m_AfkDataProcessed
        PhysicalAnimalController.Instance.InstantiateAnimals();
    }
}
