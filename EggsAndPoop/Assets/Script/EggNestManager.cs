using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EggNestManager : MonoBehaviour
{
    public static EggNestManager Instance;

    public GameObject farmEggPrefab;
    public RectTransform eggCollectTarget;

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
        var prefab = (eggData != null && eggData.nestEggPrefab != null) ? eggData.nestEggPrefab : farmEggPrefab;
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

        if (!PlayerInventoryManager.Instance.CanAddEgg())
        {
            FloatingMessageController.Instance.Show("Your bag is full. Hatch an egg to make room!");
            return;
        }

        PlayerInventoryManager.Instance.AddEggsOfType(entry.eggType, 1);
        _unclaimedEggs.Remove(entry);

        var nest = _nests.FirstOrDefault(n => n.slotIndex == slotIndex);
        if (nest != null && nest.spawnedEgg != null)
        {
            var eggGo = nest.spawnedEgg;
            var worldPos = eggGo.transform.position;
            var eggData = EggRoster.Instance.GetByType(entry.eggType);
            nest.spawnedEgg = null;
            nest.IsCollecting = true;
            StartCoroutine(ShrinkThenCollect(eggGo, worldPos, eggData?.eggIcon, nest));
        }

        DataController.instance.CompleteSave();
    }

    private IEnumerator ShrinkThenCollect(GameObject eggGo, Vector3 worldPos, Texture2D icon, EggNest nest)
    {
        float duration = 0.12f;
        float t = 0;
        Vector3 startScale = eggGo.transform.localScale;
        while (t < duration)
        {
            t += Time.deltaTime;
            eggGo.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            yield return null;
        }
        Destroy(eggGo);
        nest.IsCollecting = false;

        if (CollectFX.Instance != null && eggCollectTarget != null)
            CollectFX.Instance.Play(worldPos, icon, eggCollectTarget);
    }

    public int GetUnclaimedEggCount() => _unclaimedEggs.Count;

    public void ModifySaveData(ref SaveData saveData)
    {
        saveData.unclaimedEggs = _unclaimedEggs;
    }
}
