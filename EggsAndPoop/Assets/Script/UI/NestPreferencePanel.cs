using UnityEngine;
using UnityEngine.UI;

public class NestPreferencePanel : MonoBehaviour
{
    public static NestPreferencePanel Instance;

    public GameObject panel;
    public Transform buttonContainer;
    public GameObject eggChoiceButtonPrefab;  // Button with RawImage icon + TextMeshProUGUI label

    private int _targetSlot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(int slotIndex)
    {
        _targetSlot = slotIndex;
        BuildButtons();
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void BuildButtons()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        var unlocked = PlayerInventoryManager.Instance.unlockedEggTypes;
        var ordered = EggProgressionManager.Instance.GetOrderedEggs();

        int count = 0;
        foreach (var eggData in ordered)
        {
            if (unlocked.Contains(eggData.eggType))
            {
                SpawnButton(eggData);
                count++;
            }
        }

        if (count % 2 != 0)
        {
            var placeholder = Instantiate(eggChoiceButtonPrefab, buttonContainer);
            var cg = placeholder.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    private void SpawnButton(EggData eggData)
    {
        var go = Instantiate(eggChoiceButtonPrefab, buttonContainer);
        go.GetComponent<EggChoiceButton>().Setup(eggData);

        var capturedType = eggData.eggType;
        go.GetComponent<Button>().onClick.AddListener(() =>
        {
            EggSlotManager.Instance.SetNestPreference(_targetSlot, capturedType);
            Close();
        });
    }
}
