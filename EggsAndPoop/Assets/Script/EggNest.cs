using UnityEngine;

public class EggNest : MonoBehaviour
{
    public int slotIndex;
    public Transform spawnPoint;
    public Vector3 eggScale = Vector3.one;
    [HideInInspector] public GameObject spawnedEgg;

    private void OnMouseDown()
    {
        if (spawnedEgg != null)
            EggNestManager.Instance.CollectEgg(slotIndex);
        else
            FloatingMessageController.Instance.Show($"Egg ready in: {EggSlotManager.Instance.TimeUntilNextEgg(slotIndex)}");
    }
}
