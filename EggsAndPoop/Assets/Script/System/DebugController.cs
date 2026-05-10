#if UNITY_EDITOR
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private float simulateHours = 1f;
    [SerializeField] private NukeAllSaveData nukeAllSaveData;

    private void Update()
    {
        // Simulate hours of love accumulation — fills slots gradually like real AFK
        if (Input.GetKeyDown(KeyCode.F1))
            EggSlotManager.Instance.DEBUG_SimulateHours(simulateHours);

        // Instantly spawn an egg at every empty nest
        if (Input.GetKeyDown(KeyCode.F2))
            EggSlotManager.Instance.DEBUG_ForceFireAllSlots();

        // Clear all unhatched eggs from inventory
        if (Input.GetKeyDown(KeyCode.F3))
            nukeAllSaveData.NukeAllEggs();

        // Reset unlocked eggs back to starter only
        if (Input.GetKeyDown(KeyCode.F4))
            nukeAllSaveData.ResetUnlockedEggs();

        // Unlock the next egg in the progression
        if (Input.GetKeyDown(KeyCode.F5))
            EggProgressionManager.Instance.DEBUG_UnlockNextEgg();
    }
}
#endif
