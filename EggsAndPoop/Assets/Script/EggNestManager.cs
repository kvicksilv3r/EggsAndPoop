using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EggNestManager : MonoBehaviour
{
    public static EggNestManager Instance;

    public GameObject farmEggPrefab;

    private EggNest[] _nests;
    private List<UnclaimedEggData> _unclaimedEggs = new List<UnclaimedEggData>();

    private void Awake()
    {
        Instance = this;
        _nests = FindObjectsOfType<EggNest>();
    }

    public void LoadUnclaimedEggs()
    {
        var data = DataController.instance.GetData();
        _unclaimedEggs = data.unclaimedEggs ?? new List<UnclaimedEggData>();
    }

    public void SpawnUnclaimedEggs()
    {
        foreach (var entry in _unclaimedEggs)
            SpawnAtNest(entry.slotIndex, entry.eggType);
    }

    public bool IsNestOccupied(int slotIndex)
    {
        return _unclaimedEggs.Any(e => e.slotIndex == slotIndex);
    }

    public void SpawnEggAtNest(int slotIndex, EggType eggType)
    {
        if (IsNestOccupied(slotIndex)) return;

        _unclaimedEggs.Add(new UnclaimedEggData { slotIndex = slotIndex, eggType = eggType });
        SpawnAtNest(slotIndex, eggType);
    }

    private void SpawnAtNest(int slotIndex, EggType eggType)
    {
        var nest = _nests.FirstOrDefault(n => n.slotIndex == slotIndex);
        if (nest == null) return;

        var eggData = EggRoster.Instance.GetByType(eggType);
        var prefab = (eggData != null && eggData.eggPrefab != null) ? eggData.eggPrefab : farmEggPrefab;
        if (prefab == null) return;

        var point = nest.spawnPoint != null ? nest.spawnPoint : nest.transform;
        var go = Instantiate(prefab, point.position, point.rotation);
        go.transform.localScale = nest.eggScale;
        nest.spawnedEgg = go;
    }

    public void CollectEgg(int slotIndex)
    {
        var entry = _unclaimedEggs.FirstOrDefault(e => e.slotIndex == slotIndex);
        if (entry == null) return;

        if (!PlayerInventoryManager.Instance.CanAddEgg()) return;

        PlayerInventoryManager.Instance.AddEggsOfType(entry.eggType, 1);
        _unclaimedEggs.Remove(entry);

        var nest = _nests.FirstOrDefault(n => n.slotIndex == slotIndex);
        if (nest != null && nest.spawnedEgg != null)
        {
            Destroy(nest.spawnedEgg);
            nest.spawnedEgg = null;
        }

        DataController.instance.CompleteSave();
    }

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.unclaimedEggs = _unclaimedEggs;
    }
}
