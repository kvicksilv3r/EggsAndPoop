#if UNITY_EDITOR
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private float simulateHours = 1f;

    private void Update()
    {
        // Simulate hours of love accumulation — fills slots gradually like real AFK
        if (Input.GetKeyDown(KeyCode.F1))
            EggSlotManager.Instance.DEBUG_SimulateHours(simulateHours);

        // Instantly spawn an egg at every empty nest
        if (Input.GetKeyDown(KeyCode.F2))
            EggSlotManager.Instance.DEBUG_ForceFireAllSlots();
    }
}
#endif
